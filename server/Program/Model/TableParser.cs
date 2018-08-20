using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

public static class TableParser
{
    public static void ParsePropertyValue<T>(T obj, FieldInfo fieldInfo, string valueStr)
    {
        System.Object value = valueStr;
        if (fieldInfo.FieldType.IsEnum)
            value = Enum.Parse(fieldInfo.FieldType, valueStr);
        else
        {
            if (fieldInfo.FieldType == typeof(int))
                value = int.Parse(valueStr);
            else if (fieldInfo.FieldType == typeof(byte))
                value = byte.Parse(valueStr);
            else if (fieldInfo.FieldType == typeof(int))
                value = int.Parse(valueStr);
            else if (fieldInfo.FieldType == typeof(float))
                value = float.Parse(valueStr);
            else if (fieldInfo.FieldType == typeof(double))
                value = double.Parse(valueStr);
            else if (fieldInfo.FieldType == typeof(uint))
                value = uint.Parse(valueStr);
            else if(fieldInfo.FieldType == typeof(ulong))
                value = ulong.Parse(valueStr);
            else if (fieldInfo.FieldType == typeof(long))
                value = long.Parse(valueStr);
            else
            {
                if (valueStr.Contains("\"\""))
                    valueStr = valueStr.Replace("\"\"", "");

                if (valueStr.Contains("\\n"))
                    valueStr = valueStr.Replace("\\n", "\n");

                // process the excel string.
                if (valueStr.Length > 2 && valueStr[0] == '\"' && valueStr[valueStr.Length - 1] == '\"')
                    valueStr = valueStr.Substring(1, valueStr.Length - 2);

                value = valueStr;
            }
        }

        if (value == null)
            return;

        fieldInfo.SetValue(obj, value);
    }

    static T ParseObject<T>(string[] lines, int idx, Dictionary<int, FieldInfo> propertyInfos)
    {
        T obj = Activator.CreateInstance<T>();
        string line = lines[idx];
        string[] values = line.Split('\t');
        foreach (KeyValuePair<int, FieldInfo> pair in propertyInfos)
        {
            if (pair.Key >= values.Length)
                continue;

            string value = values[pair.Key];
            if (string.IsNullOrEmpty(value))
                continue;

            try
            {
                ParsePropertyValue(obj, pair.Value, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("[{5}]ParseError: Row={0} Column={1} Name={2} Want={3} Get={4}",
                    idx + 1,
                    pair.Key + 1,
                    pair.Value.Name,
                    pair.Value.FieldType.Name,
                    value,
                    typeof(T).ToString()));
                throw ex;
            }
        }
        return obj;
    }

    static Dictionary<int, FieldInfo> GetPropertyInfos<T>(string memberLine)
    {
        Type objType = typeof(T);

        string[] members = memberLine.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        Dictionary<int, FieldInfo> propertyInfos = new Dictionary<int, FieldInfo>();
        for (int i = 0; i < members.Length; i++)
        {
            FieldInfo fieldInfo = objType.GetField(members[i]);
            if (fieldInfo == null)
                continue;
            propertyInfos[i] = fieldInfo;
        }

        return propertyInfos;
    }


    public static T[] ParseContent<T>(string content)
    {
        // try parse the table lines.
        string[] lines = content.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 3)
        {
            Console.WriteLine("表格文件行数错误，【1】属性名称【2】变量名称【3-...】值：" + typeof(T).ToString());
            return null;
        }

        // fetch all of the field infos.
        Dictionary<int, FieldInfo> propertyInfos = GetPropertyInfos<T>(lines[1]);

        // parse it one by one.
        T[] array = new T[lines.Length - 2];
        for (int i = 0; i < lines.Length - 2; i++)
            array[i] = ParseObject<T>(lines, i + 2, propertyInfos);

        return array;
    }

    public static List<int[]> ParsePropertyValue(string text)
    {
        List<int[]> PropertyList = new List<int[]>();
        if (text != "\"" && text != "")
        {
            string[] propertys = text.Split(";".ToCharArray());
            foreach (string propertyString in propertys)
            {
                string[] propertySet = propertyString.Split("-".ToCharArray());
                if (propertySet.Length >= 2)
                {
                    int[] PropertyValue = new int[propertySet.Length];
                    for (int i = 0; i < propertySet.Length; i++)
                        PropertyValue[i] = int.Parse(propertySet[i]);
                    PropertyList.Add(PropertyValue);
                }
            }
        }
        return PropertyList;
    }
}
