using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix
{
    public class LoadingPanel : Frame
    {
        public Slider slider;

        public override void Init()
        {
            base.Init();
            ReferenceCollector rc = panel.GetComponent<ReferenceCollector>();
            slider = rc.Get<GameObject>("slider").GetComponent<Slider>();
        }
        public override void Show(object[] arg)
        {
            base.Show(arg);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            float progress = 0;
            //if(SceneSkip.Instance.taskTotalTime < 0.00001 && SceneSkip.Instance.taskTotalTime > -0.00001)
            //{
            //    progress = 0.5f;
            //}
            //else
            //{
            //    progress = (SceneSkip.Instance.taskTotalTime - SceneSkip.Instance.SurplusTime()) / SceneSkip.Instance.taskTotalTime;
            //}
            //if (SceneSkip.Instance.async != null && SceneSkip.Instance.async.isDone)
            //    progress = progress + SceneSkip.Instance.async.progress / 2;
            //slider.value = progress;
        }
    }
}

