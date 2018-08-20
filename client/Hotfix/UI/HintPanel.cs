using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix
{
    public class HintPanel : Frame
    {
        GameObject hintTemple;
        int duration;

        List<HintElem> hints = new List<HintElem>();
        Queue<string> hintContents = new Queue<string>();
        float passTime;

        public override void Init()
        {
            base.Init();
            ReferenceCollector rc = panel.GetComponent<ReferenceCollector>();
            hintTemple = rc.Get<GameObject>("sp_bottom");
            Animator animator = hintTemple.GetComponent<Animator>();
            AnimationClip[] infos = animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].name == "HintAnimation")
                {
                    duration = (int)(infos[i].length * 1000);
                }
            }
        }
        public override void Destroy()
        {
            base.Destroy();
            hintContents.Clear();
            hints.Clear();
        }
        public override void Show(object[] arg)
        {
            base.Show(arg);
            string content = (string)arg[0];
            hintContents.Enqueue(content);

        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (passTime > 0)
            {
                passTime -= Time.deltaTime;
            }
            if (hintContents.Count > 0 && passTime <= 0)
            {
                passTime = 0.5f;
                ShowHint(hintContents.Dequeue());
            }
        }
        async void ShowHint(string content)
        {
            HintElem hint = GetHint();
            hint.Refresh(content);

            await Task.Delay(duration);
            if (hint == null || hint.go == null)
            {
                return;
            }
            hint.go.SetActive(false);
            if (hints.Count < 5)
            {
                hints.Add(hint);
            }
            else
            {
                GameObject.Destroy(hint.go);
                hint = null;
            }
        }
        public HintElem GetHint()
        {
            HintElem elem = null;
            if (hints.Count > 0)
            {
                elem = hints[0];
                hints.RemoveAt(0);
            }
            else
            {
                GameObject go = GameObject.Instantiate<GameObject>(hintTemple);
                go.transform.SetParent(panel.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.localRotation = Quaternion.identity;
                elem = new HintElem(go);
            }

            return elem;
        }

        public class HintElem
        {
            Text txt_content;
            Animator animator;
            public GameObject go;
            public HintElem(GameObject go)
            {
                this.go = go;
                ReferenceCollector rc = go.GetComponent<ReferenceCollector>();
                txt_content = rc.Get<GameObject>("txt_content").GetComponent<Text>();
                animator = go.GetComponent<Animator>();
            }

            public void Refresh(string content)
            {
                go.SetActive(true);
                txt_content.text = content;
                animator.Play("HintAnimation", 0, 0);
                //SoundManager.Instance.PlaySound(SoundCategory.UI, "false");//播放出错音效
            }
        }
    }
}

