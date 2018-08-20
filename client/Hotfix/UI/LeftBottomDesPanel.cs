using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix
{
    public class LeftBottomDesPanel : Frame
    {
        public GameObject des_prefab;
        public Transform content;

        List<Text> des_gos = new List<Text>();
        int maxCount = 50;

        public override void Init()
        {
            base.Init();
            ReferenceCollector rc = panel.GetComponent<ReferenceCollector>();
            des_prefab = rc.Get<GameObject>("txt_des");
            content = rc.Get<GameObject>("content").transform;

            des_prefab.gameObject.SetActive(false);
            RectTransform rt_c = content as RectTransform;
            RectTransform rt_d = des_prefab.transform as RectTransform;
            rt_d.sizeDelta = new Vector2(rt_c.rect.width - 20, rt_d.sizeDelta.y);
        }
        public override void Destroy()
        {
            base.Destroy();
        }

        public void AddLeftBottomDesHandle(EventPackage package)
        {
            Text des_txt = null;
            if (des_gos.Count < maxCount)
            {
                GameObject npcGO = GameObject.Instantiate<GameObject>(des_prefab, content);
                des_txt = npcGO.GetComponent<Text>();
            }
            else
            {
                des_txt = des_gos[0];
                des_gos.RemoveAt(0);
            }
            des_gos.Add(des_txt);
            des_txt.text = (string)package.param3;
            if (des_txt.gameObject.activeSelf == false)
            {
                des_txt.gameObject.SetActive(true);
            }
            des_txt.transform.SetAsLastSibling();
        }
    }
}

