using Base;
using Data;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Hotfix
{
    public static class Game
    {
        public static void Init()
        {
            Debug.Log("game init");
            ConfigDataManager.Instance.Load();
        }

        public static void Update()
        {
            if(Input.GetKeyDown(KeyCode.A))
            {
                Login(0, "123", "123");
            }
        }

        public static void LateUpdate()
        {

        }

        public static void FixedUpdate()
        {

        }
        public static System.Diagnostics.Stopwatch totalwatch = new System.Diagnostics.Stopwatch();

        public static void RestartRunTimeDec()
        {
            totalwatch.Restart();
        }
        public static void EndRunTimeDec(string str)
        {
            TimeSpan timespan = totalwatch.Elapsed;  //获取当前实例测量得出的总时间
            totalwatch.Stop();
            Debug.Log($"{str}：{timespan.TotalMilliseconds}(毫秒)");
        }
        public static async void Login(int mChannel, string accout, string pwd)
        {
            //if (string.IsNullOrEmpty(accout) || string.IsNullOrEmpty(pwd))
            //{
            //    UIManager.Instance.Show(FrameType.HintPanel, new System.Object[] { "用户名或者密码不能为空" }, ShowLayer.Layer_5);
            //    return;
            //}
            //UIManager.Instance.Show(FrameType.LoginTipPanel, null, ShowLayer.Layer_5);
            RestartRunTimeDec();
            bool connected = await ClientNetwork.Instance.Connect(SessionType.Login, Begin.Instance.ip, Begin.Instance.port);
            if (connected)
            {
                Debug.Log("登录请求");
                C2L_LoginRequest req = new C2L_LoginRequest();
                req.account = accout;
                req.password = pwd;
                ClientNetwork.Instance.Send(req, LoginResponse, SessionType.Login);
            }
            else
            {
                Debug.Log("无法连接登录服务器");
                //UIManager.Instance.Find(FrameType.LoginTipPanel).Hide();
                //ShowMessageBox("提示", "无法连接服务器", "好的大王", null);
            }
        }
        static long playerid;
        static void LoginResponse(object resp)
        {
            Debug.Log("登录响应");
            L2C_LoginResponse response = resp as L2C_LoginResponse;
            if (response.errorCode == (int)ErrorCode.Success)
            {
                playerid = response.id;
                C2L_EnterAreaRequest req = new C2L_EnterAreaRequest();
                req.areaid = response.areas[0].id;
                ClientNetwork.Instance.Send(req, EnterAreaResponse, SessionType.Login);
            }
            else
            {
                //UIManager.Instance.Find(FrameType.LoginTipPanel).Hide();
                //ShowMessageBox("提示", "登录失败 id:" + response.errorCode, "好的", null);
            }
        }
        async static void EnterAreaResponse(object resp)
        {
            
            L2C_EnterAreaResponse response = resp as L2C_EnterAreaResponse;
            if (response.errorCode == (int)ErrorCode.Success)
            {
                ClientNetwork.Instance.Disconnect(SessionType.Login);
                Debug.Log("连接网关");
                bool connected = await ClientNetwork.Instance.Connect(SessionType.Gate, response.gateip, response.gateport);
                if (connected)
                {
                    C2Gate_PlayerGateVerifyRequest req = new C2Gate_PlayerGateVerifyRequest();
                    req.checkcode = response.checkcode;
                    req.id = playerid;
                    ClientNetwork.Instance.Send(req, GateVerifyResponse);
                }
                else
                {
                    Debug.Log("无法连接网关服务器");
                    //UIManager.Instance.Find(FrameType.LoginTipPanel).Hide();
                    //ShowMessageBox("提示", "服务器维护中", "好的大王", null);
                }
            }
        }
        static void GateVerifyResponse(object response)
        {
            Debug.Log("网关验证响应");
            Gate2C_PlayerGateVerifyResponse resp = response as Gate2C_PlayerGateVerifyResponse;
            Frame f = UIManager.Instance.Find(FrameType.LoginTipPanel);
            if (null != f)
                f.Hide();
            if (resp.errorCode == (int)ErrorCode.Success)
            {
                EndRunTimeDec("完成游戏");
                //LoadingTask lt = new GetPlayerInfoTask(1, false);
                //SceneSkip.Instance.AddTaskLoading(lt);
                //SceneSkip.Instance.GoScene(SceneType.Main);
                //localPlayer.GetAllInfo();
                //Debug.Log($"login success");
            }
            else
            {
                //ShowMessageBox("提示", "获取用户数据失败", "好的大王", null);
            }
        }
    }
}
