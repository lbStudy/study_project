using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(2691)]
    public class A2A_ServerPingMessageHandler : AMHandler<A2A_ServerPingMessage>
    {
        protected override void Run(MsgPacakage pacakage)
        {
            try
            {
                A2A_ServerPingMessage msg = pacakage.msg as A2A_ServerPingMessage;
                Log.Debug($"{ msg.fromApp } server ping.");
                pacakage.Source.pingTime = Game.Instance.Msec;
            }
            finally
            {
                pacakage.Clear();
            }
        }
    }
}
