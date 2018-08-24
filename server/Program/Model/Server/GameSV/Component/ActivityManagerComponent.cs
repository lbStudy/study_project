using System;
using System.Collections.Generic;
using Base;
using Data;
using System.Threading.Tasks;

public class ActivityManagerComponent : Component, IAwake
{
    public static ActivityManagerComponent Instance;

    List<ActivityInfo> activitys = new List<ActivityInfo>();
    Dictionary<int, ActivityInfo> activityDic = new Dictionary<int, ActivityInfo>();
    Dictionary<int, List<ActivityInfo>> activityTypeDic = new Dictionary<int, List<ActivityInfo>>();

    public List<ActivityInfo> Activitys { get { return activitys; } }

    public void Awake()
    {
        Instance = this;
        GetActivity();
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
    public async void GetActivity()
    {
        //while(true)
        //{
        //    if(Game.Instance.IsFinishModule(InitModule.InnerConnect))
        //    {
        //        Gate2M_GetActivityRequest gate2m = new Gate2M_GetActivityRequest();
        //        Session managerSession = NetInnerComponent.Instance.GetByAppID(ServerConfigComponent.Instance.ManagerAppId);
        //        Gate2M_GetActivityResponse respFromM = await managerSession.Call(gate2m) as Gate2M_GetActivityResponse;
        //        if (respFromM.errorCode == (int)ErrorCode.Success)
        //        {
        //            for (int i = 0; i < respFromM.activitys.Count; i++)
        //            {
        //                ActivityInfo activity = respFromM.activitys[i];
        //                if (activity.op == 2)
        //                {
        //                    ActivityManagerComponent.Instance.Remove(activity.id);
        //                }
        //                else
        //                {
        //                    ActivityManagerComponent.Instance.Refersh(activity);
        //                }
        //            }
        //            Game.Instance.SetInitFinishModule(InitModule.GetActivity);
        //            return;
        //        }
        //    }
        //    await Task.Delay(2000);
        //}
    }
    public ActivityInfo FindById(int activityid)
    {
        ActivityInfo activity = null;
        activityDic.TryGetValue(activityid, out activity);
        return activity;
    }
    public List<ActivityInfo> FindByType(ActivityType activityType)
    {
        List<ActivityInfo> ais = null;
        activityTypeDic.TryGetValue((int)activityType, out ais);
        return ais;
    }

    public void Refersh(ActivityInfo activity)
    {
        if (activity == null)
            return;

        activityDic[activity.id] = activity;

        int index = activitys.FindIndex(x => x.id == activity.id);
        if (index >= 0)
            activitys[index] = activity;
        else
            activitys.Add(activity);

        List<ActivityInfo> ais = null;
        if (!activityTypeDic.TryGetValue(activity.type, out ais))
        {
            ais = new List<ActivityInfo>();
            activityTypeDic.Add(activity.type, ais);
        }
        index = ais.FindIndex(x => x.id == activity.id);
        if (index >= 0)
            ais[index] = activity;
        else
            ais.Add(activity);
    }
    public void Remove(int activityid)
    {
        if (activityid <= 0)
            return;
        ActivityInfo ai = null;
        if(activityDic.TryGetValue(activityid, out ai))
        {
            activityDic.Remove(activityid);
            activitys.Remove(ai);
            List<ActivityInfo> ais = null;
            if (activityTypeDic.TryGetValue(ai.type, out ais))
            {
                ais.Remove(ai);
            }
        }
    }
}
