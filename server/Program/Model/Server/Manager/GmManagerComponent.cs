using System;
using System.Collections.Generic;
using Base;
using System.Threading;
using System.Xml;
using SimpleLayout;
using System.Reflection;
using Data;

public class GmManagerComponent : Component, IAwake
{
    Dictionary<int, Page> pageDic = new Dictionary<int, Page>();
    Page curPage;

    public static GmManagerComponent Instance;
    public const int mainPageid = 1;
    public void Awake()
    {
        Instance = this;
        LoadLayout();
        GotoPage(mainPageid);
        Thread td1 = new Thread(Refresh);
        td1.Start();

        IntervalTask refreshTimeInterval = new IntervalTask(ConstConfigComponent.ConstConfig.ServerInfoTime, ServerInfoShow);
        TimeManagerComponent.Instance.Add(refreshTimeInterval);
    }
    public override void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }
        base.Dispose();
        Instance = null;
    }

    public void ServerInfoShow()
    {
        Log.Debug($"Manager人数 : {ServerAllotComponent.Instance.TotalPlayerNumber}");
    }

    void LoadLayout()
    {
        Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
        Assembly assembly = null;
        for (int i = 0; i < assemblys.Length; i++)
        {
            if(assemblys[i].GetName().Name == "Model")
            {
                assembly = assemblys[i];
                break;
            }
        }
        if (assembly == null)
            return;
        XmlDocument xmldoc = new XmlDocument();
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;
        XmlReader reader = XmlReader.Create("../../bglayout.xml", settings);
        xmldoc.Load(reader);

        XmlNodeList pages = xmldoc.SelectSingleNode("layout").SelectNodes("page");
        if (pages == null)
        {
            Console.WriteLine("page is null in bglayout.xml");
            return;
        }
        foreach(XmlNode pageNode in pages)
        {
            XmlElement elem = (XmlElement)pageNode;
            string str = elem.GetAttribute("type");
            Type pageType = assembly.GetType(str);
            if(pageType == null)
            {
                Console.WriteLine($"not exist page type {str}");
                continue;
            }
            Page page = Activator.CreateInstance(pageType) as Page;
            page.Analysis(elem, assembly);
            if(pageDic.ContainsKey(page.Id))
            {
                Console.WriteLine($"exist same page id {page.Id}.");
                continue;
            }
            pageDic.Add(page.Id, page);
        }
    }
    void Refresh()
    {
        while(true)
        {
            curPage.Show();
        }
    }
    public void GotoPage(int pageid)
    {
        Page page = null;
        if (pageDic.TryGetValue(pageid, out page))
        {
            curPage = page;
        }
        else
        {
            curPage = pageDic[mainPageid];
        }
    }
}
namespace SimpleLayout
{
    public class Widget
    {
        protected string des;
        protected string showContent;
        public string ShowContent { get { return showContent; } }
        protected int gotopage;
        public int Gotopage { get { return gotopage; } }

