using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Reflection;
using System.Xml;

namespace ProtocolTool
{
    public class BaseInfo
    {
        private string Opcode;
        private string Name;

        public BaseInfo(string opcode, string name)
        {
            Opcode = opcode;
            Name = name;
        }
        public string opcode { get{ return Opcode; }set{ Opcode = value; } }
        public string name { get { return Name; } set { Name = value; } }
    }
    public class ProtocolInfo
    {
        public string handlePath;
        public string simpleName;
        public string nameSpace;

    }
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, ProtocolInfo> protocolInfos = new Dictionary<string, ProtocolInfo>();
        XmlDocument serverProtocolXml;
        XmlNode protocolRoot;
        Assembly model;
        XmlNode configNode;
        const string msgNodeName = "msg";
        const int opCodeoffset = 100000;
        const string requsetStr = "Request";
        const string responseStr = "Response";
        const string messageStr = "Message";

        const string noneStr = "none";

        const string toStr = "to";
        const string fromStr = "from";

        const string clientStr = "Client";
        const string allStr = "all";

        string client_bodyPath = null;
        string server_bodyPath = null;
        string msgEnumPath = null;
        string clientEnumPath = null;
        public MainWindow()
        {
            InitializeComponent();
            LoadConfigXML();
            LoadProtocolXML();
            LoadModleAssembly();
            Init();
        }
        private void LoadModleAssembly()
        {
            XmlElement node = (XmlElement)configNode.SelectSingleNode("modelDllPath");
            string path = node.GetAttribute("path");
            if (File.Exists(path))
            {
                byte[] dllBytes = File.ReadAllBytes(path);
                if (dllBytes != null)
                    model = Assembly.Load(dllBytes);
            }
        }
        private void LoadProtocolXML()
        {
            serverProtocolXml = new XmlDocument();
            XmlElement elem = (XmlElement)configNode.SelectSingleNode("msgPath");
            if(elem == null)
            {
                MessageBox.Show("not exsit msgPath in Config.xml");
                Close();
                return;
            }
            if (File.Exists(elem.GetAttribute("path")))
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                XmlReader reader = XmlReader.Create(elem.GetAttribute("path"), settings);
                serverProtocolXml.Load(reader);
                reader.Close();
            }
            protocolRoot = serverProtocolXml.SelectSingleNode("protocols");
            if(protocolRoot == null)
            {
                XmlNode node = serverProtocolXml.CreateXmlDeclaration("1.0", "utf-8", "");
                serverProtocolXml.AppendChild(node);
                protocolRoot = serverProtocolXml.AppendChild(serverProtocolXml.CreateElement("", "protocols", ""));
                Save();
            }
            else
            {
                XmlNodeList nodes = protocolRoot.SelectNodes(msgNodeName);
                List<XmlElement> newNodes = new List<XmlElement>();
                foreach(XmlNode val in nodes)
                {
                    newNodes.Add((XmlElement)val);
                }
                newNodes.Sort((x, y) => { return x.GetAttribute("name").CompareTo(y.GetAttribute("name")); });
                protocolRoot.RemoveAll();
                for(int i = 0; i < newNodes.Count; i++)
                {
                    protocolRoot.AppendChild(newNodes[i]);
                }
                Save();
            }
        }
        private void LoadConfigXML()
        {
            XmlDocument configXml = new XmlDocument();
            if (!File.Exists("Config.xml"))
            {
                MessageBox.Show("not exsit Config.xml");
                Close();
                return;
            }
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                XmlReader reader = XmlReader.Create("Config.xml", settings);
                configXml.Load(reader);
                configNode = configXml.SelectSingleNode("config");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                Close();
                return;
            }
        }
        private void Init()
        {
            XmlElement elem = (XmlElement)configNode.SelectSingleNode("server_protocol_body_path");
            server_bodyPath = elem.GetAttribute("path");
            if (!Directory.Exists(server_bodyPath))
                Directory.CreateDirectory(server_bodyPath);

            elem = (XmlElement)configNode.SelectSingleNode("client_protocol_body_path");
            client_bodyPath = elem.GetAttribute("path");
            if (!Directory.Exists(client_bodyPath))
                Directory.CreateDirectory(client_bodyPath);

            elem = (XmlElement)configNode.SelectSingleNode("msgEnumPath");
            msgEnumPath = elem.GetAttribute("path");

            elem = (XmlElement)configNode.SelectSingleNode("clientEnumPath");
            clientEnumPath = elem.GetAttribute("path");

            XmlNodeList nodes = configNode.SelectNodes("app");
            for (int i = 0; i < nodes.Count; i++)
            {
                elem = (XmlElement)nodes[i];
                string key = elem.GetAttribute("name");
                ProtocolInfo info = new ProtocolInfo();
                info.simpleName = elem.GetAttribute("simpleName");
                info.nameSpace = elem.GetAttribute("namespace");
                info.handlePath = elem.GetAttribute("handlePath");
                protocolInfos[key] = info;
                AddSelect(key);
            }
            nodes = protocolRoot.SelectNodes(msgNodeName);
            AddGroupSelect(allStr);
            for (int i = 0; i < nodes.Count; i++)
            {
                elem = (XmlElement)nodes[i];
                string groupName = elem.GetAttribute("group");
                if (IsExistGroup(groupName))
                    continue;
                AddGroupSelect(groupName);
            }
            ResetList();

            label3.Visibility = Visibility.Hidden;
            opCode.Visibility = Visibility.Hidden;
        }
        private List<int> GetUseOpcodes(string path)
        {
            List<int> opcodes = new List<int>();
            if(Directory.Exists(path))
            {
                DirectoryInfo folder = new DirectoryInfo(path);
                foreach (FileInfo file in folder.GetFiles("*.cs"))
                {
                    Console.WriteLine(file.FullName);
                    string content = File.ReadAllText(file.FullName);
                    string str = GetValue(content, "[Protocol(", ")]");
                    opcodes.Add(int.Parse(str));
                }
            }
            else
            {
                Directory.CreateDirectory(path);
            }
            return opcodes;
        }
        /// <summary>
        /// 获得字符串中开始和结束字符串中间得值
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="s">开始</param>
        /// <param name="e">结束</param>
        /// <returns></returns> 
        public string GetValue(string str, string s, string e)
        {
            int start = str.IndexOf(s);
            int first = str.IndexOf('(', start);
            int end = str.IndexOf(e, start);
            string subStr = str.Substring(first + 1, end - first - 1);
            return subStr.Trim();
        }
        public void AddSelect(string key)
        {
            bool isExist = false;
            for (int j = 0; j < fromComboBox.Items.Count; j++)
            {
                ComboBoxItem item = (ComboBoxItem)fromComboBox.Items[j];
                if (item.Content.ToString() == key)
                {
                    isExist = true;
                    break;
                }
            }
            if (!isExist)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = key;
                fromComboBox.Items.Add(item);
            }
            isExist = false;
            for (int j = 0; j < toComboBox.Items.Count; j++)
            {
                ComboBoxItem item = (ComboBoxItem)toComboBox.Items[j];
                if (item.Content.ToString() == key)
                {
                    isExist = true;
                    break;
                }
            }
            if (!isExist)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = key;
                toComboBox.Items.Add(item);
            }
        }

        public bool IsExistGroup(string groupName)
        {
            foreach (ComboBoxItem val in Groups.Items)
            {
                if (((string)val.Content).ToLower() == groupName.ToLower())
                {
                    return true;
                }
            }
            return false;
        }
        public void ModifyGroup(string oldStr, string newStr)
        {
            foreach (ComboBoxItem val in Groups.Items)
            {
                if (((string)val.Content).ToLower() == oldStr.ToLower())
                {
                    val.Content = newStr;
                    break;
                }
            }
            foreach (ComboBoxItem val in SelectGroup.Items)
            {
                if (((string)val.Content).ToLower() == oldStr.ToLower())
                {
                    val.Content = newStr;
                    break;
                }
            }
        }
        public void RemoveGroup(string groupName)
        {
            foreach (ComboBoxItem val in Groups.Items)
            {
                if ((string)val.Content == groupName)
                {
                    Groups.Items.Remove(val);
                    break;
                }
            }
            foreach (ComboBoxItem val in SelectGroup.Items)
            {
                if ((string)val.Content == groupName)
                {
                    SelectGroup.Items.Remove(val);
                    break;
                }
            }
        }
        public void AddGroupSelect(string groupName)
        {
            ComboBoxItem item = new ComboBoxItem();
            item.Content = groupName;
            Groups.Items.Add(item);
            if(groupName != allStr)
            {
                ComboBoxItem item2 = new ComboBoxItem();
                item2.Content = groupName;
                SelectGroup.Items.Add(item2);
            }
        }
        private void name_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox name = (TextBox)sender;
        }
        private bool IsFillCorrect()
        {
            ComboBoxItem item = (ComboBoxItem)fromComboBox.SelectedItem;
            if (item == null || string.IsNullOrEmpty((string)item.Content))
            {
                MessageBox.Show("please select from");
                return false;
            }
            
            item = (ComboBoxItem)toComboBox.SelectedItem;
            if (item == null || string.IsNullOrEmpty((string)item.Content))
            {
                MessageBox.Show("please select to");
                return false;
            }
            string to = (string)item.Content;

            item = (ComboBoxItem)typeComboBox.SelectedItem;
            if (item == null || string.IsNullOrEmpty((string)item.Content))
            {
                MessageBox.Show("please select type.");
                return false;
            }
            
            ProtocolInfo info = protocolInfos[to];
            if (info.handlePath == null)
            {
                MessageBox.Show("please config path : " + (string)item.Content + "_Protocol_Handle_Path");
                return false;
            }
            string protocolName = name.Text.Trim();
            if (string.IsNullOrEmpty(protocolName))
            {
                MessageBox.Show("please write name.");
                return false;
            }

            return true;
        }
        private string GetProtocolBodyPath(string from, string to)
        {
            return from == clientStr || to == clientStr ? client_bodyPath : server_bodyPath;
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            if(!IsFillCorrect())
            {
                return;
            }
            ComboBoxItem item = (ComboBoxItem)fromComboBox.SelectedItem;
            string from = (string)item.Content;
            item = (ComboBoxItem)toComboBox.SelectedItem;
            string to = (string)item.Content;
            item = (ComboBoxItem)typeComboBox.SelectedItem;
            if(to == from && to != "All")
            {
                MessageBox.Show("to can not same with from.");
                return;
            }
            
            string type = (string)item.Content;
            if (type == responseStr)
            {
                MessageBox.Show("can not create Response by youself. it auto create when create Resquest.");
                return;
            }

            if(type == requsetStr && to == clientStr)
            {
                MessageBox.Show("can not send Resquest to Client. can use massage.");
                return;
            }

            if (SelectGroup.SelectedItem == null)
            {
                MessageBox.Show("please select group.");
                return;
            }

            string bodyPath = GetProtocolBodyPath(from, to);
            string protocolName = name.Text.Trim();
            if(type == messageStr)
            {
                ProtocolInfo info = protocolInfos[to];
                string fullName = protocolInfos[from].simpleName + "2" + protocolInfos[to].simpleName + "_" + protocolName + type;
                string fullPath = bodyPath + fullName + ".cs";
                int newOpcode = GetNewOpcode();
                if (newOpcode <= 0)
                    return;
                if (!Create(fullName, from, to, type, fullPath, newOpcode, info))
                    return;
            }
            if (type == requsetStr)
            {
                ProtocolInfo info = protocolInfos[to];
                string fullName = protocolInfos[from].simpleName + "2" + protocolInfos[to].simpleName + "_" + protocolName + type;
                string fullName2 = protocolInfos[to].simpleName + "2" + protocolInfos[from].simpleName + "_" + protocolName + responseStr;
                string fullPath = bodyPath + fullName + ".cs";
                int newOpcode = GetNewOpcode();
                if (newOpcode <= 0)
                    return;
                if (!Create(fullName, from, to, type, fullPath, newOpcode, info, fullName2))
                    return;
                info = protocolInfos[from];
                fullPath = bodyPath + fullName2 + ".cs";
                if(!Create(fullName2, to, from, responseStr, fullPath, newOpcode + opCodeoffset, info))
                {
                    MessageBox.Show("create response fail.");
                    return;
                }
            }

            MessageBox.Show("create successful");
        }
        private bool Create(string fullName, string from, string to, string type, string fullPath, int newOpcode, ProtocolInfo info, string fullName2 = null)
        {
            if (IsExistSameProtocolName(fullName))
            {
                MessageBox.Show($"exist same protocol name. {fullName}");
                return false;
            }

            int selects = 0;
            if(IsContentClient())
            {
                //if ((bool)compress.IsChecked)
                //    selects |= 1;
                if ((bool)encrypt.IsChecked)
                    selects |= 2;
            }

            string groupName = noneStr;
            if(SelectGroup.SelectedItem != null)
            {
                groupName = (string)((ComboBoxItem)SelectGroup.SelectedItem).Content;
            }

            WriteProtocolXML(fullName, newOpcode, from, to, type, selects, groupName);
            ResetList();

            if (!File.Exists(fullPath))
                WriteProtocolBody(fullName, fullPath, newOpcode, type);

            if (!string.IsNullOrEmpty(info.handlePath) && !string.IsNullOrEmpty(info.nameSpace) && type != responseStr)
            {
                string fullName3 = fullName + "Handler";
                fullPath = info.handlePath + fullName3 + ".cs";
                if (!File.Exists(fullPath))
                {
                    if (type == messageStr)
                        WriteMessageHandle(fullName3, fullPath, newOpcode, fullName, info.nameSpace);
                    else if (type == requsetStr)
                        WriteRequestHandle(fullName3, fullPath, newOpcode, fullName, info.nameSpace, fullName2);
                }
            }
            CreateEnum();
            return true;
        }
        private void WriteProtocolXML(string name, int opcode, string from, string to, string type, int selects, string groupName)
        {
            XmlElement msg = serverProtocolXml.CreateElement(msgNodeName);//创建一个<msg>节点 
            msg.SetAttribute("name", name); 
            msg.SetAttribute("opcode", opcode.ToString());
            msg.SetAttribute(fromStr, from);
            msg.SetAttribute(toStr, to);
            msg.SetAttribute("type", type);
            msg.SetAttribute("selects", selects.ToString());
            msg.SetAttribute("group", groupName);

            InsertProtocolNode(msg);
        }
        private void Save()
        {
            XmlElement elem = (XmlElement)configNode.SelectSingleNode("msgPath");
            serverProtocolXml.Save(elem.GetAttribute("path"));
        }
        private void InsertProtocolNode(XmlElement insertElem)
        {
            string insertName = insertElem.GetAttribute("name");
            bool isInsertFail = true;
            foreach (XmlNode node in protocolRoot.ChildNodes)
            {
                XmlElement elem = (XmlElement)node;
                string name = elem.GetAttribute("name");
                if(insertName.CompareTo(name) <= 0)
                {
                    isInsertFail = false;
                    protocolRoot.InsertBefore(insertElem, elem);
                    break;
                }
            }
            if(isInsertFail)
            {
                protocolRoot.AppendChild(insertElem);
            }
            Save();
        }
        private void DeleteProtocolNode(XmlNode node)
        {
            protocolRoot.RemoveChild(node);
            ResetList();
            Save();
            CreateEnum();
        }
        private string GetVariableContent(int opcode)
        {
            string content = null;
            XmlNode node = FindProtocolNodeByOpcode(opcode);
            if (node == null)
                return content;
            XmlNodeList items = node.SelectNodes("item");
            foreach (XmlNode item in items)
            {
                content += "\t\t" + ((XmlElement)item).GetAttribute("type") + " " + ((XmlElement)item).GetAttribute("name") + ";\n";
            }
            return content;
        }
        private void WriteProtocolBody(string fullName, string fullPath, int newOpcode, string type)
        {
            try
            {
                StreamReader temple = new StreamReader("./ProtocolBodyTemple.txt");
                string s;
                FileStream fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);

                while ((s = temple.ReadLine()) != null)
                {
                    s = s.Replace("#opCode", newOpcode.ToString());
                    s = s.Replace("#ProtocolBodyName", fullName);
                    
                    if(responseStr == type)
                    {
                        s = s.Replace("#property", "[ProtoBuf.ProtoMember(1)]\npublic int errorCode { get; set; }");
                        s = s.Replace("#clear", "errorCode = 0;");
                    }
                    else
                    {
                        s = s.Replace("#property", "");
                        s = s.Replace("#clear", "");
                    }
                    //s = s.Replace("#ProtocolType", type);
                    sw.WriteLine(s);
                }
                sw.Flush();
                sw.Close();
                temple.Close();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void WriteMessageHandle(string fullName, string fullPath, int newOpcode, string protocolBodyName, string nameSpace)
        {
            try
            {
                StreamReader temple = new StreamReader("./MessageHandleTemple.txt");
                string s;
                FileStream fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);

                while ((s = temple.ReadLine()) != null)
                {
                    s = s.Replace("#nameSpace", nameSpace);
                    s = s.Replace("#opCode", newOpcode.ToString());
                    s = s.Replace("#HandleClassName", fullName);
                    s = s.Replace("#ProtocolBodyName", protocolBodyName);
                    sw.WriteLine(s);
                }
                sw.Flush();
                sw.Close();
                temple.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void WriteRequestHandle(string fullName, string fullPath, int newOpcode, string protocolBodyName, string nameSpace, string responseName)
        {
            try
            {
                StreamReader temple = new StreamReader("./RequestHandleTemple.txt");
                string s;
                FileStream fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);

                while ((s = temple.ReadLine()) != null)
                {
                    s = s.Replace("#nameSpace", nameSpace);
                    s = s.Replace("#opCode", newOpcode.ToString());
                    s = s.Replace("#HandleClassName", fullName);
                    s = s.Replace("#ProtocolBodyName", protocolBodyName);
                    s = s.Replace("#ResponseName", responseName);
                    sw.WriteLine(s);
                }
                sw.Flush();
                sw.Close();
                temple.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void opCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox opCode = (TextBox)sender;
        }
        private void fromComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectShow(IsContentClient());
        }
        private void toComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectShow(IsContentClient());
        }
        private void SelectShow(bool isShow)
        {
            //compress.Visibility = isShow ? Visibility.Visible : Visibility.Hidden;
            encrypt.Visibility = isShow ? Visibility.Visible : Visibility.Hidden;
        }
        private bool IsContentClient()
        {
            ComboBoxItem item = (ComboBoxItem)fromComboBox.SelectedItem;
            string from = item != null ? (string)item.Content : "";
            item = (ComboBoxItem)toComboBox.SelectedItem;
            string to = item != null ? (string)item.Content : "";
            if (from == clientStr || to == clientStr)
            {
                return true;
            }
            return false;
        }
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void itemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Type vr = itemList.SelectedItem.GetType();
            //PropertyInfo variableName = vr.GetProperty("variableName");
            //string name = (string)variableName.GetValue(itemList.SelectedItem);
            //PropertyInfo variableType = vr.GetProperty("variableType");
            //string type = (string)variableType.GetValue(itemList.SelectedItem);
            //MessageBox.Show($"{name} {type}");
        }
        private bool IsExistSameVariableName(string str)
        {
            for (int i = 0; i < itemList.Items.Count; i++)
            {
                Type vr = itemList.Items[i].GetType();
                PropertyInfo variableName = vr.GetProperty("variableName");
                string name = (string)variableName.GetValue(itemList.SelectedItem);
                if (str == name)
                    return true;
            }
            return false;
        }
        private bool IsExistSameProtocolName(string str)
        {
            XmlNodeList nodes = protocolRoot.SelectNodes(msgNodeName);
            if (nodes == null)
                return false;
            foreach(XmlNode node in nodes)
            {
                if (((XmlElement)node).GetAttribute("name") == str)
                    return true;
            }
            return false;
        }
        private int GetNewOpcode()
        {
            //string str = opCode.Text.Trim();
            int val = 0;
            //if(!string.IsNullOrEmpty(str))
            //{
            //    try
            //    {
            //        val = int.Parse(str);
            //    }
            //    catch
            //    {
            //        MessageBox.Show("opCode error");
            //        return 0;
            //    }
            //}
            XmlNodeList nodes = protocolRoot.SelectNodes(msgNodeName);
            if (val > 0)
            {
                if(val >= opCodeoffset)
                {
                    MessageBox.Show($"opCode < {opCodeoffset}");
                    return 0;
                }
                if (nodes != null)
                {
                    foreach (XmlNode node in nodes)
                    {
                        XmlElement elem = (XmlElement)node;
                        if (int.Parse(elem.GetAttribute("opcode")) == val)
                        {
                            MessageBox.Show($"{val} opcode same with {elem.GetAttribute("name")}.");
                            return 0;
                        }
                    }
                }
            }
            else
            {
                if (nodes == null)
                    val = 1;
                else
                {
                    Random random = new Random(DateTime.Now.Second);
                    while(true)
                    {
                        val = random.Next(opCodeoffset);
                        if(val > 0)
                        {
                            bool isExist = false;
                            foreach (XmlNode node in nodes)
                            {
                                XmlElement elem = (XmlElement)node;
                                if (int.Parse(elem.GetAttribute("opcode")) == val)
                                {
                                    isExist = true;
                                    break;
                                }
                            }
                            if (!isExist)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return val;
        }
        private void findContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            ResetList();
        }
        private void ResetList()
        {
            protocolView.Items.Clear();
            XmlNodeList nodes = protocolRoot.SelectNodes(msgNodeName);
            if (nodes == null)
            {
                return;
            }
            string showGroup = Groups.SelectedItem == null ? allStr : (string)((ComboBoxItem)Groups.SelectedItem).Content;
            string str = findContent.Text.Trim();
            bool isNull = string.IsNullOrEmpty(str);
            foreach (XmlNode node in nodes)
            {
                string opcode = ((XmlElement)node).GetAttribute("opcode");
                string name = ((XmlElement)node).GetAttribute("name");
                string group = ((XmlElement)node).GetAttribute("group");

                if(!string.IsNullOrEmpty(showGroup) && showGroup != group && showGroup != allStr)
                {
                    continue;
                }

                int val = 0;
                if(!isNull)
                {
                    if (int.TryParse(str, out val))
                    {
                        if (!opcode.Contains(str))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!name.Contains(str))
                        {
                            continue;
                        }
                    }
                }

                BaseInfo info = new BaseInfo(opcode, name);
                protocolView.Items.Add(info);
            }
        }
        private void RefreshSelect(XmlNode node)
        {
            if (node == null)
                return;
            XmlElement elem = (XmlElement)node;
            fromComboBox.SelectedItem = FindComboBoxItem(fromComboBox, elem.GetAttribute(fromStr));
            toComboBox.SelectedItem = FindComboBoxItem(toComboBox, elem.GetAttribute(toStr));
            name.Text = elem.GetAttribute("name");
            opCode.Text = elem.GetAttribute("opcode");
            typeComboBox.SelectedItem = FindComboBoxItem(typeComboBox, elem.GetAttribute("type"));
            string groupName = elem.GetAttribute("group");
            RefreshGroupSelect(groupName);
            SelectShow(IsContentClient());
            int selects = int.Parse(elem.GetAttribute("selects"));
            //compress.IsChecked = (selects & 1) > 0;
            encrypt.IsChecked = (selects & 2) > 0;
            //ProtocolInfo info = protocolInfos[elem.GetAttribute(toStr)];
            //string fullPath = info.bodyPath + "/" + name.Text + ".cs";
            itemList.Items.Clear();
            if (model != null)
            {
                Type type = model.GetType("Data." + name.Text);
                if (type == null)
                    return;
                FieldInfo[] fields = type.GetFields();
                if (fields == null)
                    return;
                string varName;
                string varType;
                for (int i = 0; i < fields.Length; i++)
                {
                    varName = fields[i].Name;
                    varType = fields[i].FieldType.Name;
                    if(fields[i].FieldType.GenericTypeArguments.Length > 0)
                    {
                        int index = varType.LastIndexOf("`");
                        string str = "<";
                        for (int j = 0; j < fields[i].FieldType.GenericTypeArguments.Length; j++)
                        {
                            str += fields[i].FieldType.GenericTypeArguments[j].Name;
                            if (j < (fields[i].FieldType.GenericTypeArguments.Length - 1))
                                str += ",";
                        }
                        str += ">";
                        varType = string.Format("{0}{1}", varType.Substring(0, index), str);
                    }
                    itemList.Items.Add(new { variableName = varName, variableType = varType });
                }
            }
        }
        private void RefreshGroupSelect(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                groupName = noneStr;
            }
            foreach(ComboBoxItem val in SelectGroup.Items)
            {
                if((string)val.Content == groupName)
                {
                    SelectGroup.SelectedItem = val;
                    return;
                }
            }
        }
        private ComboBoxItem FindComboBoxItem(ComboBox comboBox, string str)
        {
            ComboBoxItem item = null;
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                item = (ComboBoxItem)comboBox.Items[i];
                if ((string)item.Content == str)
                {
                    return item;
                }
            }
            return item;
        }
        private void remove_Click(object sender, RoutedEventArgs e)
        {
            if (protocolView.SelectedItems == null || protocolView.SelectedItems.Count == 0)
            {
                MessageBox.Show($"select is null.");
                return;
            }
            MessageBoxResult result = MessageBox.Show($"delete protocol?", "", MessageBoxButton.YesNoCancel);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            List<string> opcodes = new List<string>();
            for(int i = 0; i < protocolView.SelectedItems.Count; i++)
            {
                Type vr = protocolView.SelectedItems[i].GetType();
                PropertyInfo variableOpcode = vr.GetProperty("opcode");
                string opcode = (string)variableOpcode.GetValue(protocolView.SelectedItems[i]);
                opcodes.Add(opcode);
            }
            for(int i = 0; i < opcodes.Count; i++)
            {
                string opcode = opcodes[i];
                XmlNode node = FindProtocolNodeByOpcode(int.Parse(opcode));
                if (node == null)
                    continue;
                XmlElement elem = (XmlElement)node;
                string type = elem.GetAttribute("type");

                if (type == messageStr)
                {
                    DeleteProtocol(int.Parse(opcode));
                }
                else if (type == requsetStr)
                {
                    DeleteProtocol(int.Parse(opcode));
                    DeleteProtocol(int.Parse(opcode) + opCodeoffset);
                }
                else if (type == responseStr)
                {
                    DeleteProtocol(int.Parse(opcode));
                    DeleteProtocol(int.Parse(opcode) - opCodeoffset);
                }
            }
        }
        private void DeleteProtocol(int opcode)
        {
            XmlNode node = FindProtocolNodeByOpcode(opcode);
            XmlElement elem = (XmlElement)node;
            DeleteProtocolNode(node);
            ProtocolInfo info = protocolInfos[elem.GetAttribute(toStr)];
            string bodyPath = GetProtocolBodyPath(elem.GetAttribute(fromStr), elem.GetAttribute(toStr));
            string fullPath = bodyPath + elem.GetAttribute("name") + ".cs";
            if (File.Exists(fullPath))
            {
                MessageBoxResult result = MessageBox.Show($"delete file : {fullPath} ?", "", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    File.Delete(fullPath);
                }
            }
            string fullName2 = elem.GetAttribute("name") + "Handler";
            fullPath = info.handlePath + fullName2 + ".cs";
            if (File.Exists(fullPath))
            {
                MessageBoxResult result = MessageBox.Show($"delete file : {fullPath} ?", "", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    File.Delete(fullPath);
                }
            }
        }
        private XmlNode FindProtocolNodeByOpcode(int opcode)
        {
            XmlNodeList nodes = protocolRoot.SelectNodes(msgNodeName);
            if (nodes == null)
                return null;
            foreach (XmlNode node in nodes)
            {
                if (int.Parse(((XmlElement)node).GetAttribute("opcode")) == opcode)
                {
                    return node;
                }
            }
            return null;
        }
        private XmlNode FindProtocolNodeByName(string name)
        {
            XmlNodeList nodes = protocolRoot.SelectNodes(msgNodeName);
            if (nodes == null)
                return null;
            foreach (XmlNode node in nodes)
            {
                if (((XmlElement)node).GetAttribute("name") == name)
                {
                    return node;
                }
            }
            return null;
        }
        private void protocolView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(protocolView.SelectedItem == null)
            {
                return;
            }
            Type vr = protocolView.SelectedItem.GetType();
            PropertyInfo variableOpcode = vr.GetProperty("opcode");
            string opcode = (string)variableOpcode.GetValue(protocolView.SelectedItem);
            //PropertyInfo variableName = vr.GetProperty("name");
            //string name = (string)variableName.GetValue(itemList.SelectedItem);
            XmlNode node = FindProtocolNodeByOpcode(int.Parse(opcode));
            RefreshSelect(node);
        }
        private void Groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResetList();
        }

        private void createClient_Click(object sender, RoutedEventArgs e)
        {
            XmlDocument clientProtocolXml = new XmlDocument();
            XmlElement elem = (XmlElement)configNode.SelectSingleNode("msgClientPath");
            if (elem == null)
            {
                MessageBox.Show("not exsit msgClientPath in Config.xml");
                Close();
                return;
            }

            XmlNode node = clientProtocolXml.CreateXmlDeclaration("1.0", "utf-8", "");
            clientProtocolXml.AppendChild(node);
            XmlNode clientProtocolRoot = clientProtocolXml.AppendChild(clientProtocolXml.CreateElement("", "protocols", ""));

            XmlNodeList protocolNodes = protocolRoot.SelectNodes(msgNodeName);
            foreach(XmlNode protocolNode in protocolNodes)
            {
                XmlElement protocolElem = (XmlElement)protocolNode;
                if (protocolElem.GetAttribute(fromStr) == clientStr || protocolElem.GetAttribute(toStr) == clientStr)
                {
                    XmlElement msg = clientProtocolXml.CreateElement(msgNodeName);//创建一个<msg>节点 
                    msg.SetAttribute("name", protocolElem.GetAttribute("name"));
                    msg.SetAttribute("opcode", protocolElem.GetAttribute("opcode"));
                    msg.SetAttribute("type", protocolElem.GetAttribute("type"));
                    msg.SetAttribute("selects", protocolElem.GetAttribute("selects"));
                    clientProtocolRoot.AppendChild(msg);
                }
            }

            clientProtocolXml.Save(elem.GetAttribute("path"));
            CreateClientEnum();
        }

        private void addgroup_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(inputGroupName.Text))
            {
                MessageBox.Show("group name is null.");
                return;
            }
            if (IsExistGroup(inputGroupName.Text))
            {
                MessageBox.Show("exist same group name.");
                return;
            }
            AddGroupSelect(inputGroupName.Text);
            inputGroupName.Text = "";
            MessageBox.Show("add group success.");
        }

        private void modifygroup_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(inputGroupName.Text))
            {
                MessageBox.Show("group name is null.");
                return;
            }
            if (IsExistGroup(inputGroupName.Text))
            {
                MessageBox.Show("exist same group name.");
                return;
            }
            string showGroup = Groups.SelectedItem == null ? "" : (string)((ComboBoxItem)Groups.SelectedItem).Content;
            if (string.IsNullOrEmpty(showGroup))
            {
                MessageBox.Show("please select can modify group name.");
                return;
            }
            if (showGroup.ToLower() == noneStr || showGroup.ToLower() == allStr)
            {
                MessageBox.Show("can not modify none or all.");
                return;
            }
            ModifyGroup(showGroup, inputGroupName.Text);
            XmlNodeList nodes = protocolRoot.SelectNodes(msgNodeName);
            bool isSave = false;
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlElement elem = (XmlElement)nodes[i];
                string groupName = elem.GetAttribute("group");
                if (showGroup != groupName)
                    continue;
                elem.SetAttribute("group", inputGroupName.Text);
                isSave = true;
            }
            if(isSave)
            {
                Save();
            }
            inputGroupName.Text = "";
            MessageBox.Show("modify group success.");
        }

        private void deletegroup_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(inputGroupName.Text))
            {
                MessageBox.Show("group name is null.");
                return;
            }
            if (!IsExistGroup(inputGroupName.Text))
            {
                MessageBox.Show("not exist group name.");
                return;
            }
            if (inputGroupName.Text.ToLower() == noneStr || inputGroupName.Text.ToLower() == allStr)
            {
                MessageBox.Show("can not delete none or all group.");
                return;
            }
            RemoveGroup(inputGroupName.Text);
            if(Groups.SelectedItem != null)
            {
                string contentStr = (string)((ComboBoxItem)Groups.SelectedItem).Content;
                if(inputGroupName.Text == contentStr)
                {
                    Groups.SelectedItem = Groups.Items[0];
                }
            }
            if(SelectGroup.SelectedItem != null)
            {
                string contentStr = (string)((ComboBoxItem)SelectGroup.SelectedItem).Content;
                if (inputGroupName.Text == contentStr)
                {
                    SelectGroup.SelectedItem = null;
                }
            }

            XmlNodeList nodes = protocolRoot.SelectNodes(msgNodeName);
            bool isSave = false;
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlElement elem = (XmlElement)nodes[i];
                string groupName = elem.GetAttribute("group");
                if (inputGroupName.Text != groupName)
                    continue;
                elem.SetAttribute("group", noneStr);
                isSave = true;
            }
            if (isSave)
            {
                Save();
            }
            ResetList();
            inputGroupName.Text = "";
            MessageBox.Show("delete group success.");
        }

        private void CreateEnum()
        {
            FileStream fs = new FileStream(msgEnumPath, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("namespace Data");
            sw.WriteLine("{");
            sw.WriteLine("\tpublic enum ProtoEnum");
            sw.WriteLine("\t{");

            XmlNodeList nodes = protocolRoot.SelectNodes(msgNodeName);
            List<XmlElement> newNodes = new List<XmlElement>();
            foreach (XmlNode val in nodes)
            {
                string name = (val as XmlElement).GetAttribute("name");
                string opcode = (val as XmlElement).GetAttribute("opcode");
                sw.WriteLine($"\t\t{name} = {opcode},");
            }

            sw.WriteLine("\t}");
            sw.WriteLine("}");
            sw.Flush();
            sw.Close();
        }
        private void CreateClientEnum()
        {
            FileStream fs = new FileStream(clientEnumPath, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("namespace Data");
            sw.WriteLine("{");
            sw.WriteLine("\tpublic enum ProtoEnum");
            sw.WriteLine("\t{");

            XmlNodeList nodes = protocolRoot.SelectNodes(msgNodeName);
            List<XmlElement> newNodes = new List<XmlElement>();
            foreach (XmlNode val in nodes)
            {
                XmlElement protocolElem = (XmlElement)val;
                if (protocolElem.GetAttribute(fromStr) == clientStr || protocolElem.GetAttribute(toStr) == clientStr)
                {
                    string name = protocolElem.GetAttribute("name");
                    string opcode = protocolElem.GetAttribute("opcode");
                    sw.WriteLine($"\t\t{name} = {opcode},");
                }
            }

            sw.WriteLine("\t}");
            sw.WriteLine("}");
            sw.Flush();
            sw.Close();
        }
    }
}
