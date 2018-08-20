using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix
{
    public enum FrameState
    {
        Loading,
        Hide,
        Show,
        Destroy
    }
    public class Frame
    {
        public GameObject panel;
        public GameObject prefab;
        public ShowLayer layer;
        public FrameType frameType;
        FrameState state;
        public FrameState State { get { return state; } }
        public virtual void Init()
        {

        }
        public virtual void FixedUpdate()
        {

        }
        public virtual void Show(System.Object[] arg)
        {
            panel.SetActive(true);
            state = FrameState.Show;
        }
        public virtual void Hide()
        {
            if (state == FrameState.Hide)
                return;
            if (null != panel)
                panel.SetActive(false);
            state = FrameState.Hide;
            UIManager.Instance.FrameAdjust(this);
        }
        public virtual void Destroy()
        {
            if (state == FrameState.Destroy)
                return;
            state = FrameState.Destroy;
            UIManager.Instance.FrameAdjust(this);
            if (null != panel)
            {
                panel.SetActive(false);
                GameObject.Destroy(panel);
            }
            prefab = null;
            panel = null;
        }
    }
}
