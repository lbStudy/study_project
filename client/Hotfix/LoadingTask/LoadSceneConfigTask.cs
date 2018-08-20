using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Config;
namespace Hotfix
{
    public class LoadSceneConfigTask : LoadingTask
    {

        // Use this for initialization
        public LoadSceneConfigTask(float needTime, bool isAsyn, bool isDoAfterEnter = false) : base(needTime, isAsyn, isDoAfterEnter)
        {

        }
        public override void Do()
        {

            //SceneSkip.Instance.OneLoadingTaskFinish(this);
            Debug.Log("finish load SceneConfig");
        }
    }

}
