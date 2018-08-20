using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;

namespace CreateTool
{
    class Program
    {
        static string targetPaht = @"..\..\..\ClientProtocol\Model\";
        static string csproj = @"..\..\..\ClientProtocol\ClientProtocol.csproj";

        static void Main(string[] args)
        {
            string path = ConfigurationManager.AppSettings["path"];
            string[] csfiles = null;
            

            csfiles = Directory.GetFiles(targetPaht, "*.cs");
            foreach (string sourceFilePaht in csfiles)
            {
                File.Delete(sourceFilePaht);
            }

            string protocol_path = path + @"protocol\outer";
            CopyCS(protocol_path);

            string data_path = path + @"data\common";
            CopyCS(data_path);

            string base_path = path + @"base";
            CopyCS(base_path);

            XmlDocument configXml = new XmlDocument();
            if (!File.Exists(csproj))
            {
                Console.WriteLine($"not exsit {csproj}");
                return;
            }
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(csproj, settings);
            configXml.Load(reader);
            reader.Close();
            reader.Dispose();

            XmlNode parentNode = configXml.ChildNodes[1].ChildNodes[5];

            XmlNode firstNode = parentNode.FirstChild;
            parentNode.RemoveAll();

            parentNode.AppendChild(firstNode);

            csfiles = Directory.GetFiles(targetPaht, "*.cs");
            foreach (string sourceFilePaht in csfiles)
            {
                string name = Path.GetFileNameWithoutExtension(sourceFilePaht);
                XmlElement elem = configXml.CreateElement("Compile", configXml.DocumentElement.NamespaceURI);
                elem.SetAttribute("Include", $@"Model\{name}.cs");
                parentNode.AppendChild(elem);
            }
            configXml.Save(csproj);
        }
        static void CopyCS(string path)
        {
            string[] csfiles = Directory.GetFiles(path, "*.cs");
            foreach (string sourceFilePaht in csfiles)
            {
                string name = Path.GetFileNameWithoutExtension(sourceFilePaht);
                File.Copy(sourceFilePaht, $"{targetPaht}{name}.cs", true);
            }
        }
    }
}
