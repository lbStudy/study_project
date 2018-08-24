using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace Base
{
    public enum ProtocolCategory
    {
        Request,
        Response,
        Message
    }
    public class ProtocolInfo
    {
        private IProtocolHandle handleInterface;
        private Type handleClass;
        private ProtocolCategory protocolCategory;
        private int opcode;
        private AppType toServer;
        private AppType fromServer;
        private SystemType systemType;
        public SystemType SysType { get { return systemType; } }
        //private bool isCompress;
        private bool isEncrypt;
        private Type protocolBodyType;
        public byte[] opcodeBytes;
        public byte[] selectsBytes;

        public int takeCount;
        public int backCount;

        public IProtocolHandle HandleInterface { get { return handleInterface; } }
        public Type HandleClass { get { return handleClass; } }
        public ProtocolCategory ProtocolCategory { get { return protocolCategory; }}
        public int Opcode { get { return opcode; } }
        public AppType ToServer { get { return toServer; } }
        public AppType FromServer { get { return fromServer; } }
        //public bool IsCompress { get { return isCompress; } }
        public bool IsEncrypt { get { return isEncrypt; } }
        public Type ProtocolBodyType { get { return protocolBodyType; } }

        private Queue<object> cacheBodys = new Queue<object>();

        public ProtocolInfo(ProtocolCategory protocolType, int opcode, AppType toServer, AppType fromServer, bool isEncrypt, Type protocolBodyType)
        {
            this.protocolCategory = protocolType;
            this.opcode = opcode;
            this.toServer = toServer;
            this.fromServer = fromServer;
            //this.isCompress = isCompress;
            this.isEncrypt = isEncrypt;
            this.protocolBodyType = protocolBodyType;
            opcodeBytes = BitConverter.GetBytes(opcode);
            selectsBytes = new byte[1];
            selectsBytes[0] = 0;
            if (isEncrypt)
            {//加密处理
                selectsBytes[0] |= 2;
            }
            takeCount = 0;
            backCount = 0;
        }
        public void SetHandleInfo(Type handleClass, IProtocolHandle handleInterface)
        {
            this.handleClass = handleClass;
            this.handleInterface = handleInterface;
        }
        public object Take()
        {
            object obj = null;
            if (cacheBodys.Count == 0)
            {
                obj = Activator.CreateInstance(protocolBodyType);
                IAwake awake = obj as IAwake;
                if (awake != null)
                    awake.Awake();
            }
            else
            {
                obj = cacheBodys.Dequeue();
            }
            takeCount++;
            return obj;
        }
        public void Back(object obj)
        {
            IDisposable disposable = obj as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
                if(cacheBodys.Count < 10)
                    cacheBodys.Enqueue(obj);
            }
            backCount++;
        }
    }
    public class ProtocolDispatcher
    {
        private static ProtocolDispatcher instance;
        public static ProtocolDispatcher Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProtocolDispatcher();
                }
                return instance;
            }
        }
        Dictionary<int, ProtocolInfo> protocolInfoDic = new Dictionary<int, ProtocolInfo>();
        Dictionary<Type, ProtocolInfo> msgOpcodes = new Dictionary<Type, ProtocolInfo>();

        public ProtocolDispatcher()
        {
            Dec();
        }

        async void Dec()
        {
            while(true)
            {
                await Task.Delay(1000 * 60 * 10);

                foreach(ProtocolInfo v in msgOpcodes.Values)
                {
                    if(v.takeCount - v.backCount < 5)
                        Log.Debug($"{v.ProtocolBodyType.Name}:  takeCount {v.takeCount} backCount {v.backCount}");
                    else
                        Log.Debug($"{v.ProtocolBodyType.Name}(Warning:{v.takeCount - v.backCount}):  takeCount({v.takeCount}) backCount({v.backCount})");
                }
            }
        }

        public void LoadConfig()
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create("../../protocol.xml", settings);
            xmldoc.Load(reader);

            XmlNode root = xmldoc.SelectSingleNode("protocols");
            if (root == null)
            {
                System.Console.WriteLine("load protocol error");
                return;
            }
            XmlNodeList nodeList = root.SelectNodes("msg");
            if(nodeList == null)
            {
                return;
            }
            protocolInfoDic.Clear();
            msgOpcodes.Clear();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            Assembly model = null;
            for (int i = 0; i < assemblys.Length; i++)
            {
                if(assemblys[i].GetName().Name == "Model")
                {
                    model = assemblys[i];
                }
            }
            if(model == null)
            {
                Log.Debug("load model Assembly fail.");
            }
            foreach (XmlNode node in nodeList)
            {
                XmlElement elem = (XmlElement)node;
                int opcode = int.Parse(elem.GetAttribute("opcode"));
                int selects = int.Parse(elem.GetAttribute("selects"));
                string name = elem.GetAttribute("name");
                Type type = model.GetType("Data." + name);
                if(type == null)
                {
                    System.Console.WriteLine($"not find {name} type");
                    return;
                }
                ProtocolInfo info = new ProtocolInfo(EnumHelper.FromString<ProtocolCategory>(elem.GetAttribute("type")),
                                                     opcode,
                                                     EnumHelper.FromString<AppType>(elem.GetAttribute("to")),
                                                     EnumHelper.FromString<AppType>(elem.GetAttribute("from")),
                                                     //(selects & 1) > 0,
                                                     (selects & 2) > 0,
                                                     type);
                if(protocolInfoDic.ContainsKey(opcode))
                {
                    System.Console.WriteLine($"same protocol opcode {opcode}");
                }
                protocolInfoDic[opcode] = info;
                msgOpcodes[type] = info;
            }
        }

        public void Load(Assembly assembly, AppType appType)
        {
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {//将消息处理与消息码关联
                object[] attrs = type.GetCustomAttributes(typeof(ProtocolAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }
                ProtocolAttribute messageAttribute = (ProtocolAttribute)attrs[0];

                ProtocolInfo info = null;
                if (protocolInfoDic.TryGetValue(messageAttribute.opCode, out info))
                {
                    if(info.ToServer == appType || info.ToServer == AppType.All)
                    {
                        IProtocolHandle obj = Activator.CreateInstance(type) as IProtocolHandle;
                        info.SetHandleInfo(type, obj);
                    }
                }
                else
                {
                    System.Console.WriteLine($"not exist protocol info, opcode {messageAttribute.opCode} {type.Name}");
                }
            }
        }
        public ProtocolInfo GetProtocolInfo(int opcode)
        {
            ProtocolInfo info = null;
            protocolInfoDic.TryGetValue(opcode, out info);
            return info;
        }
        public ProtocolInfo GetProtocolInfo(Type msgType)
        {
            ProtocolInfo info = null;
            msgOpcodes.TryGetValue(msgType, out info);
            return info;
        }
        public T Take<T>(int opcode)
        {
            ProtocolInfo pi = null;
            object obj = null;
            if(protocolInfoDic.TryGetValue(opcode, out pi))
            {
                obj = pi.Take();
            }
            return (T)obj;
        }
        public void Back(object obj)
        {
            Type type = obj.GetType();
            ProtocolInfo pi = null;
            if (msgOpcodes.TryGetValue(type, out pi))
            {
                pi.Back(obj);
            }
        }
    }

}