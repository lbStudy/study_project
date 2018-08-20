using System;
using System.Collections.Generic;
using Data;
using Base;


public class GmActivityManagerComponent : Component, IAwake
{
    public static GmActivityManagerComponent Instance;

    public List<ActivityInfo> activitys = new List<ActivityInfo>();

    public void Awake()
    {
        Instance = this;
        IntervalTask task = new IntervalTask(1000, TimeDetection);
        TimeManagerComponent.Instance.Add(task);
        ReqActivityInfo();
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
    public void TimeDetection()
    {
        if (Game.Instance.State != GameState.Runing)
            return;
        ActivityInfo info = null;
        List<ActivityInfo> rmAi = null;
        for (int i = activitys.Count - 1; i >= 0; i--)
        {
            info = activitys[i];
            if (info.EndTime <= Game.Instance.Sec)
            {//结束
                info.op = 2;
                Remove(info);
                if (rmAi == null)
                    rmAi = new List<ActivityInfo>();
                rmAi.Add(info);
            }
        }
        if(rmAi != null)
            EventDispatcher.Instance.Run<List<ActivityInfo>>((int)EventIdType.ActivityEnd, rmAi);
    }
    public class HttpActivityResponse
    {
        public int retCode;
        public List<ActivityInfo> activitys;
    }
    async void ReqActivityInfo()
    {
        string url = ServerConfigComponent.Instance.GetWebUrl("activity");
        if(string.IsNullOrEmpty(url))
        {
            Console.WriteLine("build activity.");
            FuncDispatcher.Instance.Run((int)FunctionId.RefreshActivity, FuncDispatcher.Instance.Run<List<ActivityInfo>>((int)FunctionId.BuildActivity), false);
            //Console.WriteLine("activity get is fail, activity url is null.");
            //Log.Warning("activity get is fail, activity url is null.");
        }
        else
        {
            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>() { { "dataType", "get_activitys" } };
                string content = await ClientHttpComponent.Instance.HttpPostAsync(url, MiniJSON.Json.Serialize(dic));
                HttpActivityResponse reqContent = LitJson.JsonMapper.ToObject<HttpActivityResponse>(content);
                FuncDispatcher.Instance.Run((int)FunctionId.RefreshActivity, reqContent.activitys, false);
            }
            catch (Exception e)
            {
                Console.WriteLine("activity get is fail.");
                Log.Error(e.ToString());
                Console.WriteLine(e.ToString());
            }
        }
        Game.Instance.SetInitFinishModule(InitModule.GetActivity);
    }
    public void Refresh(ActivityInfo activity)
    {
        if (activity == null)
            return;

        int index = activitys.FindIndex(x => x.id == activity.id);
        if (index >= 0)
            activitys[index] = activity;
        else
            activitys.Add(activity);
    }
    public void Remove(ActivityInfo activity)
    {
        if (activity == null)
            return;
        activitys.Remove(activity);
    }
    public void Remove(int activityid)
    {
        activitys.RemoveAll(x => x.id == activityid);
    }
}