        public virtual void Analysis(XmlElement elem)
        {
            gotopage = int.Parse(elem.GetAttribute("gotopage"));
            des = elem.GetAttribute("des");
        }
        public virtual void Do(string input)
        {
            
        }
    }
    public class Option : Widget
    {
        protected int index;
        public int Index { get { return index; } }
        public override void Do(string input)
        {
            base.Do(input);
            GmManagerComponent.Instance.GotoPage(gotopage);
        }
        public override void Analysis(XmlElement elem)
        {
            base.Analysis(elem);
            index = int.Parse(elem.GetAttribute("index"));
            showContent = " " + index + ":" + des;
        }
    }
    public class GoToPageOption : Option
    {

    }
    public class HotfixOption : Option
    {

    }
    public class GlobalNotice : Option
    {

    }
    public class AddRoomCard : Option
    {

    }
    public class ClearOption : Option
    {
        public override void Do(string input)
        {
            Console.Clear();
            base.Do(input);
        }
    }
    public class ReloadDataConfig : Option
    {

    }
    public class ReloadConstConfig : Option
    {
        public override void Do(string input)
        {
            base.Do(input);
            OneThreadSynchronizationContext.Instance.Post((s) =>
            {
                M2A_ReloadConstConfigMessage msg = new M2A_ReloadConstConfigMessage();
                //NetInnerComponent.Instance.SendMsgToAllServer(msg);
                ConstConfigComponent.Instance.Load();
            }, null);
        }
    }
    public class RefreshActivity : Option
    {
        public override void Do(string input)
        {
            base.Do(input);
            OneThreadSynchronizationContext.Instance.Post((s) =>
            {
                //FuncDispatcher.Instance.Run(FunctionId.RefreshActivity, FuncDispatcher.Instance.Run<List<ActivityInfo>>(FunctionId.BuildActivity), true);
            }, null);
        }
    }
    public class CloseServer : Option
    {

    }
    public class Input : Widget
    {
        public override void Analysis(XmlElement elem)
        {
            base.Analysis(elem);
            showContent = des;
        }
    }
    public class HotfixInput : Input
    {
        public override void Do(string input)
        {
            base.Do(input);
            int modules = 0;
            if(int.TryParse(input, out modules))
            {
                if(modules == 0)
                {
                    GmManagerComponent.Instance.GotoPage(GmManagerComponent.mainPageid);
                }
                else
                {
                    OneThreadSynchronizationContext.Instance.Post((s) =>
                    {
                        M2A_HotfixCodeMessage msg = new M2A_HotfixCodeMessage();
                        msg.module = modules;
                        //NetInnerComponent.Instance.SendMsgToAllServer(msg);
                        FuncDispatcher.Instance.Run((int)FunctionId.LoadHotfixModule, msg.module);
                    }, null);
                    GmManagerComponent.Instance.GotoPage(gotopage);
                }
            }
            else
            {
                Console.WriteLine("input error.");
                //BgManagerComponent.Instance.GotoPage(BgManagerComponent.mainPageid);
            }
        }
        public override void Analysis(XmlElement elem)
        {
            base.Analysis(elem);
            showContent = "0:退出 ";
            foreach (HotfixModule val in Enum.GetValues(typeof(HotfixModule)))
            {
                showContent += (int)val + ":" + val + " ";
            }
            showContent += ", can add.";
        }
    }
    public class ConfigInput : Input
    {
        public override void Do(string input)
        {
            base.Do(input);
            
            if (!string.IsNullOrEmpty(input))
            {
                int modules = 0;
                if (int.TryParse(input, out modules) && modules == 0)
                {
                    GmManagerComponent.Instance.GotoPage(GmManagerComponent.mainPageid);
                }
                else
                {
                    string[] strs = input.Split(' ');
                    List<int> configTypes = new List<int>();
                    ConfigType configType;
                    for(int i = 0; i < strs.Length; i++)
                    {
                        if(Enum.TryParse(strs[i], out configType))
                        {
                            configTypes.Add((int)configType);
                        }
                        else
                        {
                            string log = $"{strs[i]} config name error.";
                            Log.Debug(log);
                            Console.WriteLine(log);
                        }
                    }
                    if(configTypes.Count > 0)
                    {
                        OneThreadSynchronizationContext.Instance.Post((s) =>
                        {
                            M2A_ReloadDataConfigMessage msg = new M2A_ReloadDataConfigMessage();
                            msg.configStrs = configTypes;
                            //NetInnerComponent.Instance.SendMsgToAllServer(msg);

                            FuncDispatcher.Instance.Run((int)FunctionId.LoadDataConfig, configTypes);
                        }, null);
                        GmManagerComponent.Instance.GotoPage(gotopage);
                    }
                }
            }
            else
            {
                Console.WriteLine("input error.");
            }
        }
        public override void Analysis(XmlElement elem)
        {
            base.Analysis(elem);
            showContent = "输入配置表名称或者0（退出），多个配置表名称可以以空格隔开.";
        }
    }
    public class GlobalNoticeInput : Input
    {
        public override void Do(string input)
        {
            base.Do(input);

            if (!string.IsNullOrEmpty(input))
            {
                OneThreadSynchronizationContext.Instance.Post((s) =>
                {
                    //SS_GlobalNoticeMessage msgToGate = new SS_GlobalNoticeMessage();
                    //msgToGate.content = input;
                    //NetInnerComponent.Instance.SendMsgToSevers(msgToGate, AppType.GateServer);
                }, null);
                GmManagerComponent.Instance.GotoPage(gotopage);
            }
            else
            {
                GmManagerComponent.Instance.GotoPage(gotopage);
            }
        }
        public override void Analysis(XmlElement elem)
        {
            base.Analysis(elem);
            showContent = "请输入全局公会内容:";
        }
    }
    public class AddRoomInput : Input
    {
        public override void Do(string input)
        {
            base.Do(input);

            if (!string.IsNullOrEmpty(input))
            {
                string[] strs = input.Split(' ');
                if(strs.Length < 2)
                {
                    Console.WriteLine("信息不全,重新输入.");
                    return;
                }
                long playerid = 0;
                if(!long.TryParse(strs[0], out playerid))
                {
                    Console.WriteLine("id输入错误,重新输入.");
                    return;
                }
                int count = 0;
                if (!int.TryParse(strs[1], out count))
                {
                    Console.WriteLine("数量输入错误,重新输入.");
                    return;
                }
                OneThreadSynchronizationContext.Instance.Post((s) =>
                {
                    AddCard(playerid, count);
                }, null);
            }
            else
            {
                GmManagerComponent.Instance.GotoPage(gotopage);
            }
        }
        public override void Analysis(XmlElement elem)
        {
            base.Analysis(elem);
            showContent = "请输入玩家id和添加数量（id count）:";
        }
        void AddCard(long playerid, int count)
        {
            PlayerAllotInfo info = ServerAllotComponent.Instance.Find(playerid);
            if (info != null)
            {
                //SS_GmCommonOpRequest reqToGate = new SS_GmCommonOpRequest();
                //reqToGate.id = playerid;
                //reqToGate.op = 1;//添加
                //reqToGate.param1 = 1;//房卡
                //reqToGate.param2 = count * 100;//数量
                //Session gateSession = NetInnerComponent.Instance.GetByAppID(info.gateid);
                //SS_GmCommonOpResponse respFromGate = await gateSession.Call<SS_GmCommonOpRequest, SS_GmCommonOpResponse>(reqToGate);
                //if (respFromGate.errorCode != (int)ErrorCode.Success)
                //{//
                //    Console.WriteLine("添加房卡失败,从gateserver");
                //}
            }
            else
            {
                //M2D_GmCommonOpRequest reqToDB = new M2D_GmCommonOpRequest();
                //reqToDB.id = playerid;
                //reqToDB.op = 1;//添加
                //reqToDB.param1 = 1;//房卡
                //reqToDB.param2 = count * 100;//数量
                //Session dbSession = NetInnerComponent.Instance.GetByAppID(ServerConfigComponent.Instance.DBAppId);
                //D2M_GmCommonOpResponse respFromDB = await dbSession.Call<M2D_GmCommonOpRequest, D2M_GmCommonOpResponse>(reqToDB);
                //if (respFromDB.errorCode != (int)ErrorCode.Success)
                //{//
                //    Console.WriteLine("添加房卡失败,从dbserver");
                //}
            }
        }
    }
    public class CloseServerInput : Input
    {
        public override void Do(string input)
        {
            base.Do(input);

            if (!string.IsNullOrEmpty(input))
            {
                int modules = 0;
                if (int.TryParse(input, out modules) && modules == 0)
                {
                    GmManagerComponent.Instance.GotoPage(GmManagerComponent.mainPageid);
                }
                else
                {
                    OneThreadSynchronizationContext.Instance.Post((s) =>
                    {
                        //M2A_CloseServerMessage msg = new M2A_CloseServerMessage();
                        //NetInnerComponent.Instance.SendMsgToAllServer(msg);
                    }, null);
                }
            }
            else
            {
                Console.WriteLine("input error.");
            }
        }
        public override void Analysis(XmlElement elem)
        {
            base.Analysis(elem);
            showContent = "输入关闭服务器id或者0（退出），多个id可以以空格隔开.";
        }
    }
    public class Page
    {
        protected int id;

