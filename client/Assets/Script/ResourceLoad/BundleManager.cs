using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Net;

//资源加载路径
public enum AssetLoadMode
{
    Editor,             //仅在编辑器下使用，不需要打bundle,适用于开发阶段实时测试运行
    StreamingAssets,    //可以在编辑器或者发布平台使用，需要打bundle，并且将所有bundle拷贝到StreamingAssets文件夹下
    Publish             //可以在编辑器或者发布平台使用，需要从资源服务器下载或者更新资源
}
public enum BundleState
{
    Init,
    WaitUpdate,
    Updating,
    Finish,
    Fail
}
public struct BundleVersions
{
    public string path;
    public string md5;
    public long size;//单位字节


    public BundleVersions(string path, string md5, long size)
    {
        this.path = path;
        this.md5 = md5;
        this.size = size;
    }
}
public class BundleManager : BehaviourSingleton<BundleManager>
{
    private Dictionary<string, Bundle> bundleDic = new Dictionary<string, Bundle>();
    private List<Bundle> bundles = new List<Bundle>();
    private Dictionary<string, List<UnityAction<Bundle>>> callbackDic = new Dictionary<string, List<UnityAction<Bundle>>>();
    private AssetBundleManifest bundleManifest = null;
    private string assetRootPath;
    public string AssetRootPath { get { return assetRootPath; } }
    private AssetLoadMode assetLoadMode;
    BundleState state;
    public BundleState State { get { return state; } }
    public string updateUrl = "";

    long totalUpdateSize;
    public long TotalUpdateSize { get { return totalUpdateSize; } }
    long curUpdateSize;
    public long CurUpdateSize { get { return curUpdateSize; } }

    Queue<BundleVersions> updateBVs = new Queue<BundleVersions>();
    FileStream writefs;
    StreamWriter write;
    Queue<byte[]> cacheBytes = new Queue<byte[]>();
    int curUpdatingCount;

    const string abStr = "_ab";
    const string ab_sg_str = "_ab_sg";
    public string ManifestName
    {
        get
        {
#if UNITY_EDITOR
            return "StandaloneWindows";
#elif UNITY_STANDALONE
            return "StandaloneWindows";
#elif UNITY_ANDROID
            return "Android";
#elif UNITY_IPHONE
            return "iOS";
#endif
        }
    }
    public void Init(string assetRootPath, string updateUrl, AssetLoadMode assetLoadMode)
    {
        state = BundleState.Init;
        this.assetRootPath = assetRootPath;
        this.assetLoadMode = assetLoadMode;
        this.updateUrl = updateUrl + ManifestName;
        if (assetLoadMode == AssetLoadMode.Publish)
        {//(1)版本检测(2)版本更新(3)LoadBundleManifest
            this.assetRootPath = assetRootPath + ManifestName + "/";
            StartCoroutine(UpdateDec());
        }
        else if(assetLoadMode == AssetLoadMode.StreamingAssets)
        {
            LoadBundleManifest();
        }
        else
        {
            state = BundleState.Finish;
        }
        StartCoroutine(BundleDec());
    }

