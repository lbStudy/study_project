using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlatformHelper
{
    public static string StreamingAssetsPath
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }
    public static string StreamingAssetsPath_file
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE || !UNITY_ANDROID
            return "file://" + Application.streamingAssetsPath + "/";
#else
            return Application.streamingAssetsPath + "/";
#endif
        }
    }
    public static string PersistentDataPath
    {
        get
        {
            return Application.persistentDataPath + "/";
        }
    }
    public static string PersistentDataPath_file
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return "file://" + Application.persistentDataPath + "/";
#else
            return Application.persistentDataPath + "/";
#endif
        }
    }
    public static string DataPath
    {
        get
        {
            return Application.dataPath + "/";
        }
    }
}