        public int Id { get { return id; } }

        public virtual void Show()
        {

        }
        public virtual void Input(string input)
        {

        }
        public virtual void Analysis(XmlElement elem, Assembly assembly)
        {
            id = int.Parse(elem.GetAttribute("id"));
        }
    }
    public class OptionPage : Page
    {
        public List<Option> options = new List<Option>();

        public override void Show()
        {
            base.Show();
            string showContent = "";
            for(int i = 0; i < options.Count; i++)
            {
                showContent += options[i].ShowContent;
            }
            Console.WriteLine(showContent);
            while(true && options.Count > 0)
            {
                Console.WriteLine("please select : ");
                string str = Console.ReadLine();
                int index = 0;
                if (int.TryParse(str, out index))
                {
                    Option select = options.Find(x => x.Index == index);
                    if(select != null)
                    {
                        select.Do(null);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("not exist select option.");
                    }
                }
                else
                {
                    Console.WriteLine("input error.");
                }
            }
        }
        public override void Input(string input)
        {
            base.Input(input);
        }
        public override void Analysis(XmlElement elem, Assembly assembly)
        {
            base.Analysis(elem, assembly);
            XmlNodeList widgetNodes = elem.SelectNodes("widget");
            if (widgetNodes == null)
            {
                Console.WriteLine($"page({Id}) not exist widget.");
                return;
            }
            foreach (XmlNode widgetNode in widgetNodes)
            {
                XmlElement widgetElem = (XmlElement)widgetNode;
                string str = widgetElem.GetAttribute("type");
                Type widgetType = assembly.GetType(str);
                if (widgetType == null)
                {
                    Console.WriteLine($"not exist widget type {str}");
                    continue;
                }
                Option widget = Activator.CreateInstance(widgetType) as Option;
                widget.Analysis(widgetElem);
                options.Add(widget);
            }
        }
    }
    public class InputPage : Page
    {
        Input input;
        public override void Show()
        {
            base.Show();
            Console.WriteLine(input.ShowContent);
            Console.WriteLine("please input : ");
            string str = Console.ReadLine();
            input.Do(str);           
        }
        public override void Input(string input)
        {
            base.Input(input);
        }
        public override void Analysis(XmlElement elem, Assembly assembly)
        {
            base.Analysis(elem, assembly);
            XmlNodeList widgetNodes = elem.SelectNodes("widget");
            if (widgetNodes == null)
            {
                Console.WriteLine($"page({Id}) not exist widget.");
                return;
            }
            if(widgetNodes.Count > 1)
            {
                Console.WriteLine($"InputPage({Id}) should one widget.");
            }
            XmlNode widgetNode = widgetNodes[0];
            XmlElement widgetElem = (XmlElement)widgetNode;
            string str = widgetElem.GetAttribute("type");
            Type widgetType = assembly.GetType(str);
            if (widgetType == null)
            {
                Console.WriteLine($"not exist widget type {str}");
                return;
            }
            Input widget = Activator.CreateInstance(widgetType) as Input;
            widget.Analysis(widgetElem);
            input = widget;
        }
    }
}
