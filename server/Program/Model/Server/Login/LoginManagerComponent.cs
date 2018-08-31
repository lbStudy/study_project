using System;
using System.Collections.Generic;
using Base;
using Data;

public class LoginManagerComponent : Component, IAwake
{
    public static LoginManagerComponent Instance;

    Dictionary<long, LoginInfo> idLoginDic = new Dictionary<long, LoginInfo>();                 //key:id
    public Dictionary<int, AreaInfo> areaDic = new Dictionary<int, AreaInfo>();                 //key1:大区id
    public List<AreaInfo> areas = new List<AreaInfo>();
    HashSet<string> registerAccount = new HashSet<string>();

    public void Awake()
    {
        Instance = this;
        //foreach(BigAreaConfig bigAreaCf in ServerConfigComponent.Instance.bigAreaCfDic.Values)
        //{
        //    AreaInfo areaInfo = new AreaInfo();
        //    areaInfo.enterCount = 0;
        //    areaInfo.id = bigAreaCf.bigAreaId;
        //    areaInfo.name = bigAreaCf.bigAreaName;
        //    areaInfo.isOpen = true;
        //    areaDic.Add(areaInfo.id, areaInfo);
        //    areas.Add(areaInfo);
        //}
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
    public LoginInfo FindLoginInfoById(long id)
    {
        LoginInfo info = null;
        idLoginDic.TryGetValue(id, out info);
        return info;
    }
    public LoginInfo FinishLogin(long id)
    {
        LoginInfo info = ObjectPoolManager.Instance.Take<LoginInfo>();
        info.id = id;
        idLoginDic[info.id] = info;
        return info;
    }
    public void RemoveLoginInfo(long id)
    {
        LoginInfo info = null;
        if (idLoginDic.TryGetValue(id, out info))
        {
            idLoginDic.Remove(id);
            info.Dispose();
        }
    }
    public void AddRegisterAccount(string account)
    {
        registerAccount.Add(account);
    }
    public bool IsExistRegisterAccount(string account)
    {
        return registerAccount.Contains(account);
    }
    public void RemoveRegisterAccount(string account)
    {
        registerAccount.Remove(account);
    }
    public AreaInfo FindAreaInfo(int areaid)
    {
        AreaInfo areaInfo = null;
        areaDic.TryGetValue(areaid, out areaInfo);
        return areaInfo;
    }
}
