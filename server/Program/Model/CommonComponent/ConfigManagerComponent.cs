using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using Base;
using Config;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
public class ConfigTypeDescription : Attribute
{
    public List<AppType> appTypes = new List<AppType>();
    public ConfigTypeDescription(params AppType[] appTypes)
    {
        this.appTypes.AddRange(appTypes);
    }
}
public enum ConfigType
{
    [ConfigTypeDescription(AppType.BattleServer)]
    characterconfig,
    Max
}
public class ConfigManagerComponent : Component , IAwake
{
    public static ConfigManagerComponent Instance;
    object lockobj = new object();
    List<ConfigType> loadConfig = new List<ConfigType>();


    public Dictionary<int, characterconfig> characterconfigs;

        
    public void Awake()
    {
        Instance = this;
        Load();
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
    private void Load()
    {
        for (int i = 0; i < (int)ConfigType.Max; i++)
        {
            Add((ConfigType)i);
        }
        if (loadConfig.Count == 0)
            Game.Instance.SetInitFinishModule(InitModule.LoadConfig);
        else
            OpenLoadThread();
    }
    public void Load(List<ConfigType> configTypes)
    {
        if(configTypes == null || configTypes.Count == 0)
        {
            return;
        }
        for(int i = 0; i < configTypes.Count; i++)
        {
            Add(configTypes[i]);
        }
        if (loadConfig.Count > 0)
            OpenLoadThread();
    }
    private void Add(ConfigType configType)
    {
        lock (lockobj)
        {
            if (IsCanLoadConfig(configType) && !loadConfig.Contains(configType))
            {
                loadConfig.Add(configType);
            }
        }
    }
    private void OpenLoadThread()
    {
        Thread thread = new Thread(LoadThread);
        thread.Start();
    }
    private void LoadThread()
    {
        ConfigType configType;
        while (true)
        {
            lock (lockobj)
            {
                if(loadConfig.Count > 0)
                {
                    configType = loadConfig[0];
                    loadConfig.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }
            Load(configType);
            Thread.Sleep(1);
        }
        Game.Instance.SetInitFinishModule(InitModule.LoadConfig);
    }
    private bool IsCanLoadConfig(ConfigType configType)
    {
        Type type = configType.GetType();
        FieldInfo fd = type.GetField(configType.ToString());
        if (fd == null)
            return false;
        ConfigTypeDescription[] ctds = (ConfigTypeDescription[])fd.GetCustomAttributes(typeof(ConfigTypeDescription), false);
        if (ctds == null || ctds.Length != 1) return false;
        ConfigTypeDescription ctd = ctds[0];
        return ctd.appTypes.Contains(Game.Instance.AppType) || ctd.appTypes.Contains(AppType.All);
    } 
    private void Load(ConfigType configType)
    {
        string path = ConstDefine.configPath + configType.ToString() + ".txt";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            AnalysisConfig(configType, ref data);
        }
        else
        {
            string log = $"not exist file, path : {path}";
            Log.Debug(log);
            Console.WriteLine(log);
        }
    }
    private void AnalysisConfig(ConfigType configType, ref string data)
    {
        string fieldName = configType.ToString() + "s";
        FieldInfo fieldInfo = Instance.GetType().GetField(fieldName);
        if(fieldInfo != null)
        {
            try
            {
                object obj = JsonConvert.DeserializeObject(data, fieldInfo.FieldType);
                fieldInfo.SetValue(Instance, obj);

                string log = $"{configType.ToString()} config load  success.";
                Log.Debug(log);
                Console.WriteLine(log);
            }
            catch(Exception e)
            {
                string log = $"analysis config failed, {configType.ToString()}.";
                Log.Debug(log);
                Console.WriteLine(log);
                Log.Debug(e.ToString());
                Console.WriteLine(e.ToString());
            }
        }
        else
        {
            string log = $"not add {fieldName} field in ConfigManagerComponent.";
            Log.Debug(log);
            Console.WriteLine(log);
        }
    }
    //private void AnalysisConfig<T>(ref T outVal, ref string data)
    //{
    //    outVal = JsonConvert.DeserializeObject<T>(data);
    //}
}
