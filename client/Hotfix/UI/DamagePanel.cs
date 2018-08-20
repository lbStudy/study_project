using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix
{
    public class DamagePanel : Frame
    {
        List<DamageElem> elems = new List<DamageElem>();
        Vector3 offset = new Vector3(0, 0, 0);

        public override void Init()
        {
            base.Init();

        }
        public override void Destroy()
        {
            base.Destroy();
            elems.Clear();
        }
        public void Damage(EventPackage package)
        {
            if (State != FrameState.Show)
                return;
            //Character target = package.param3 as Character;
            //long val = package.param1;
            //if (target.Go == null)
            //    return;
            //Vector3 finalyOffset = new Vector3(offset.x, offset.y + target.Height * target.Go.transform.localScale.y, offset.z);
            ////从世界空间到视窗空间的变换位置
            //Vector3 pos = Room.Instance.CurMainCamera.WorldToViewportPoint(target.Go.transform.position + finalyOffset);
            //if (pos.z > 0f && (pos.x > 0f && pos.x < 1f && pos.y > 0f && pos.y < 1f))
            //{//在视野范围内
            //    if (Room.Instance.localHero.IsNearArea(target))
            //    {
            //        DamageElem elem;
            //        if (elems.Count > 0)
            //        {
            //            elem = elems[0];
            //            elems.RemoveAt(0);
            //        }
            //        else
            //        {
            //            GameObject template = (GameObject)Resources.Load("Damage");
            //            GameObject go = GameObject.Instantiate<GameObject>(template, panel.transform);
            //            elem = new DamageElem(go);
            //        }
            //        //从视窗空间到世界空间的变换位置
            //        pos = UIManager.Instance.UICamera.ViewportToWorldPoint(pos);
            //        //变换位置从世界坐标到自身坐标
            //        pos = panel.transform.InverseTransformPoint(pos);
            //        pos.z = 0;
            //        elem.go.transform.localPosition = pos;
            //        elem.Refresh(val.ToString());
            //        ShowHint(elem);
            //    }
            //}
        }
        async void ShowHint(DamageElem elem)
        {
            await Task.Delay(elem.duration);
            if (State == FrameState.Destroy || panel == null || elem.go == null)
            {
                return;
            }
            elem.Hide();
            elems.Add(elem);
        }



        public class DamageElem
        {
            public GameObject go;
            Text txt_val;
            Animator animator;

            public int duration;
            public DamageElem(GameObject go)
            {
                this.go = go;
                txt_val = go.GetComponentInChildren<Text>();
                animator = go.GetComponentInChildren<Animator>();

                AnimationClip[] infos = animator.runtimeAnimatorController.animationClips;
                for (int i = 0; i < infos.Length; i++)
                {
                    if (infos[i].name == "DamageAni")
                    {
                        duration = (int)(infos[i].length * 1000);
                    }
                }
            }

            public void Refresh(string val)
            {
                go.SetActive(true);
                txt_val.text = val;
                animator.Play("DamageAni", 0, 0);
            }
            public void Hide()
            {
                go.SetActive(false);
            }
        }
    }
}

