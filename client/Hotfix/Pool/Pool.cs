using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix
{
    public class Pool
    {
        private GameObject prefab = null;
        private bool isDoing = false;
        private List<GameObject> cacheObj = new List<GameObject>();
        private List<Action<GameObject>> loadAFinishAction = new List<Action<GameObject>>();
        public string name;
        public int CurCount { get { return cacheObj.Count; } }
        public Pool(ref string name)
        {
            this.name = name;
        }
        public void Clear()
        {
            for (int i = 0; i < cacheObj.Count; i++)
            {
                if (cacheObj[i] != null)
                    UnityEngine.Object.Destroy(cacheObj[i]);
            }
            cacheObj.Clear();
            loadAFinishAction.Clear();
            prefab = null;
            isDoing = false;
        }
        public void InitAsyn(ref string path, int count, Action finishCallBall)
        {
            if (isDoing == false)
            {
                isDoing = true;
                string newPath = path + name;
                BundleManager.Instance.LoadAssetAsyn<GameObject>(newPath, true, (lodaObj) =>
                {
                    isDoing = false;
                    prefab = lodaObj;
                    if(prefab != null)
                    {
                        AddGo(count);
                    }
                    if (finishCallBall != null)
                        finishCallBall();
                });
            }
        }
        public void Init(ref string path, int count)
        {
            string newPath = path + name;
            prefab = BundleManager.Instance.LoadAsset<GameObject>(newPath);
            if (prefab != null)
            {
                AddGo(count);
            }
        }
        public void AsynTake(ref string path, Action<GameObject> action)
        {
            GameObject obj = null;
            if (cacheObj.Count > 0)
            {
                if (action != null)
                {
                    obj = cacheObj[0];
                    cacheObj.RemoveAt(0);
                    action(obj);
                }       
            }
            else if (prefab != null)
            {
                //allObject.Add(obj);
                if (action != null)
                {
                    obj = GameObject.Instantiate(prefab);
                    action(obj);
                }       
            }
            else
            {
                if (action != null)
                    loadAFinishAction.Add(action);
                if (!isDoing)
                {
                    isDoing = true;
                    string newPath = path + name;
                    BundleManager.Instance.LoadAssetAsyn<GameObject>(newPath, true, (objLoad) =>
                    {
                        isDoing = false;
                        prefab = objLoad;
                        GameObject go = null;
                        for (int i = 0; i < loadAFinishAction.Count; i++)
                        {
                            if (loadAFinishAction[i] != null)
                            {
                                if (prefab != null)
                                {
                                    go = GameObject.Instantiate(prefab);
                                    //allObject.Add(go);
                                    loadAFinishAction[i](go);
                                }
                                else
                                {
                                    loadAFinishAction[i](null);
                                }
                            }
                        }
                        loadAFinishAction.Clear();
                    });
                }
            }
        }
        public GameObject Take(ref string path)
        {
            GameObject go = null;
            //如果对象池有就取出一个
            if (cacheObj.Count > 0)
            {
                go = cacheObj[0];
                cacheObj.RemoveAt(0);
                go.SetActive(true);
            }
            //没有的话就创建出一个再取出
            else
            {
                if (prefab == null)
                {
                    string newPath = path + name;
                    prefab = BundleManager.Instance.LoadAsset<GameObject>(newPath);
                }
                if(prefab != null)
                {
                    go = GameObject.Instantiate(prefab) as GameObject;
                    //go.transform.localScale = Vector3.one;
                    //go.transform.rotation = Quaternion.identity;
                    go.name = name;
                }
            }
            if(go != null)
                go.transform.SetParent(null);
            return go;
        }
        public void Back(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(PoolManager.Instance.gameObject.transform);
            cacheObj.Add(obj);
        }
        public void AddGo(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject go = GameObject.Instantiate(prefab) as GameObject;
                go.name = name;
                Back(go);
            }
        }
    }
}
