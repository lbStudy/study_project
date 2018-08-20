using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix
{
    public class TipsText : Frame
    {



        public override void Init()
        {
            base.Init();
            ReferenceCollector rc = panel.GetComponent<ReferenceCollector>();

        }


        public override void Show(System.Object[] arg)
        {
            base.Show(arg);

        }
    }
}

