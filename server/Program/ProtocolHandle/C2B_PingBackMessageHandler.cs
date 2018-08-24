using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(91135)]
    public class C2B_PingBackMessageHandler : AMHandler<C2B_PingBackMessage>
    {
        protected override void Run(MsgPackage package)
        {
            C2B_PingBackMessage msg = package.msg as C2B_PingBackMessage;
            try
            {
                if (package.Toid > 0 && msg.roomid > 0)
                {
                    Room room = RoomManagerComponent.Instance.Find(msg.roomid);
                    if (room == null)
                    {
                        package.Source.Dispose();
                        return;
                    }
                    RoomMember member = room.Find(package.Toid);
                    if (member == null)
                    {
                        package.Source.Dispose();
                        return;
                    }
                    long cz = msg.curClienTime - member.clientTime;
                    if(cz < 0)
                    {
                        package.Source.Dispose();
                        return;
                    }
                    member.delay = (long)(cz * 0.5f);
                }
                else
                {
                    package.Source.Dispose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                package.Dispose();
            }
        }
    }
}
