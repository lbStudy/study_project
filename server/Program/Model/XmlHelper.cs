using System;
using System.Reflection;
using System.Security;
using System.Xml;


public class XmlHelper
{
    static public T GreateAndSetValue<T>(SecurityElement node)
    {
        T obj = Activator.CreateInstance<T>();
        FieldInfo[] fields = typeof(T).GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            string name = string.Format("{0}", fields[i].Name);
            if (string.IsNullOrEmpty(name)) continue;

            string fieldValue = node.Attribute(name);
            if (string.IsNullOrEmpty(fieldValue)) continue;
            try
            {
                TableParser.ParsePropertyValue<T>(obj, fields[i], fieldValue);

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("XML读取错误：对象类型({2}) => 属性名({0}) => 属性类型({3}) => 属性值({1})",
                    fields[i].Name, fieldValue, typeof(T).ToString(), fields[i].FieldType.ToString()));
            }
        }
        return obj;
    }

    static public T GreateAndSetValue<T>(XmlElement node)
    {
        T obj = Activator.CreateInstance<T>();
        FieldInfo[] fields = typeof(T).GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            string name = string.Format("{0}", fields[i].Name);
            if (string.IsNullOrEmpty(name)) continue;

            string fieldValue = node.GetAttribute(name);
            if (string.IsNullOrEmpty(fieldValue)) continue;
            try
            {
                TableParser.ParsePropertyValue<T>(obj, fields[i], fieldValue);

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("XML读取错误：对象类型({2}) => 属性名({0}) => 属性类型({3}) => 属性值({1})",
                    fields[i].Name, fieldValue, typeof(T).ToString(), fields[i].FieldType.ToString()));
            }
        }
        return obj;
    }
}