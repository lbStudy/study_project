using System.Collections.Generic;
using Data;
using Base;

namespace FuncHandle
{
    [Function((int)FunctionId.RefreshActivity)]
    public class RefreshActivity : IFunc<List<ActivityInfo>, bool>
    {
        public void Run(List<ActivityInfo> activitys, bool isNotify)
        {
            if (activitys != null)
            {
                for (int i = 0; i < activitys.Count; i++)
                {
                    ActivityInfo activity = activitys[i];
                    if (activity.op == 1)
                    {
                        long startTime = 0;
                        long endTime = 0;
                        if (long.TryParse(activity.startTime, out startTime) && long.TryParse(activity.endTime, out endTime))
                        {
                            activity.StartTime = startTime;
                            activity.EndTime = endTime;
                            if (Game.Instance.Sec >= activity.EndTime)
                            {
                                Log.Warning($"activity{activity.id} time end.");
                                continue;
                            }
                            GmActivityManagerComponent.Instance.Refresh(activity);
                        }
                        else
                        {
                            Log.Warning($"activitytype({((ActivityType)activity.type).ToString()}) activity.startTime({activity.startTime}) or activity.endTime({activity.endTime}) parse error.");
                        }
                    }
                    else if (activity.op == 2)
                    {
                        GmActivityManagerComponent.Instance.Remove(activity.id);
                    }
                }
                if(isNotify)
                {
                    M2Gate_RefreshActivityMessage m2gate = new M2Gate_RefreshActivityMessage();
                    m2gate.activitys = activitys;
                    //NetInnerComponent.Instance.SendMsgToSevers(m2gate, AppType.GateServer);
                }
            }
        }
    }
}
