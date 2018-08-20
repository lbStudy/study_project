
//using ILRuntime.CLR.TypeSystem;
using Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Linq;
//using ILRuntime.Runtime.Intepreter;

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
        private ProtocolCategory protocolCategory;
        private int opcode;
        private bool isEncrypt;
        private Type protocolBodyType;
        private IProtocolHandle handleInterface;

        public IProtocolHandle HandleInterface { get { return handleInterface; } }
        public ProtocolCategory ProtocolCategory { get { return protocolCategory; }}
        public int Opcode { get { return opcode; } }
        public bool IsEncrypt { get { return isEncrypt; } }
        public Type ProtocolBodyType { get { return protocolBodyType; } }
        private Queue<object> cacheBodys = new Queue<object>();
        public int takeCount;
        public int backCount;
        public ProtocolInfo(ProtocolCategory protocolType, int opcode, bool isEncrypt, Type protocolBodyType)
        {
            this.protocolCategory = protocolType;
            this.opcode = opcode;
            this.isEncrypt = isEncrypt;
            this.protocolBodyType = protocolBodyType;
        }
        public void SetHandleInfo(IProtocolHandle handleInterface)
        {
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
                if (cacheBodys.Count < 2)
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
        public void LoadProtocol(System.IO.Stream stream, Assembly model)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
           
            XmlReader reader = XmlReader.Create(stream, settings);
            xmldoc.Load(reader);

            XmlNode root = xmldoc.SelectSingleNode("protocols");
            if (root == null)
            {
                UnityEngine.Debug.LogError("load protocol error");
                return;
            }
            XmlNodeList nodeList = root.SelectNodes("msg");
            if(nodeList == null)
            {
                return;
            }
            protocolInfoDic.Clear();
            msgOpcodes.Clear();
            foreach (XmlNode node in nodeList)
            {
                XmlElement elem = (XmlElement)node;
                int opcode = int.Parse(elem.GetAttribute("opcode"));
                int selects = int.Parse(elem.GetAttribute("selects"));
                string name = elem.GetAttribute("name");
                Type type = model.GetType("Data." + name);
                if(type == null)
                {
                    UnityEngine.Debug.LogError($"not find {name} type");
                    return;
                }
                ProtocolCategory protocalType = ProtocolCategory.Message;
                switch (elem.GetAttribute("type"))
                {
                    case "Request":
                        protocalType = ProtocolCategory.Request;
                        break;
                    case "Response":
                        protocalType = ProtocolCategory.Response;
                        break;
                    default:
                        break;
                }

    
                ProtocolInfo info = new ProtocolInfo(protocalType,
                                                     opcode,
                                                     (selects & 2) > 0,
                                                     type);
                if(protocolInfoDic.ContainsKey(opcode))
                {
                    UnityEngine.Debug.LogError($"same protocol opcode {opcode}");
                }
                protocolInfoDic[opcode] = info;
                msgOpcodes[type] = info;
                
            }
        }
        public void LoadProtocol(System.IO.Stream stream, List<Type> allTypes)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;

            XmlReader reader = XmlReader.Create(stream, settings);
            xmldoc.Load(reader);

            XmlNode root = xmldoc.SelectSingleNode("protocols");
            if (root == null)
            {
                UnityEngine.Debug.LogError("load protocol error");
                return;
            }
            XmlNodeList nodeList = root.SelectNodes("msg");
            if (nodeList == null)
            {
                return;
            }
            protocolInfoDic.Clear();
            msgOpcodes.Clear();
            foreach (XmlNode node in nodeList)
            {
                XmlElement elem = (XmlElement)node;
                int opcode = int.Parse(elem.GetAttribute("opcode"));
                int selects = int.Parse(elem.GetAttribute("selects"));
                string name = elem.GetAttribute("name");
                Type type = allTypes.Find(x => x.FullName == "Data." + name);
                if (type == null)
                {
                    UnityEngine.Debug.LogError($"not find {name} type");
                    return;
                }
                ProtocolCategory protocalType = ProtocolCategory.Message;
                switch (elem.GetAttribute("type"))
                {
                    case "Request":
                        protocalType = ProtocolCategory.Request;
                        break;
                    case "Response":
                        protocalType = ProtocolCategory.Response;
                        break;
                    default:
                        break;
                }


                ProtocolInfo info = new ProtocolInfo(protocalType,
                                                     opcode,
                                                     (selects & 2) > 0,
                                                     type);
                if (protocolInfoDic.ContainsKey(opcode))
                {
                    UnityEngine.Debug.LogError($"same protocol opcode {opcode}");
                }
                protocolInfoDic[opcode] = info;
                msgOpcodes[type] = info;
            }
        }
        public void Load(Assembly assembly)
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
                    IProtocolHandle obj = Activator.CreateInstance(type) as IProtocolHandle;
                    info.SetHandleInfo(obj);
                }
                else
                {
                    UnityEngine.Debug.LogError($"not exist protocol info, opcode {messageAttribute.opCode}");
                }
            }
        }

        public void Load(List<Type> allTypes)
        {
            foreach (Type type in allTypes)
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
                    IProtocolHandle obj = Activator.CreateInstance(type) as IProtocolHandle;
                    info.SetHandleInfo(obj);
                }
                else
                {
                    UnityEngine.Debug.LogError($"not exist protocol info, opcode {messageAttribute.opCode}");
                }
            }
        }

        public ProtocolInfo GetProtocolInfo(int opcode)
        {
            ProtocolInfo info = null;
            protocolInfoDic.TryGetValue(opcode, out info);
            return info;
        }
        public ProtocolInfo GetProtocolInfo(Type type)
        {
           
            ProtocolInfo info = null;
            msgOpcodes.TryGetValue(type, out info);
            return info;
        }

        public object Take(int opcode)
        {
            ProtocolInfo pi = null;
            object obj = null;
            if (protocolInfoDic.TryGetValue(opcode, out pi))
            {
                obj = pi.Take();
            }
            return obj;
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