    IEnumerator UpdateDec()
    {//(1)删除不存在的bundle(2)更新md5改变的bundle(3)添加新增bundle
        if (Directory.Exists(assetRootPath) == false)
        {//创建bundle文件夹
            Directory.CreateDirectory(assetRootPath);
        }
        //下载更新文件
        string url = $"{updateUrl}\\{ManifestName}.txt";
        Debug.Log($"url : {url}");
        WWW www = new WWW(url);
        yield return www;
        if(string.IsNullOrEmpty(www.error) == false)
        {
            state = BundleState.Fail;
            Debug.Log($"update file load fail. from url : {url}. error : {www.error}");
        }
        else
        {
            Dictionary<string, BundleVersions> newBVDic = new Dictionary<string, BundleVersions>();

            string[] bundleLines = www.text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach(string line in bundleLines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                string[] strs = line.Split(' ');
                BundleVersions bv = new BundleVersions(strs[0], strs[1], long.Parse(strs[2]));
                newBVDic.Add(bv.path, bv);
            }

            Dictionary<string, BundleVersions> oldBVDic = new Dictionary<string, BundleVersions>();
            Dictionary<string, BundleVersions> removeBVDic = new Dictionary<string, BundleVersions>();
            string updataFilePath = assetRootPath + ManifestName + ".txt";
            if (File.Exists(updataFilePath))
            {//本地存在更新文件,比对文件
                try
                {
                    FileStream readfs = new FileStream(updataFilePath, FileMode.Open, FileAccess.Read);
                    StreamReader read = new StreamReader(readfs, Encoding.Default);

                    string strReadline;

                    while ((strReadline = read.ReadLine()) != null)
                    {
                        string[] strs = strReadline.Split(' ');
                        BundleVersions bv = new BundleVersions(strs[0], strs[1], long.Parse(strs[2]));
                        BundleVersions newBV;
                        if (newBVDic.TryGetValue(bv.path, out newBV))
                        {//存在bundle，比较md5
                            if (newBV.md5 == bv.md5)
                            {
                                oldBVDic.Add(bv.path, bv);
                            }
                            else
                            {
                                removeBVDic.Add(bv.path, bv);
                            }
                        }
                        else
                        {
                            removeBVDic.Add(bv.path, bv);
                        }
                    }
                    read.Close();
                    readfs.Close();
                }
                catch(Exception e)
                {
                    state = BundleState.Fail;
                    Debug.LogError(e.ToString());
                }
            }
            //移除改变或者不存在的bundle
            RemvoeChangeBundle(removeBVDic);
            //更新本地文件
            writefs = new FileStream(updataFilePath, FileMode.Create, FileAccess.Write);
            write = new StreamWriter(writefs, Encoding.Default);

            foreach(BundleVersions val in oldBVDic.Values)
            {
                WriteUpdateFile(val, false);
            }

            write.Flush();
            writefs.Flush();

            //添加到更新列表
            foreach (BundleVersions val in newBVDic.Values)
            {
                if(oldBVDic.ContainsKey(val.path))
                {
                    continue;
                }
                updateBVs.Enqueue(val);
                totalUpdateSize += val.size;
            }
            if(updateBVs.Count > 0)
            {
                curUpdateSize = 0;
                state = BundleState.WaitUpdate;
            }
            else
            {
                CloseUpdateFile();
                LoadBundleManifest();
            }
        }
    }
    void CloseUpdateFile()
    {
        if(write != null)
        {
            write.Close();
            write = null;
        }
        if(writefs != null)
        {
            writefs.Close();
            writefs = null;
        }
    }
    void WriteUpdateFile(BundleVersions bv, bool isFlush)
    {
        string content = bv.path + " " + bv.md5 + " " + bv.size;
        write.WriteLine(content);
        if(isFlush)
        {
            write.Flush();
            writefs.Flush();
        }
    }
    byte[] GetCacheBytes()
    {
        if (cacheBytes.Count > 0)
            return cacheBytes.Dequeue();
        return new byte[8 * 1024];
    }
    void AddCacheBytes(byte[] cacheByte)
    {
        cacheBytes.Enqueue(cacheByte);
    }
    public void StartUpdateBundleFromServer()
    {
        curUpdatingCount = 0;
        state = BundleState.Updating;
        System.Net.ServicePointManager.DefaultConnectionLimit = 200;
        Debug.Log($"total need update : {updateBVs.Count} bundle.");
        for (int i = 0; i < 5; i++)
        {
            UpdateBundleFromServer();
        }
    }
    void FinishUpdate()
    {
        LoadBundleManifest();
        cacheBytes.Clear();
    }
    async void UpdateBundleFromServer()
    {
        if (updateBVs.Count == 0)
        {
            if(curUpdatingCount == 0 && updateBVs.Count == 0)
            {
                FinishUpdate();
            }
            return;
        }
            
        BundleVersions bv = updateBVs.Dequeue();
        string assetPath = $"{assetRootPath}/{bv.path}";
        string dirPath = Path.GetDirectoryName(assetPath);
        if(Directory.Exists(dirPath) == false)
        {
            Directory.CreateDirectory(dirPath);
        }

        FileStream fs = new FileStream(assetPath, FileMode.Create);
        curUpdatingCount++;
        byte[] buffer = GetCacheBytes();
        try
        {
            string url = $"{updateUrl}/{bv.path}";
            Debug.Log("req bundle url : " + url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            WebResponse wresp = request.GetResponse();
            long fileSizeAll = wresp.ContentLength;
            Stream ns = wresp.GetResponseStream();
            int readSize = 0;
            int curSize = 0;
            while (curSize < fileSizeAll)
            {
                readSize = await ns.ReadAsync(buffer, 0, buffer.Length);
                await fs.WriteAsync(buffer, 0, readSize);
                curSize += readSize;
            }
            fs.Close();
            ns.Close();
            wresp.Close();
            WriteUpdateFile(bv, true);
            curUpdateSize += bv.size;
            curUpdatingCount--;
            AddCacheBytes(buffer);
            Debug.Log("finish bundle update : " + url);
            UpdateBundleFromServer();
        }
        catch(Exception e)
        {
            fs.Close();
            curUpdatingCount--;
            AddCacheBytes(buffer);
            //出现异常重新下载
            updateBVs.Enqueue(bv);
            //UpdateBundleFromServer();
            Debug.LogError(e.ToString());
        }
    }
    void RemvoeChangeBundle(Dictionary<string, BundleVersions> removeBVDic)
    {
        foreach(string path in removeBVDic.Keys)
        {
            if(File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    IEnumerator BundleDec()
    {
        List<Bundle> removeList = new List<Bundle>();
        while(true)
        {
            yield return new WaitForSeconds(3);
            if (state != BundleState.Finish)
                continue;
            yield return Resources.UnloadUnusedAssets();
            GC.Collect();
            yield return new WaitForEndOfFrame();
            foreach (Bundle bundle in bundles)
            {
                if(bundle.IsRemoved())
                {
                    removeList.Add(bundle);
                }
            }

            if(removeList.Count > 0)
            {
                foreach(Bundle bundle in removeList)
                {
                    RemoveBundle(bundle);
                }

                foreach (Bundle bundle in removeList)
                {
                    yield return new WaitForEndOfFrame();
                    bundle.Unload(true);
                }
                removeList.Clear();
            }
        }
    }

    public void ManualClearUnusedAssets(UnityAction finishcallback)
    {
        StartCoroutine(ManualClear(finishcallback));
    }
    IEnumerator ManualClear(UnityAction finishcallback)
    {
        yield return Resources.UnloadUnusedAssets();
        yield return new WaitForEndOfFrame();
        //https://www.cnblogs.com/Tearix/p/6844956.html
        //Resources.UnloadUnusedAssets只是标记，并不清理，并且不是标记为空闲（这个是GC干的，就是mono的清理，因为mono不归还内存给os，只在mono层标记为空闲，os层还是使用中），只是把该块内存降级，让gc清理。
        //所以destroy an object后调Resources.UnloadUnusedAssets并没有卵用，要等GC来了才生效。
        GC.Collect();
        yield return new WaitForEndOfFrame();
        List<Bundle> removeList = new List<Bundle>();
        foreach (Bundle bundle in bundles)
        {
            if (bundle.IsRemoved())
            {
                removeList.Add(bundle);
            }
        }

        if (removeList.Count > 0)
        {
            foreach (Bundle bundle in removeList)
            {
                RemoveBundle(bundle);
            }

            foreach (Bundle bundle in removeList)
            {
                bundle.Unload(true);
            }
        }
        Debug.Log("ManualClear finish");
        if(finishcallback != null)
        {
            finishcallback();
        }
    }
    void LoadBundleManifest()
    {
        string path = assetRootPath + ManifestName;
        Debug.Log($"Manifest path : {path}");
        AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
        if (assetBundle != null)
        {
            bundleManifest = (AssetBundleManifest)assetBundle.LoadAsset("assetbundlemanifest");
            string[] allbundles = bundleManifest.GetAllAssetBundles();
            for (int i = 0; i < allbundles.Length; i++)
            {
                //Debug.Log(allbundles[i]);
                bundleDic[allbundles[i]] = null;
            }
            Debug.Log("Load Manifest Success.");
            state = BundleState.Finish;
        }
        else
        {
            state = BundleState.Fail;
            Debug.LogError("Load Manifest Failed : " + (path));
        }
    }
    //该方法仅仅移除bundle在bundlemanager中的引用,如果想手动移除bundle以及资源，需要调用Bundle.Unload
    public void RemoveBundle(Bundle bundle)
    {
        if(bundles.Remove(bundle))
            bundleDic[bundle.bundleName] = null;
    }
    Bundle LoadBundle(ref string bundlename, bool isAsyn)
    {
        Bundle bundle = null;
        
        if (bundleDic.TryGetValue(bundlename, out bundle) == false)
        {
            Debug.LogError($"Not exist bundlename {bundlename} in Manifest.");
            return null;
        }

        if (bundle == null)
        {
            Debug.Log("bundleName: " + bundlename);
            bundle = new Bundle( bundlename);
            bundleDic[bundlename] = bundle;
            bundles.Add(bundle);
            bundle.AddRef();
            string[] allDepends = null;
            allDepends = bundleManifest.GetAllDependencies(bundlename);
            if (allDepends != null)
            {
                for (int i = 0; i < allDepends.Length; i++)
                {
                    Bundle dep = LoadBundle(ref allDepends[i], isAsyn);
                    if (dep != null)
                    {
                        dep.AddRef();
                        bundle.AddDependency(dep);
                    }
                }
            }
            bundle.Load(assetRootPath + bundlename, isAsyn);
        }
        
        return bundle;
    }
    void LoadBundleAsyn(ref string BundleName, UnityAction<Bundle> callback)
    {
        Bundle bundle = null;
        
        if(assetLoadMode == AssetLoadMode.Editor)
        {
            if (bundleDic.TryGetValue(BundleName, out bundle) && bundle != null)
            {
                callback(bundle);
            }
            else
            {
                bundle = new Bundle(BundleName);
                bundleDic[BundleName] = bundle;
                bundles.Add(bundle);
                bundle.AddRef();
                callback(bundle);
            }
        }
        else
        {
            string bundlename = GetBundleFilename(BundleName);
            if (bundleDic.TryGetValue(bundlename, out bundle) == false)
            {
                Debug.Log($"Not bundle {bundlename}");
                return;
            }
            if(bundle != null)
            {
                if (bundle.isDone)
                {//包已经加载完成
                    callback(bundle);
                    return;
                }
            }
            List<UnityAction<Bundle>> callbacks = null;
            if (callbackDic.TryGetValue(bundlename, out callbacks))
            {
                callbacks.Add(callback);
            }
            else
            {
                callbacks = new List<UnityAction<Bundle>>();
                callbacks.Add(callback);
                callbackDic.Add(bundlename, callbacks);
            }
            if (callbacks.Count == 1)
            {
                StartCoroutine(LoadBundleAsyn(bundlename));
            }
        }
    }
    Bundle LoadBundle(ref string BundleName)
    {
        Bundle bundle = null;

        if (assetLoadMode == AssetLoadMode.Editor)
        {
            if (bundleDic.TryGetValue(BundleName, out bundle) == false || bundle == null)
            {
                bundle = new Bundle(BundleName);
                bundleDic[BundleName] = bundle;
                bundles.Add(bundle);
                bundle.AddRef();
            }
        }
        else
        {
            string bundlename = GetBundleFilename(BundleName);
            if (bundleDic.TryGetValue(bundlename, out bundle) == false)
            {
                Debug.Log($"Not bundle {bundlename}");
                return null;
            }
            if (bundle == null)
            {
                bundle = LoadBundle(ref bundlename, false);
            }
            if(bundle.isDone == false)
            {
                Debug.LogError($"Asyn Loading bundle {bundlename}, but use syn load again.");
                return null;
            }
        }
        return bundle;
    }
    IEnumerator LoadBundleAsyn(string bundlename)
    {
        Bundle bundle = LoadBundle(ref bundlename, true);
        yield return bundle;
        List<UnityAction<Bundle>> callbacks = null;
        if (callbackDic.TryGetValue(bundlename, out callbacks))
        {
            for(int i = 0; i < callbacks.Count; i++)
            {
                if (callbacks[i] != null)
                    callbacks[i](bundle);
            }
            callbacks.Clear();
        }
    }
    public string GetBundleFilename(string resource)
    {
        int index = resource.LastIndexOf('.');
        if(index > 0)
        {
            resource = resource.Remove(index);
        }
        string bundlename = resource.ToLower();
        bundlename = bundlename.Replace("\\", "/");
        //bundlename = bundlename.Replace(@"\", "/");
        return bundlename;
    }
    public static void ReadAssetAsyn(ref string assetName, Bundle bundle, AssetBundleRequest req)
    {
        Instance.StartCoroutine(Instance.LoadAssetAsyn(assetName, bundle, req));
    }
    IEnumerator LoadAssetAsyn(string assetName, Bundle bundle, AssetBundleRequest req)
    {
        yield return req;
        bundle.AsynReadFinish(ref assetName, req);
    }
    string GetBundleName(ref string assetPath)
    {
        string dirPath = Path.GetDirectoryName(assetPath);
        string dirName = Path.GetFileNameWithoutExtension(dirPath);
        if (dirName.Contains(ab_sg_str))
        {
            return assetPath;
        }
        else if (dirName.Contains(abStr))
        {
            return dirPath;
        }
        return string.Empty;
    }
    //资源加载接口
    public void LoadAssetAsyn<T>(string assetPath, bool isAsynReadAsset, Action<T> callback) where T : UnityEngine.Object
    {
        //WeakReference wf = new WeakReference(callback);
        //callback = null;
        string bundleName = GetBundleName(ref assetPath);
        if(string.IsNullOrEmpty(bundleName))
        {
            Debug.Log($"ab包所在的文件夹应该包含{abStr}或者{ab_sg_str}:所以该路径有误{assetPath}");
            if (callback != null)
                callback(null);
        }
        LoadBundleAsyn(ref bundleName, (bundle) =>
        {
            if(bundle != null)
            {
                if(isAsynReadAsset)
                {
                    bundle.ReadAssetAsyn<T>(ref assetPath, (asset) =>
                    {
                        //if(wf.IsAlive)
                        //{
                        //    (wf.Target as UnityAction<T>)(asset as T);
                        //}
                        if(asset == null)
                        {
                            Debug.LogError("Not read asset , path : " + assetPath);
                        }
                        if(callback != null)
                            callback(asset as T);
                    });
                }
                else
                {
                    T asset = bundle.ReadAsset<T>(ref assetPath);
                    //if (wf.IsAlive)
                    //{
                    //    (wf.Target as UnityAction<T>)(asset as T);
                    //}
                    if (asset == null)
                    {
                        Debug.LogError("Not read asset , path : " + assetPath);
                    }
                    if (callback != null)
                        callback(asset as T);
                }
            }
            else
            {
                Debug.LogError("Not load asset , path : " + assetPath);
            }
        });
    }
    public T LoadAsset<T>(string assetPath) where T : UnityEngine.Object
    {
        string bundleName = GetBundleName(ref assetPath);
        Bundle bundle = LoadBundle(ref bundleName);
        if (bundle == null)
            return null;
        T asset = bundle.ReadAsset<T>(ref assetPath);
        if (asset == null)
        {
            Debug.LogError("Not read asset , path : " + assetPath);
        }
        return asset;
    }
}