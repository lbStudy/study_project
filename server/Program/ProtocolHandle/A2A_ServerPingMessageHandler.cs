using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(2691)]
    public class A2A_ServerPingMessageHandler : AMHandler<A2A_ServerPingMessage>
    {
        protected override void Run(MsgPackage package)
        {
            try
            {
                A2A_ServerPingMessage msg = package.msg as A2A_ServerPingMessage;
                Log.Debug($"{ msg.fromApp } server ping.");
                package.Source.pingTime = Game.Instance.Msec;
            }
            finally
            {
                package.Dispose();
            }
        }
    }
}
