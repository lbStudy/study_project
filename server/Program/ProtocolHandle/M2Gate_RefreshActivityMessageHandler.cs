using Base;
using Data;
using System.Collections.Generic;

namespace ProtocolHandle
{
    [Protocol(8407)]
    public class M2Gate_RefreshActivityMessageHandler : AMHandler<M2Gate_RefreshActivityMessage>
    {
        protected override void Run(MsgPacakage pacakage)
        {
            try
            {
                M2Gate_RefreshActivityMessage msg = pacakage.msg as M2Gate_RefreshActivityMessage;
                if (msg.activitys == null)
                    return;

                List<ActivityInfo> rmAis = null;

                for (int i = 0; i < msg.activitys.Count; i++)
                {
                    ActivityInfo activity = msg.activitys[i];
                    if (activity.op == 2)
                    {
                        if (rmAis == null)
                            rmAis = new List<ActivityInfo>();
                        rmAis.Add(activity);
                        ActivityManagerComponent.Instance.Remove(activity.id);
                    }
                    else
                    {
                        ActivityManagerComponent.Instance.Refersh(activity);
                    }
                }
                if (rmAis != null)
                    PlayerManagerComponent.Instance.RemoveActivity(rmAis);

                Gate2C_RefreshActivityMessage gate2c = new Gate2C_RefreshActivityMessage();
                gate2c.activitys = msg.activitys;
                PlayerManagerComponent.Instance.SendMsgToAllPlayer(gate2c);
            }
            finally
            {
                pacakage.Clear();
            }
            
        }
    }
}
