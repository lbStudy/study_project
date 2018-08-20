using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Data;
namespace Hotfix
{
    public class CharacterTopShowPanel : Frame
    {

        //List<CharacterTopInfo> topInfos = new List<CharacterTopInfo>();
        [SerializeField]
        public Vector3 offset = new Vector3(0, 1f, 0);

        public override void Show(object[] arg)
        {
            base.Show(arg);
        }

        public void Refresh()
        {
            //List<Character> characters = Room.Instance.Characters;
            //int index = 0;
            //CharacterTopInfo topInfo = null;
            //Character target;
            //Vector3 finalyOffset;
            //Vector3 pos;
            //for (int i = 0; i < characters.Count; i++)
            //{
            //    target = characters[i];
            //    if (target.Go == null || target.baseInfo.category == (int)CharacterCategory.Item|| target.state == CharacterState.Dead || target.isRemove)
            //        continue;
            //    finalyOffset = new Vector3(offset.x, offset.y + target.Height * target.Go.transform.localScale.y, offset.z);
            //    //从世界空间到视窗空间的变换位置
            //    pos = Room.Instance.CurMainCamera.WorldToViewportPoint(target.Go.transform.position + finalyOffset);
            //    if(pos.z > 0f && (pos.x > 0f && pos.x < 1f && pos.y > 0f && pos.y < 1f))
            //    {//在视野范围内
            //        if (Room.Instance.localHero.IsNearArea(target))
            //        {
            //            if (index < topInfos.Count)
            //            {
            //                topInfo = topInfos[index];
            //                if (!topInfo.gameObject.activeSelf)
            //                    topInfo.gameObject.SetActive(true);
            //            }
            //            else
            //            {
            //                GameObject topInfoPre = Resources.Load<GameObject>("CharacterTopInfo");
            //                GameObject go = GameObject.Instantiate<GameObject>(topInfoPre, panel.transform);
            //                topInfo = go.GetComponent<CharacterTopInfo>();
            //                topInfos.Add(topInfo);
            //            }
            //            //从视窗空间到世界空间的变换位置
            //            pos = UIManager.Instance.UICamera.ViewportToWorldPoint(pos);
            //            //变换位置从世界坐标到自身坐标
            //            pos = topInfo.transform.parent.InverseTransformPoint(pos);
            //            pos.z = 0;
            //            topInfo.transform.localPosition = pos;
            //            topInfo.Refrsh(target);
            //            index++;
            //        }
            //    }
            //}
            //for(int i = index; i < topInfos.Count; i++)
            //{
            //    topInfos[i].gameObject.SetActive(false);
            //}
        }

    }
}

