using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Data;
using System.Configuration;

class Program
{
    static void Main(string[] args)
    {
        CreateCs();
    }
    static void CreateCs()
    {
        string path = ConfigurationManager.AppSettings["path"];
        //string enumfileStr = ConfigurationManager.AppSettings["enumfile"];
        //if(!File.Exists(enumfileStr))
        //{
        //    Console.WriteLine($"not exist path : {enumfileStr}");
        //    return;
        //}
        if (Directory.Exists(path) == false)//如果不存在就创建file文件夹
        {
            Console.WriteLine($"not exist path : {path}");
            return;
            //Directory.CreateDirectory("./Attribute");
        }

        //FileStream enumFile = new FileStream(enumfileStr, FileMode.Open);
        //StreamReader enumFileStream = new StreamReader(enumFile, Encoding.Default);
        //string c = enumFileStream.ReadToEnd();
        //c.Split('[');

        FileStream fs = new FileStream("./PlayerAttributeTemplate.txt", FileMode.Open);
        StreamReader sr = new StreamReader(fs, Encoding.Default);

        StreamWriter writer2 = new StreamWriter($"{path}/PlayerCommonData.cs", false, Encoding.Default);
        writer2.AutoFlush = false;

        writer2.WriteLine("public partial class PlayerCommonData");
        writer2.WriteLine("{");

        StreamWriter writer3 = new StreamWriter($"{path}/PlayerDataHandle.cs", false, Encoding.Default);
        writer3.AutoFlush = false;

        writer3.WriteLine("using Model;");
        writer3.WriteLine("\n");
        writer3.WriteLine("public partial class PlayerDataHandle");
        writer3.WriteLine("{");
        writer3.WriteLine("\tprivate PlayerDataHandle()");
        writer3.WriteLine("\t{");

        foreach (D_AttributeType val in (typeof(D_AttributeType)).GetEnumValues())
        {
            Type type = val.GetType();
            FieldInfo fd = type.GetField(val.ToString());
            if (fd == null)
                continue;
            PlayerAttributeDescription[] attrs = (PlayerAttributeDescription[])fd.GetCustomAttributes(typeof(PlayerAttributeDescription), false);
            if (attrs.Length > 0)
            {
                PlayerAttributeDescription des = attrs[0];
                string scriptName = "D_" + val.ToString();
                string csPath = path + "/" + scriptName + ".cs";
                string str = val.ToString();
                string dStr = char.ToUpper(str[0]) + str.Substring(1);

                writer2.WriteLine($"\tpublic {des.valType} {dStr} " + "{ get { return " + "detailData." + str + "; } }");
                writer3.WriteLine($"\t\tattributeHandleFuncs[(int)D_AttributeType.{str}] = new {scriptName}();");

                if (File.Exists(csPath))
                    continue;

                StreamWriter writer = new StreamWriter(csPath, false, Encoding.Default);
                writer.AutoFlush = false;
                String line;
                string type2 = "PlayerDetailData";
                string variable1 = "detail";
                string variable2 = "detail." + str;
                string variable3 = "player.CommonData." + dStr;
                while ((line = sr.ReadLine()) != null)
                {
                    line = StringReplace(line, "ClassName", scriptName);
                    line = StringReplace(line, "type1", des.valType);
                    line = StringReplace(line, "type2", type2);
                    line = StringReplace(line, "variable1", variable1);
                    line = StringReplace(line, "variable2", variable2);
                    line = StringReplace(line, "variable3", variable3);
                    line = StringReplace(line, "const1", des.maxVal.ToString());
                    writer.WriteLine(line);
                }
                writer.Flush();
                writer.Close();
                fs.Seek(0, SeekOrigin.Begin);
            }
        }

        //foreach (E_AttributeType val in (typeof(E_AttributeType)).GetEnumValues())
        //{
        //    Type type = val.GetType();
        //    FieldInfo fd = type.GetField(val.ToString());
        //    if (fd == null)
        //        continue;
        //    PlayerAttributeDescription[] attrs = (PlayerAttributeDescription[])fd.GetCustomAttributes(typeof(PlayerAttributeDescription), false);
        //    if (attrs.Length > 0)
        //    {
        //        PlayerAttributeDescription des = attrs[0];

        //        string scriptName = "E_" + val.ToString();
        //        string csPath = path + "/" + scriptName + ".cs";

        //        string str = val.ToString();
        //        string dValStr = char.ToUpper(str[0]) + str.Substring(1);

        //        writer2.WriteLine($"\tpublic {des.valType} {dValStr} " + "{ get { return " + "extraData." + str + "; } }");
        //        writer3.WriteLine($"\t\tattributeHandleFuncs[(int)E_AttributeType.{str}] = new {scriptName}();");

        //        if (File.Exists(csPath))
        //            continue;

        //        StreamWriter writer = new StreamWriter(csPath, false, Encoding.Default);
        //        writer.AutoFlush = false;
        //        String line;
        //        string type2 = "PlayerExtraData";
        //        string variable1 = "extra";
        //        string variable2 = "extra." + str;
        //        string variable3 = "player.CommonData." + dValStr;
        //        while ((line = sr.ReadLine()) != null)
        //        {
        //            line = StringReplace(line, "ClassName", scriptName);
        //            line = StringReplace(line, "type1", des.valType);
        //            line = StringReplace(line, "type2", type2);
        //            line = StringReplace(line, "variable1", variable1);
        //            line = StringReplace(line, "variable2", variable2);
        //            line = StringReplace(line, "variable3", variable3);
        //            line = StringReplace(line, "const1", des.maxVal.ToString());
        //            writer.WriteLine(line);
        //        }
        //        writer.Flush();
        //        writer.Close();
        //        fs.Seek(0, SeekOrigin.Begin);
        //    }
        //}

        sr.Close();
        fs.Close();

        writer2.WriteLine("}");

        writer2.Flush();
        writer2.Close();

        writer3.WriteLine("\t}");
        writer3.WriteLine("}");

        writer3.Flush();
        writer3.Close();
    }
    public static string StringReplace(string str, string toRep, string strRep)
    {
        StringBuilder sb = new StringBuilder();

        int np = 0, n_ptmp = 0;

        for (;;)
        {
            string str_tmp = str.Substring(np);
            n_ptmp = str_tmp.IndexOf(toRep);

            if (n_ptmp == -1)
            {
                sb.Append(str_tmp);
                break;
            }
            else
            {
                sb.Append(str_tmp.Substring(0, n_ptmp)).Append(strRep);
                np += n_ptmp + toRep.Length;
            }
        }
        return sb.ToString();
    }
}
