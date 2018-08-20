//using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Base;

namespace Hotfix
{
    public class PingDetection
    {
        public static PingDetection Instance
        {
            get
            {
                if (instance == null)
                    instance = new PingDetection();
                return instance;
            }
        }
        private static PingDetection instance;
        //C2Gate_PingMessage msg = new C2Gate_PingMessage();
        bool isConnecting;
        public long preSendTime = 0;
        int mRecconetTimes = 10;
        //async void Reconnect()
        //{
        //    //Debug.Log("1");
        //    //isConnecting = true;
        //    //if (mRecconetTimes > 0)
        //    //{
        //    //    Debug.Log("2");
        //    //    UIManager.Instance.Show(FrameType.ReconnectTipPanel);
        //    //    mRecconetTimes--;
        //    //    bool connected = await Game.Instance.Connect(Game.Instance.LocalPlayer.ip, Game.Instance.LocalPlayer.port);
        //    //    Debug.Log("2");
        //    //    isConnecting = false;
        //    //    if (connected)
        //    //    {
        //    //        Debug.Log("4");
        //    //        C2Gate_ReloginRequest req = new C2Gate_ReloginRequest();
        //    //        req.checkCode = Game.Instance.LocalPlayer.checkcode;
        //    //        req.playerid = Game.Instance.LocalPlayer.id;
        //    //        Gate2C_ReloginResponse resp = await ClientNetwork.Instance.Call(req) as Gate2C_ReloginResponse;
        //    //        Debug.Log("5");
        //    //        if (null != resp)
        //    //        {
        //    //            Debug.Log("6");
        //    //            UIManager.Instance.HideFrame(FrameType.ReconnectTipPanel);
        //    //            if (resp.errorCode != (int)ErrorCode.Success)
        //    //            {
        //    //                GoLogin();
        //    //            }
        //    //            else
        //    //            {
        //    //                Debug.Log("8");
        //    //                Game.Instance.LocalPlayer.GetAllInfo();
        //    //            }
        //    //        }
        //    //        else
        //    //            Reconnect();
        //    //    }
        //    //    else
        //    //    {

        //    //        Reconnect();

        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    UIManager.Instance.HideFrame(FrameType.ReconnectTipPanel);
        //    //    Game.Instance.ShowMessageBox("提示", "你的网络不佳，重连失败 ,请在恢复好网络后重新打开客户端", "好的大王", () =>
        //    //    {
        //    //        Application.Quit();
        //    //        isConnecting = false;
        //    //    });

        //    //}

        //}
        //public void DoHeart()
        //{
        //    isConnecting = false;
        //    //ClientNetwork.Instance.sendSession.Send(msg);
        //    preSendTime = TimeHelper.ClientNowSeconds();
        //}
        //public bool IsConnected
        //{
        //    get
        //    {
        //        return ClientNetwork.Instance.sendSession != null && !ClientNetwork.Instance.sendSession.IsDisposer && ClientNetwork.Instance.IsConnected;
        //    }
        //}

        //public void Ping()
        //{
        //    //if (Game.Instance.isConnect == false)
        //    //    return;
        //    long nowTime = TimeHelper.ClientNowSeconds();

        //    {

        //        if (IsConnected)
        //        {
        //            if (nowTime - preSendTime > 5)
        //            {
        //                DoHeart();
        //            }

        //        }
        //        else if (!isConnecting /*&& SceneSkip.Instance.curInScene != SceneType.Login*/)
        //        {
        //            Debug.Log("0");
        //            mRecconetTimes = 15;
        //            Reconnect();
        //        }

        //    }
        //}
        //void GoLogin()
        //{
        //    //SceneSkip.Instance.GoScene(SceneType.Login);
        //    //Game.Instance.Clear();
        //}

    }
}