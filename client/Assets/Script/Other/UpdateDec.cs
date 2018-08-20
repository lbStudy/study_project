using UnityEngine;
using UnityEngine.UI;
public class UpdateDec : MonoBehaviour {

    public Image slider;
    public Text txt_size;


	void Start ()
    {
        string assetRootPath = GetAseetLoadRootPath();
        Debug.Log("assetLoadMode : " + Begin.Instance.assetLoadMode.ToString() + "assetRootPath : " + assetRootPath);
        BundleManager.Instance.Init(assetRootPath, Begin.Instance.updateUrl, Begin.Instance.assetLoadMode);
        slider.transform.parent.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (BundleManager.Instance.State == BundleState.WaitUpdate)
        {
            slider.transform.parent.gameObject.SetActive(true);
            BundleManager.Instance.StartUpdateBundleFromServer();
        }
        else if(BundleManager.Instance.State == BundleState.Updating || BundleManager.Instance.State == BundleState.Finish)
        {
            string dwStr = "KB";
            float total = BundleManager.Instance.TotalUpdateSize / 1024f;
            float n = 1024f;
            if(total > 1024)
            {
                dwStr = "MB";
                total = total / 1024f;
                //total = (int)(total * 1000) * 0.001f;
                n = 1024f * 1024f;
            }
            float cur = BundleManager.Instance.CurUpdateSize / n;
            //cur = (int)(cur * 1000) * 0.001f;
            txt_size.text = cur.ToString("f2") + dwStr + " / " + total.ToString("f2") + dwStr;
            slider.fillAmount = cur / total;
        }
        else if(BundleManager.Instance.State == BundleState.Fail)
        {
            Debug.LogError("bundle init fail");
        }
    }

    public string GetAseetLoadRootPath()
    {
        string path = string.Empty;
        if (Begin.Instance.assetLoadMode == AssetLoadMode.Editor)
        {
#if UNITY_EDITOR
            path = PlatformHelper.DataPath;
#else
            Debug.LogError("Can not use AssetLoadType.Editor when not in editor.");
#endif
        }
        else if (Begin.Instance.assetLoadMode == AssetLoadMode.StreamingAssets)
        {
            path = PlatformHelper.StreamingAssetsPath;
        }
        else if (Begin.Instance.assetLoadMode == AssetLoadMode.Publish)
        {
#if UNITY_EDITOR
            path = System.Environment.CurrentDirectory + "/AssetBundles/";
#else
                path = PlatformHelper.PersistentDataPath;
#endif
        }
        return path;
    }
}
