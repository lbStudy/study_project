using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using UnityEngine.U2D;
using System.Reflection;
using UnityEditor.Sprites;

public class CreateBundleUpdateFile : AssetPostprocessor
{
    [MenuItem("Tools/CreateBundleName")]
    static void CreateBundleName()
    {
        SetAssetBundleName("Assets");
    }
    public static void SetAssetBundleName(string dirPath)
    {
        string[] dirPaths = Directory.GetDirectories(dirPath);
        foreach(string v in dirPaths)
        {
            string dirName = Path.GetFileNameWithoutExtension(v);
            if(dirName == "StreamingAssets")
            {
                continue;
            }
            if (dirName.Contains("_ab"))
            {
                if(dirName.Contains("_sg"))
                {
                    SetAssetBundleBySelfName(v);
                }
                else
                {
                    SetAssetBundleNameByDirName(v);
                }
            }
            SetAssetBundleName(v);
        }
    }
    public static void SetAssetBundleNameByDirName(string dirPath)
    {        dirPath = dirPath.Replace("\\", "/");        int index = dirPath.IndexOf("Assets/");
        string bundleName = dirPath.Substring(index + 7);
        string[] files = Directory.GetFiles(dirPath);
        foreach (string filename in files)
        {
            string fname = filename.Replace("\\", "/");
            AssetImporter importer = AssetImporter.GetAtPath(fname);
            if (importer != null)
            {
                importer.SetAssetBundleNameAndVariant(bundleName, "");
            }
        }
    }
    public static void SetAssetBundleBySelfName(string pathname)
    {
        string[] files = Directory.GetFiles(pathname);
        foreach (string filename in files)
        {
            string fname = filename.Replace("\\", "/");

            AssetImporter importer = AssetImporter.GetAtPath(fname);
            if (importer != null)
            {
                string name = Path.GetFileNameWithoutExtension(fname);
                string targetPath = fname.Replace("Assets/", "");
                targetPath = targetPath.Substring(0, targetPath.LastIndexOf(name));
                if (!targetPath.EndsWith("/"))
                {
                    targetPath += "/";
                }
                importer.SetAssetBundleNameAndVariant(targetPath + name, "");
            }
        }
    }
    public static void SetAssetBundleName(string pathname, string bundleName)
    {
        AssetImporter importer = AssetImporter.GetAtPath(pathname);
        if (importer != null)
        {
            importer.SetAssetBundleNameAndVariant(bundleName, "");
        }
    }
    [MenuItem("Tools/CreateBundleUpdateFile")]
    static void CreateUpdateFile()
    {
        string path = System.Environment.CurrentDirectory + "/AssetBundles";

        string[] directoryPaths = Directory.GetDirectories(path);

        foreach(string v in directoryPaths)
        {
            //DirectoryInfo di = Directory.CreateDirectory(v);
            string dirName = Path.GetFileNameWithoutExtension(v);
            FileStream fs1 = new FileStream(v + "/" + dirName + ".txt", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs1);

            //sw.WriteLine(Log);//开始写入值
            CreateUpdateFile(v, sw, ref dirName);
            sw.Close();
            fs1.Close();

        }
    }

    static void CreateUpdateFile(string dirPath, StreamWriter sw, ref string dirName)
    {
        string[] allFilePaths = Directory.GetFiles(dirPath);
        foreach (string filePath in allFilePaths)
        {
            if (Path.HasExtension(filePath))
                continue;
            string md5_str = GetMD5HashFromFile(filePath);
            int index = filePath.IndexOf(dirName);
            string name = filePath.Substring(index + dirName.Length + 1);
            FileInfo info = new FileInfo(filePath);
            //Console.WriteLine("文件大小=" + System.Math.Ceiling(info.Length / 1024.0) + " KB");
            //float kb = (long)((info.Length / 1024f) * 1000) * 0.001f; 
            sw.WriteLine(name + " " + md5_str + " " + info.Length);
        }
        string[] directoryPaths = Directory.GetDirectories(dirPath);
        foreach (string v in directoryPaths)
        {
            CreateUpdateFile(v, sw, ref dirName);
        }
    }
    private static string GetMD5HashFromFile(string fileName)
    {
        try
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("GetMD5HashFromFile() fail, error:" + ex.Message);
        }
    }
}
