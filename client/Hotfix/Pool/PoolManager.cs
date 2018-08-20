using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Hotfix
{
    public class PoolManager : Singleton<PoolManager>
    {
        private Dictionary<string, Pool> poolDic = new Dictionary<string, Pool>();
        private GameObject gameObject;
        
        void Init()
        {
            gameObject = new UnityEngine.GameObject("poolmanager");
        }
        protected override void OnDestroy()
        {
            ClearAllPool();
        }
        public void ClearAllPool()
        {
            foreach (KeyValuePair<string, Pool> pair in poolDic)
            {
                pair.Value.Clear();
            }
            poolDic.Clear();
        }
        public void AsynTake(string name, string path, Action<GameObject> action)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            Pool pool = null;
            if (poolDic.TryGetValue(name, out pool))
            {
                pool.AsynTake(ref path, action);
            }
            else
            {
                pool = new Pool(ref name);
                poolDic.Add(name, pool);
                pool.AsynTake(ref path, action);
            }
        }
        public void Back(string name, GameObject obj)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            Pool pool = null;

            if (poolDic.TryGetValue(name, out pool))
            {
                pool.Back(obj);
            }
        }
        public GameObject Take(string path, string name)
        {
            GameObject go = null;
            Pool pool = null;

            //从预制体里取出一个对象
            if (poolDic.TryGetValue(name, out pool))
            {
                go = pool.Take(ref path);
            }
            //否则新建一个并且取出
            else
            {
                pool = new Pool(ref name);
                poolDic.Add(name, pool);
                go = pool.Take(ref path);
            }
            return go;
        }
        public void CreatePool(string path, string name, int initCount)
        {
            Pool pool = null;
            if (poolDic.TryGetValue(name, out pool))
            {
                int count = pool.CurCount < initCount ? initCount - pool.CurCount : initCount;
                pool.AddGo(count);
            }
            else
            {
                pool = new Pool(ref name);
                pool.Init(ref path, initCount);
                poolDic.Add(name, pool);
            }
        }
        public void AsynCreatePool(string path, string name, int initCount, Action finishCallBall)
        {
            Pool pool = null;
            if (poolDic.TryGetValue(name, out pool))
            {
                int count = pool.CurCount < initCount ? initCount - pool.CurCount : 0;
                pool.AddGo(count);
                if (finishCallBall != null)
                    finishCallBall();
            }
            else
            {
                pool = new Pool(ref name);
                pool.InitAsyn(ref path, initCount, finishCallBall);
                poolDic.Add(name, pool);
            }
        }
    }
}
