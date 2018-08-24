using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(75151)]
    public class C2Gate_TriggerActivityRequestHandler : AMRpcHandler<C2Gate_TriggerActivityRequest>
    {
        protected override void Run(RpcPackage package)
        {
            C2Gate_TriggerActivityRequest req = package.msg as C2Gate_TriggerActivityRequest;
            Gate2C_TriggerActivityResponse response = package.Response as Gate2C_TriggerActivityResponse;

            try
            {
                Player player = PlayerManagerComponent.Instance.Find(package.Toid);
                if (player == null)
                {
                    response.errorCode = (int)ErrorCode.NotExistPlayer;
                    return;
                }
                ActivityInfo activityInfo = ActivityManagerComponent.Instance.FindById(req.activityid);
                if(activityInfo == null)
                {
                    response.errorCode = (int)ErrorCode.NotExist;
                    return;
                }
                if (Game.Instance.Sec < activityInfo.StartTime || Game.Instance.Sec > activityInfo.EndTime)
                {
                    response.errorCode = (int)ErrorCode.Outside;
                    return;
                }
                ActivityType atype = (ActivityType)activityInfo.type;
                int nowDay = TimeHelper.TotalDays(Game.Instance.Sec);
                switch (atype)
                {
                    case ActivityType.Login:
                        {
                            if(activityInfo.ress == null 
                                || activityInfo.ress.awards == null 
                                || activityInfo.ress.awards.Count == 0 )
                            {
                                response.errorCode = (int)ErrorCode.Fail;
                                return;
                            }

                            PlayerActivityInfo pAcInfo = player.CommonData.FindActivity(req.activityid);
                            if (pAcInfo == null)
                            {
                                pAcInfo = new PlayerActivityInfo();
                                pAcInfo.changeParam1 = new System.Collections.Generic.List<int>();
                                pAcInfo.id = req.activityid;
                                player.CommonData.AddActivity(pAcInfo);
                            }
                            else
                            {
                                if(pAcInfo.changeParam1.Contains(nowDay))
                                {
                                    response.errorCode = (int)ErrorCode.Fail;
                                    return;
                                }
                            }
                            response.awardInfo = new AwardInfo();
                            for (int i = 0; i < activityInfo.ress.awards.Count; i++)
                            {
                                AwardInfo award = activityInfo.ress.awards[i];
                                Helper.OverlayAward(award, response.awardInfo);
                            }
                            pAcInfo.changeParam1.Add(nowDay);
                            player.CommonData.SynchroActivity();
                            Helper.AddAward(player, response.awardInfo, LogAction.loginactivity_getcard);
                        }
                        break;
                    case ActivityType.Day7Login:
                        {
                            //int cd = nowDay - player.TemporaryData.firstLoginDay;
                            //if (cd < 0 || cd > 6)
                            //{
                            //    response.errorCode = (int)ErrorCode.Outside;
                            //    return;
                            //}
                            if(player.CommonData.Is7DayLoginAward(nowDay))
                            {
                                response.errorCode = (int)ErrorCode.Fail;
                                return;
                            }
                            int index = player.CommonData.Award7DayLoginCount();
                            if (activityInfo.ress == null
                                || activityInfo.ress.awards == null
                                || activityInfo.ress.awards.Count < index)
                            {
                                response.errorCode = (int)ErrorCode.Fail;
                                return;
                            }
                            response.awardInfo = new AwardInfo();
                            AwardInfo award = activityInfo.ress.awards[index];    
                            Helper.OverlayAward(award, response.awardInfo);
                            player.CommonData.Add7DayLoginAward(nowDay);
                            response.param1 = player.CommonData.Finish7DayAwards;
                            Helper.AddAward(player, response.awardInfo, LogAction.day7activity_getcard);
                        }
                        break;
                    case ActivityType.ShareGame:
                        //{
                        //    int lastday = TimeHelper.TotalDays(player.CommonData.Lastsharetime);
                        //    if(lastday == nowDay)
                        //    {
                        //        response.errorCode = (int)ErrorCode.Fail;
                        //        return;
                        //    }
                        //    if (activityInfo.ress == null
                        //        || activityInfo.ress.awards == null
                        //        || activityInfo.ress.awards.Count == 0)
                        //    {
                        //        response.errorCode = (int)ErrorCode.Fail;
                        //        return;
                        //    }
                        //    response.awardInfo = new AwardInfo();
                        //    for (int i = 0; i < activityInfo.ress.awards.Count; i++)
                        //    {
                        //        AwardInfo award = activityInfo.ress.awards[i];
                        //        Helper.OverlayAward(award, response.awardInfo);
                        //    }
                        //    player.CommonData.Set(D_AttributeType.lastsharetime, Game.Instance.Sec, true);
                        //    player.CommonData.SynchroData();
                        //    Helper.AddAward(player, response.awardInfo, LogAction.shareactivity_getcard);
                        //}
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                response.errorCode = (int)ErrorCode.CodeError;
            }
            finally
            {
                package.Reply();
            }
        }
    }
}
