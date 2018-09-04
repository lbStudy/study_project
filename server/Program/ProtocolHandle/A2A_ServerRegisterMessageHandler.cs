using Base;
using Data;
using System;
using System.Collections.Generic;

namespace ProtocolHandle
{
    [Protocol(65357)]
    public class A2A_ServerRegisterMessageHandler : AMHandler<A2A_ServerRegisterMessage>
    {
        protected override void Run(MsgPackage package)
        {
            A2A_ServerRegisterMessage msg = package.msg as A2A_ServerRegisterMessage;
            try
            {
                if(Game.Instance.AppType == AppType.LoginServer)
                {
                    if(msg.appType == (int)AppType.ManagerServer)
                    {
                        AreaInfo areaInfo = LoginManagerComponent.Instance.FindAreaInfo(msg.areaId);
                        if(areaInfo == null)
                        {
                            areaInfo = new AreaInfo();
                            areaInfo.id = msg.areaId;
                            areaInfo.name = msg.areaName;
                            areaInfo.isOpen = true;
                            areaInfo.enterCount = 0;
                            LoginManagerComponent.Instance.AddAreaInfo(areaInfo);
                        }
                        else
                        {
                            areaInfo.name = msg.areaName;
                        }
                    }
                }
                else if (Game.Instance.AppType == AppType.ManagerServer)
                {
                    List<InnerNetInfo> netInfos = null;
                    if (msg.appType == (int)AppType.GateServer)
                    {//通知已注册服务器连接网关
                        foreach(AppType appType in Enum.GetValues(typeof(AppType)))
                        {
                            if(ConstDefine.ActiveConnectGate.Contains(appType))
                            {
                                List<InnerNetInfo> netInfos2 = NetInnerComponent.Instance.FindByAppType(appType);
                                if (netInfos2 == null || netInfos2.Count == 0)
                                    continue;
                                if(netInfos == null)
                                    netInfos = new List<InnerNetInfo>();
                                netInfos.AddRange(netInfos2);
                            }
                        }
                        if (netInfos != null && netInfos.Count > 0)
                        {
                            A2A_ServerConnectMessage scMsg = ProtocolDispatcher.Instance.Take<A2A_ServerConnectMessage>();

                            InnerConnectInfo icInfo = new InnerConnectInfo();
                            icInfo.appId = msg.appId;
                            icInfo.appType = msg.appType;
                            icInfo.innerIp = msg.innerIp;
                            icInfo.innerPort = msg.innerPort;
                            icInfo.system = msg.system;

                            scMsg.connectInfo.Add(icInfo);

                            foreach (InnerNetInfo v in netInfos)
                            {
                                v.session.SendMessage(scMsg, NetHelper.GetSendId(null));
                            }

                            ProtocolDispatcher.Instance.Back(scMsg);
                        }

                    }
                    else
                    {//通知注册服务器连接网关
                        netInfos = NetInnerComponent.Instance.FindByAppType(AppType.GateServer);
                        if (netInfos != null && netInfos.Count > 0)
                        {
                            A2A_ServerConnectMessage scMsg = ProtocolDispatcher.Instance.Take<A2A_ServerConnectMessage>();
                            foreach (InnerNetInfo v in netInfos)
                            {
                                InnerConnectInfo icInfo = new InnerConnectInfo();
                                icInfo.appId = v.appId;
                                icInfo.appType = (int)v.appType;
                                icInfo.innerIp = v.innerIp;
                                icInfo.innerPort = v.innerPort;
                                icInfo.system = v.system;

                                scMsg.connectInfo.Add(icInfo);
                            }

                            package.Source.SendMessage(scMsg, NetHelper.GetSendId(null));

                            ProtocolDispatcher.Instance.Back(scMsg);
                        }
                    }
                }

                InnerNetInfo netInfo = NetInnerComponent.Instance.FindByAppId(msg.appId);
                if(netInfo == null)
                    netInfo = new InnerNetInfo();
                else
                {
                    if(netInfo.session != null && netInfo.session != package.Source)
                    {
                        netInfo.session.Dispose();
                    }
                }
                netInfo.appId = msg.appId;
                netInfo.appType = (AppType)msg.appType;
                netInfo.session = package.Source;
                netInfo.session.relevanceID = msg.appId;
                netInfo.system = msg.system;
                netInfo.innerIp = msg.innerIp;
                netInfo.innerPort = msg.innerPort;
                netInfo.outerIp = msg.outerIp;
                netInfo.outerPort = msg.outerPort;
                NetInnerComponent.Instance.Add(netInfo);
                Console.WriteLine($"{netInfo.appType.ToString()}({msg.appId}) inner register.");
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
