using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AtlasManager : Singleton<AtlasManager>
{
    const string dirPath = "UI/";
    public void Init()
    {
        SpriteAtlasManager.atlasRequested += LoadAtlas;
    }
    public void Clear()
    {
        SpriteAtlasManager.atlasRequested -= LoadAtlas;
    }
    void LoadAtlas(string name, Action<SpriteAtlas> action)
    {
        string path = dirPath + name + "/" + name + ".spriteatlas";
        Debug.Log("SpriteAtlas Start Load : " + path);
        SpriteAtlas sa = BundleManager.Instance.LoadAsset<SpriteAtlas>(path);
        if(sa == null)
        {
            Debug.Log("SpriteAtlas Load Fail : " + path);
        }
        else
        {
            action(sa);
        }
    }
}
