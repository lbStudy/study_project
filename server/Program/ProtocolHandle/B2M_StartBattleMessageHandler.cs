using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(64069)]
    public class B2M_StartBattleMessageHandler : AMHandler<B2M_StartBattleMessage>
    {
        protected override void Run(MsgPacakage package)
        {
            B2M_StartBattleMessage msg = package.msg as B2M_StartBattleMessage;
            try
            {
                RoomInfo roomInfo = RoomAllotComponent.Instance.FindRoom(msg.roomid);
                if(roomInfo != null)
                {
                    roomInfo.canJoinTime = Game.Instance.Msec + (msg.gameTime / 2);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                package.Clear();
            }
        }
    }
}
