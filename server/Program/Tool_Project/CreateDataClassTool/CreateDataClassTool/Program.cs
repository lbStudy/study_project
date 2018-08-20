using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Configuration;
using System.Threading;

namespace CreateDataClassTool
{
    class Program
    {
        static Dictionary<string, string> typeTransform = new Dictionary<string, string> {
            { "integer", "int" },
            { "string", "string" },
            { "boolean", "bool" },
            { "float", "float" },
            { "int", "int" },
            { "bool", "bool" },};
        static List<string> createConfig = new List<string>();

        [STAThread]
        static void Main(string[] args)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择Excle文件";
            fileDialog.Filter = "Excle文件|*.xlsx";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] names = fileDialog.FileNames;
                if (names == null || names.Length == 0) return;

                ThreadPool.SetMaxThreads(3, 3);
                foreach (string file in names)
                {
                    createConfig.Add(file);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(Create), file);
                }
            }
            while(true)
            {
                Thread.Sleep(100);

                if (createConfig.Count == 0)
                    break;
            }
            Console.WriteLine("finish create.");
        }

        public static void Create(object obj)
        {
            string file = (string)obj;
            Dictionary<string, string> fieldDic = ReadFileFromExcel(file);
            if (fieldDic != null && fieldDic.Count > 0)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                string csPath = config.AppSettings.Settings["cspath"].Value + name + ".cs";
                FileStream fs = null;
                StreamWriter sw = null;
                try
                {
                    fs = new FileStream(csPath, FileMode.Create, FileAccess.Write);
                    sw = new StreamWriter(fs); // 创建写入流

                    sw.WriteLine("using System.Collections.Generic;");
                    sw.WriteLine("");
                    sw.WriteLine("namespace Model");
                    sw.WriteLine("{");
                    sw.WriteLine($"\tpublic class {name}");
                    sw.WriteLine("\t{");
                    foreach (KeyValuePair<string, string> pair in fieldDic)
                    {
                        sw.WriteLine($"\t\tpublic {pair.Value} {pair.Key};");
                    }
                    sw.WriteLine("\t}");
                    sw.WriteLine("}");
                    sw.Flush();
                    sw.Close();
                    Console.WriteLine($"create success : {file} ");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    if (sw != null)
                    {
                        sw.Close();
                    }
                }
                finally
                {
                    createConfig.Remove(file);
                }
            }
            else
            {
                createConfig.Remove(file);
            }
        }

        /// <summary>  
        /// 读取excel字段
        /// </summary>  
        /// <param name="filePath">excel路径</param>  
        /// <returns>返回datatable</returns>  
        public static Dictionary<string, string> ReadFileFromExcel(string filePath)
        {
            FileStream fs = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            //IRow describeRow = null;
            IRow nameRow = null;
            IRow typeRow = null;
            //ICell describeCell = null;
            ICell nameCell = null;
            ICell typeCell = null;
            Dictionary<string, string> fieldDic = new Dictionary<string, string>();
            string name = Path.GetFileName(filePath);
            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    // 2007版本  
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本  
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);

                    if (workbook == null)
                    {
                        return null;
                    }
                    sheet = workbook.GetSheetAt(0);//读取第一个sheet
                    if (sheet == null || sheet.LastRowNum < 2)
                    {
                        return null;
                    }
                    //describeRow = sheet.GetRow(0);//描述
                    typeRow = sheet.GetRow(1);//类型
                    nameRow = sheet.GetRow(2);//名称
                    int cellCount = nameRow.LastCellNum;//列数  

                    for (int j = nameRow.FirstCellNum; j <= cellCount; ++j)
                    {
                        nameCell = nameRow.GetCell(j);
                        typeCell = typeRow.GetCell(j);
                        //describeCell = describeRow.GetCell(j);
                        if (nameCell == null || typeCell == null)
                        {
                            continue;
                        }
                        else
                        {
                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)  
                            if (nameCell.CellType == CellType.String && typeCell.CellType == CellType.String)
                            {
                                if(string.IsNullOrEmpty(nameCell.StringCellValue) || string.IsNullOrEmpty(typeCell.StringCellValue))
                                {
                                    continue;
                                }
                                string typeStr = typeCell.StringCellValue.ToLower();
                                string newTypeStr = null;
                                if(typeStr == "array")
                                {
                                    string postilStr = null;
                                    if (typeCell.CellComment != null && typeCell.CellComment.String != null && typeCell.CellComment.String.String != null)
                                        postilStr = typeCell.CellComment.String.String;
                                    if (!string.IsNullOrEmpty(postilStr))
                                    {
                                        int firstIndex = postilStr.IndexOf('[');
                                        if (firstIndex >= 0)
                                        {
                                            string arrayTypeStr = postilStr.Substring(0, firstIndex).ToLower();
                                            string arrayTypeNewStr = null;
                                            if(typeTransform.TryGetValue(arrayTypeStr, out arrayTypeNewStr))
                                            {
                                                newTypeStr = postilStr.Replace(arrayTypeStr, arrayTypeNewStr);
                                            }
                                            else
                                            {
                                                Console.WriteLine($"{nameCell.StringCellValue} 数组备注类型错误 {postilStr} ,在表{name}.");
                                            }
                                        }
                                        else
                                        {
                                            firstIndex = postilStr.IndexOf('<');
                                            if(firstIndex >= 0)
                                            {
                                                string listStr = postilStr.Substring(0, firstIndex).ToLower();
                                                if (listStr == "list")
                                                {
                                                    string arrayTypeStr = postilStr.Substring(firstIndex + 1, postilStr.Length - firstIndex - 2).ToLower();
                                                    string arrayTypeNewStr = null;
                                                    if (typeTransform.TryGetValue(arrayTypeStr, out arrayTypeNewStr))
                                                    {
                                                        newTypeStr = "List<" + arrayTypeNewStr + ">";
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine($"{nameCell.StringCellValue} 数组备注类型错误 {postilStr} ,在表{name}.");
                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine($"{nameCell.StringCellValue} 备注类型错误 {postilStr} ,在表{name}.");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine($"{nameCell.StringCellValue} 备注类型错误 {postilStr} ,在表{name}.");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        newTypeStr = "int[]";
                                    }
                                }
                                else
                                {
                                    typeTransform.TryGetValue(typeStr, out newTypeStr);
                                }
                                if(!string.IsNullOrEmpty(newTypeStr))
                                {
                                    if(!fieldDic.ContainsKey(nameCell.StringCellValue))
                                        fieldDic.Add(nameCell.StringCellValue, newTypeStr);
                                    else
                                    {
                                        Console.WriteLine($"存在相同的字段名称 {nameCell.StringCellValue},在表{name}.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"{nameCell.StringCellValue} 类型错误 {typeCell.StringCellValue} ,在表{name}.");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                Console.WriteLine(e.ToString());
                return null;
            }
            return fieldDic;
        }
    }
}
