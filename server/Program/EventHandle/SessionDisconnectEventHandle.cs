using Base;
using Data;
using System.Threading.Tasks;

namespace EventHandle
{
    [Event((int)Base.InnerEventIdType.OuterSessionDisconnect, AppType.GateServer)]
    public class Gate_SessionDisconnectEventHandle : IEvent<Session>
    {
        public async void Run(Session session)
        {
            if (session.relevanceID > 0)
            {
                TranspondComponent.instance.ClientDisconnect(session.relevanceID);
                Player player = PlayerManagerComponent.Instance.Find(session.relevanceID);
                if (player != null)
                {
                    player.SetState(PlayerState.Offline);
                    player.TemporaryData.offlineTime = Game.Instance.Msec;

                        
                    //if (player.TemporaryData.roomid > 0)
                    //{
                    //    Gate2B_PlayerOfflineMessage msgToB = new Gate2B_PlayerOfflineMessage();
                    //    msgToB.id = session.relevanceID;
                    //    msgToB.roomid = player.TemporaryData.roomid;
                    //    Session battleSession = NetInnerComponent.Instance.GetByAppID(player.TemporaryData.battleAppid);
                    //    battleSession.Send(msgToB);
                    //}
                }
                Gate2M_PlayerOfflineMessage msgToM = new Gate2M_PlayerOfflineMessage();
                msgToM.id = session.relevanceID;
                Session managerSession = NetInnerComponent.Instance.GetByAppID(ServerConfigComponent.Instance.ManagerAppId);
                managerSession.SendMessage(msgToM, 0);
                Log.Debug($"gate玩家离线 : {session.relevanceID}");
            }   
        }
    }

    [Event((int)Base.InnerEventIdType.OuterSessionDisconnect, AppType.LoginServer)]
    public class Login_SessionDisconnectEventHandle : IEvent<Session>
    {
        public void Run(Session session)
        {
            if (session.relevanceID > 0)
            {
                LoginInfo info = LoginManagerComponent.Instance.FindLoginInfoById(session.relevanceID);
                if(info != null)
                {
                    if(!info.IsInGame && info.state != LoginState.Entering && info.session == session)
                        LoginManagerComponent.Instance.RemoveLoginInfo(session.relevanceID);
                    if(info.session == session)
                        info.state = LoginState.None;
                }
            }
        }
    }

    [Event((int)Base.InnerEventIdType.OuterSessionDisconnect, AppType.BattleServer)]
    public class Battle_SessionDisconnectEventHandle : IEvent<Session>
    {
        public void Run(Session session)
        {
            if (session.relevanceID > 0)
            {
                
            }
        }
    }
}
