using Base;
using Data;
using System.Collections.Generic;

namespace EventHandle
{
    [Event((int)EventIdType.ActivityTrigger, AppType.GateServer)]
    public class ActivityTriggerEvent : IEvent<ActivityType, Player, object, object, object>
    {
        public void Run(ActivityType activityType, Player player, object param1, object param2, object param3)
        {
            List<ActivityInfo> activityInfos = ActivityManagerComponent.Instance.FindByType(activityType);
            if (activityInfos != null)
            {
                Gate2C_ActivityAwardMessage msg2c = null;

                for (int i = 0; i < activityInfos.Count; i++)
                {
                    ActivityInfo activityInfo = activityInfos[i];
                    if (activityInfo == null)
                        continue;
                    if(activityInfo.StartTime > Game.Instance.Sec || Game.Instance.Sec > activityInfo.EndTime)
                    {
                        continue;
                    }
                    if(msg2c == null)
                    {
                        msg2c = new Gate2C_ActivityAwardMessage();
                        msg2c.id = player.Id;
                        msg2c.activityType = (int)activityType;
                        msg2c.awardInfo = new AwardInfo();
                    }
                    for (int j = 0; j < activityInfo.ress.awards.Count; j++)
                    {
                        AwardInfo award = activityInfo.ress.awards[j];
                        Helper.OverlayAward(award, msg2c.awardInfo);
                    }
                }

                if(msg2c != null)
                {
                    LogAction logAction = LogAction.None;
                    if (activityType == ActivityType.BingAgent)
                        logAction = LogAction.bing_getcard;
                    Helper.AddAward(player, msg2c.awardInfo, logAction);
                    //TranspondComponent.instance.ToClient(msg2c, player.Id);
                }
            }
        }
    }
}
