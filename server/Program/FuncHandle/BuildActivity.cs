using System.Collections.Generic;
using Base;
using Data;

namespace FuncHandle
{
    [Function((int)FunctionId.BuildActivity)]
    public class BuildActivity : IFunc_R<List<ActivityInfo>>
    {
        public List<ActivityInfo> Run()
        {
            List<ActivityInfo> activitys = new List<ActivityInfo>();

            if(ConstConfigComponent.ConstConfig.Activity_login != null && ConstConfigComponent.ConstConfig.Activity_login.Count > 0)
            {
                ActivityInfo info = Analysis(ConstConfigComponent.ConstConfig.Activity_login, ActivityType.Login);
                if(info != null)
                    activitys.Add(info);
            }

            if (ConstConfigComponent.ConstConfig.Activity_7daylogin != null && ConstConfigComponent.ConstConfig.Activity_7daylogin.Count > 0)
            {
                ActivityInfo info = Analysis(ConstConfigComponent.ConstConfig.Activity_7daylogin, ActivityType.Day7Login);
                if (info != null)
                    activitys.Add(info);
            }

            if (ConstConfigComponent.ConstConfig.Activity_share != null && ConstConfigComponent.ConstConfig.Activity_share.Count > 0)
            {
                ActivityInfo info = Analysis(ConstConfigComponent.ConstConfig.Activity_share, ActivityType.ShareGame);
                if (info != null)
                    activitys.Add(info);
            }

            if (ConstConfigComponent.ConstConfig.Activity_bing != null && ConstConfigComponent.ConstConfig.Activity_bing.Count > 0)
            {
                ActivityInfo info = Analysis(ConstConfigComponent.ConstConfig.Activity_bing, ActivityType.BingAgent);
                if (info != null)
                    activitys.Add(info);
            }

            if (ConstConfigComponent.ConstConfig.Activity_daili != null && ConstConfigComponent.ConstConfig.Activity_daili.Count > 0)
            {
                ActivityInfo info = Analysis(ConstConfigComponent.ConstConfig.Activity_daili, ActivityType.Cooperation);
                if (info != null)
                    activitys.Add(info);
            }

            return activitys;
        }

        public ActivityInfo Analysis(List<string> ds, ActivityType actype)
        {
            try
            {
                ActivityInfo info = new ActivityInfo();
                info.id = int.Parse(ds[0]);
                info.op = int.Parse(ds[1]);
                info.type = (int)actype;
                info.startTime = ds[2];
                info.endTime = ds[3];
                info.des = ds[4];
                info.ress = new ResInfo();
                info.ress.awards = new List<AwardInfo>();
                for (int i = 5; i < ds.Count; i += 2)
                {
                    if (i + 1 < ds.Count)
                    {
                        AwardInfo aw = new AwardInfo();
                        aw.itemids = new List<int>();
                        aw.itemids.Add(int.Parse(ds[i]));
                        aw.counts = new List<int>();
                        aw.counts.Add(int.Parse(ds[i + 1]));
                        info.ress.awards.Add(aw);
                    }
                }
                return info;
            }
            catch
            {
                return null;
            }
        }
    }
}
