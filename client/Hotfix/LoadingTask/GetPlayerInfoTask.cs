using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Base;
//using Data;
namespace Hotfix
{
    public class GetPlayerInfoTask : LoadingTask
    {
        public GetPlayerInfoTask(float needTime, bool isAsyn, bool isDoAfterEnter = false) : base(needTime, isAsyn, isDoAfterEnter)
        {

        }
        public override async void Do()
        {
            //Player localPlayer = Game.Instance.LocalPlayer;

            //bool isSuccess = await localPlayer.GetPlayerInfo();

            //isSuccess = await localPlayer.EnterRoom((int)localPlayer.roomId);

            //SceneSkip.Instance.OneLoadingTaskFinish(this);
        }
    }
}