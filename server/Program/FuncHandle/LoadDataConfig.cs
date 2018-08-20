using System.Collections.Generic;
using Base;
using System;
using Data;

namespace FuncHandle
{
    [Function((int)FunctionId.LoadDataConfig)]
    public class LoadDataConfig : IFunc<List<int>>
    {
        public void Run(List<int> configStrs)
        {
            if (configStrs == null || configStrs.Count == 0)
            {
                List<ConfigType> configTypes = new List<ConfigType>();
                for (int i = 0; i < configStrs.Count; i++)
                {
                    if (Enum.IsDefined(typeof(ConfigType), configStrs[i]))
                    {
                        configTypes.Add((ConfigType)configStrs[i]);
                    }
                }
                ConfigManagerComponent.Instance.Load(configTypes);
            }
        }
    }
}
