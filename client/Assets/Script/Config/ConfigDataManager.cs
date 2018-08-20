
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Config;
using System.Reflection;

public enum ConfigType
{
    characterconfig = 0,
    soundconfig,
    Max
}

public class ConfigDataManager : Singleton<ConfigDataManager>
{
    public Dictionary<int, characterconfig> characterconfigs;
    public Dictionary<int, soundconfig> soundconfigs;
    public void Load()
    {
        for (int i = 0; i < (int)ConfigType.Max; i++)
        {
            ConfigType configType = (ConfigType)i;
            string path = "Config/Table_ab_sg/" + Enum.GetName(typeof(ConfigType), configType) + ".txt";
            TextAsset configTxt = BundleManager.Instance.LoadAsset<TextAsset>(path);
            if (configTxt != null)
            {
                AnalysisConfig(configType, configTxt);
            }
            else
            {
                Debug.LogError($"Not exist config, path : {path}");
            }
        }
    }
    private void AnalysisConfig(ConfigType configType, TextAsset configTxt)
    {
        string fieldName = Enum.GetName(typeof(ConfigType), configType) + "s";

        FieldInfo fieldInfo = Instance.GetType().GetField(fieldName);
        if(fieldInfo != null)
        {
            try
            {
                //object obj = JsonConvert.DeserializeObject(configTxt.text, fieldInfo.FieldType);
                object obj = fastJSON.JSON.ToObject(configTxt.text, fieldInfo.FieldType);
                fieldInfo.SetValue(Instance, obj);
            }
            catch(Exception e)
            {
                Debug.LogError($"Error AnalysisConfig : {fieldName}");
                Debug.LogError(e.ToString());
            }
        }
        else
        {
            Debug.LogError($"Not exist config field : {fieldName}");
        }
    }
}
