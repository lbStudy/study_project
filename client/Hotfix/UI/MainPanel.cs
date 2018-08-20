using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Data;
//using Base;

namespace Hotfix
{
    public class MainPanel : Frame
    {
        public override void Init()
        {
            base.Init();


            //获得panel上的ReferenceCollector组件
            ReferenceCollector rc = panel.GetComponent<ReferenceCollector>();



        }

        public override void Show(object[] arg)
        {
            base.Show(arg);
            //SoundManager.Instance.PlaySound(SoundCategory.BGM, "BGM_JQ");//播放背景音乐

            UIManager.Instance.Show(FrameType.RoomPanel, null, ShowLayer.Layer_3);
            UIManager.Instance.Show(FrameType.LeftBottomDesPanel, null, ShowLayer.Layer_3);
            UIManager.Instance.Show(FrameType.RightDesPanel, null, ShowLayer.Layer_3);

            //GlobalEventManager.Trigger(GlobalEventType.AddLeftBottomDes, new EventPackage(0, 0, "今天天气真是热!"));
        }
    }
}

