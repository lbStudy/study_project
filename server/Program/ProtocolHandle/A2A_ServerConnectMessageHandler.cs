using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(23260)]
    public class A2A_ServerConnectMessageHandler : AMHandler<A2A_ServerConnectMessage>
    {
        protected override void Run(MsgPackage package)
        {
            A2A_ServerConnectMessage msg = package.msg as A2A_ServerConnectMessage;
            try
            {
                foreach(InnerConnectInfo v in msg.connectInfo)
                {
                    NetInnerComponent.Instance.InnerRegister(v.innerIp, v.innerPort, v.appId, (AppType)v.appType, v.system);
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
