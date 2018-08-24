using Data;
using Base;
using System.Collections.Generic;

namespace EventHandle
{
    [Event((int)EventIdType.ActivityEnd, AppType.ManagerServer)]
    public class GmActivityEndEvent : IEvent<List<ActivityInfo>>
    {
        public void Run(System.Collections.Generic.List<ActivityInfo> activityInfos)
        {
            M2Gate_RefreshActivityMessage m2gate = new M2Gate_RefreshActivityMessage();
            m2gate.activitys = activityInfos;
            //NetInnerComponent.Instance.SendMsgToSevers(m2gate, AppType.GateServer);
        }
    }
}
