using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix
{
    public class MessagePanel : Frame
    {
        Text mContent;
        Text mButtonText;
        Text mTitle;
        System.Action mAction;
        System.Action mOnClose;
        public override void Init()
        {
            base.Init();
            ReferenceCollector rc = panel.GetComponent<ReferenceCollector>();
            rc.Get<GameObject>("btn_close").GetComponent<Button>().onClick.AddListener(() =>
            {
                Hide();
                if (null != mOnClose)
                    mOnClose();
            });
            mContent = rc.Get<GameObject>("content").GetComponent<Text>();
            mButtonText = rc.Get<GameObject>("buttonText").GetComponent<Text>();
            mTitle = rc.Get<GameObject>("titile").GetComponent<Text>();
            Button mButton = rc.Get<GameObject>("ok").GetComponent<Button>();
            mButton.onClick.AddListener(() =>
            {
                if (null != mAction)
                    mAction();
                Hide();
            });
        }
        public override void Show(object[] arg)
        {
            base.Show(arg);
            mTitle.text = (string)arg[0];
            mContent.text = (string)arg[1];
            mButtonText.text = (string)arg[2];
            mAction = (Action)arg[3];
            mOnClose = (Action)arg[4];
        }
    }
}

