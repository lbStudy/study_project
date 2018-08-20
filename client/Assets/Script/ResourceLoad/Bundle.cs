using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.IO;
using UnityEngine.Events;

/// <summary>
/// 资源包, Tip:不能对同一个资源进行异步读取的同时又进行同步读取
/// </summary>
public class Bundle : CustomYieldInstruction
{
    
    private AssetBundleCreateRequest req;
    private List<Bundle> dependencies = new List<Bundle>();
    private int refCount;
    private AssetBundle assetBundle;
    public string bundleName { get;}

    private Dictionary<string, List<UnityAction<UnityEngine.Object>>> callbackDic;
    private Dictionary<string, WeakReference> assets = new Dictionary<string, WeakReference>();//所有被直接使用的资源
    /// <summary>
    /// Is the load already finished? (Read Only)
    /// </summary>
    public bool isDone
    {
        get
        {
            if (req == null)
                return true;
            if(req.isDone)
            {
                if (assetBundle == null)
                    assetBundle = req.assetBundle;
                if (this.dependencies.Count > 0)
                {
                    for (int i = 0; i < this.dependencies.Count; i++)
                    {
                        if (this.dependencies[i].isDone == false)
                            return false;
                    }
                }
            }

            return req.isDone;
        }
    }

    public override bool keepWaiting
    {
        get
        {
            return isDone == false;
        }
    }

    public Bundle( string bundleName)
    {
        this.bundleName = bundleName;
    }
    public void Load(string path, bool isAsyn)
    {
        if (isAsyn)
            req = AssetBundle.LoadFromFileAsync(path);
        else
            assetBundle = AssetBundle.LoadFromFile(path);
    }
    public bool IsRemoved()
    {
        if(isDone && refCount == 1 && (callbackDic == null || callbackDic.Count == 0))
        {
            foreach(WeakReference wr in assets.Values)
            {
                if(wr.IsAlive)
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    public void AddRef()
    {
        refCount++;
    }
    public void DelRef()
    {
        refCount--;
    }
    public void AddDependency(Bundle bundle)
    {
        dependencies.Add(bundle);
    }
    public void Unload(bool isAll)
    {
        if(assetBundle != null)
        {
            assetBundle.Unload(isAll);
        }
        req = null;
        assetBundle = null;
        for(int i = 0; i < dependencies.Count; i++)
        {
            dependencies[i].DelRef();
        }
        dependencies.Clear();
        BundleManager.Instance.RemoveBundle(this);
        Debug.Log($"bundle is removed : {bundleName}");
    }
    public T ReadAsset<T>(ref string assetName) where T : UnityEngine.Object
    {
        T asset = null;
        WeakReference wr = null;
        string name = Path.GetFileNameWithoutExtension(assetName);
        if (assets.TryGetValue(name, out wr))
        {
            if (wr.IsAlive)
            {
                return wr.Target as T;
            }
        }
        if (assetBundle == null)
        {
#if UNITY_EDITOR
            asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>("Assets/" + assetName);
#endif
        }
        else
        {
            asset = assetBundle.LoadAsset<T>(name);
        }
        if (asset == null)
        {
            Debug.Log($"Not exist asset : {name}");
            return null;
        }
        wr = new WeakReference(asset);
        assets[name] = wr;
        return asset;
    }
    public void ReadAssetAsyn<T>(ref string assetName, UnityAction<UnityEngine.Object> callback) where T : UnityEngine.Object
    {
        WeakReference wr = null;
        string name = Path.GetFileNameWithoutExtension(assetName);
        if (assets.TryGetValue(name, out wr))
        {
            if (wr.IsAlive)
            {
                callback(wr.Target as T);
                return;
            }
        }
        if(assetBundle == null)
        {
#if UNITY_EDITOR
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>("Assets/" + assetName);
            if (asset == null)
            {
                Debug.Log($"Not exist asset : {name}");
            }
            else
            {
                wr = new WeakReference(asset);
                assets[name] = wr;
            }
            callback(asset);
#endif
        }
        else
        {
            List<UnityAction<UnityEngine.Object>> callbacks = null;
            if (callbackDic == null)
            {
                callbackDic = new Dictionary<string, List<UnityAction<UnityEngine.Object>>>();
                callbacks = new List<UnityAction<UnityEngine.Object>>();
                callbackDic.Add(name, callbacks);
            }
            else
            {
                if (callbackDic.TryGetValue(name, out callbacks) == false)
                {
                    callbacks = new List<UnityAction<UnityEngine.Object>>();
                    callbackDic.Add(name, callbacks);
                }
            }
            callbacks.Add(callback);
            if (callbacks.Count == 1)
            {
                AssetBundleRequest req = assetBundle.LoadAssetAsync<T>(name);
                BundleManager.ReadAssetAsyn(ref name, this, req);
            }
        }
    }
    public void AsynReadFinish(ref string assetName, AssetBundleRequest req)
    {
        List<UnityAction<UnityEngine.Object>> callbacks = null;
        if (callbackDic.TryGetValue(assetName, out callbacks))
        {
            for (int i = 0; i < callbacks.Count; i++)
            {
                if (callbacks[i] != null)
                {
                    callbacks[i](req.asset);
                }
            }
            callbackDic.Remove(assetName);
        }
        if(req.asset == null)
        {
            Debug.Log($"Not exist asset : {assetName}");
        }
        else
        {
            WeakReference wr = new WeakReference(req.asset);
            assets[assetName] = wr;
        }
    }
}