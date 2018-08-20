using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Hotfix
{
    public enum FrameType
    {
        RoomPanel,
        LoginPanel,
        MessagePanel,
        LoginTipPanel,
        LoadingPanel,
        HintPanel,
        MainPanel,
        ReconnectTipPanel,
        NpcPanel,
        WorldMapPanel,
        TipsText,
        ShelterPanel,//遮挡面板
        LoginCGPanel,
        DamagePanel,
        LeftBottomDesPanel,
        RightDesPanel
    }
    public enum ShowLayer
    {
        Layer_1 = 0,//血条...
        Layer_2,
        Layer_3,//主界面
        Layer_4,//弹出界面,自动添加遮挡面板防止点击到下层界面
        Layer_5 //提示信息
    }

    public class UIManager
    {
        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new UIManager();
                return instance;
            }
        }
        private static UIManager instance;

        Frame scenePanel;
        public Frame ScenePanel { get { return scenePanel; } }
        Dictionary<FrameType, Frame> frameDic = new Dictionary<FrameType, Frame>();
        Dictionary<ShowLayer, List<Frame>> frames = new Dictionary<ShowLayer, List<Frame>>();
        Canvas[] layers;
        Camera uiCamera;
        public Camera UICamera { get { return uiCamera; } }

        public void Init(GameObject uiRoot)
        {
            layers = uiRoot.transform.GetComponentsInChildren<Canvas>();
            uiCamera = uiRoot.transform.GetComponentInChildren<Camera>();
        }
        public void Clear()
        {
            ClearUI();
        }
        public void EnterScene(EventPackage package)
        {
            //SceneType sceneType = (SceneType)package.param1;
            //if(sceneType == SceneType.Loading)
            //{
            //    ClearUI();
            //}
            //else if(sceneType != SceneType.Battle)
            //{
            //    ClearUI();
            //}
            //EnterScene(sceneType);
        }
        //public void EnterScene(SceneType sceneType)
        //{
        //    if (sceneType == SceneType.Loading)
        //    {
        //        scenePanel = Show(FrameType.LoadingPanel, null, ShowLayer.Layer_4);
        //    }
        //    else if (sceneType == SceneType.Main)
        //    {
        //        scenePanel = Show(FrameType.MainPanel, null, ShowLayer.Layer_3);
        //    }
        //    else if (sceneType == SceneType.Battle)
        //    {
        //        scenePanel = Show(FrameType.RoomPanel, null, ShowLayer.Layer_3);
        //    }
        //    else if (SceneType.Login == sceneType)
        //    {
        //        scenePanel = Show(FrameType.LoginPanel, null, ShowLayer.Layer_3);            
        //    }
        //}
        public void ClearUI()
        {
            if (scenePanel != null)
            {
                scenePanel.Destroy();
                scenePanel = null;
            }
            foreach (Frame frame in frameDic.Values)
            {
                frame.Destroy();
            }
            frameDic.Clear();
            frames.Clear();
        }
        List<Frame> removeList = new List<Frame>();
        public void FixedUpdate()
        {
            foreach (var f in frameDic)
            {
                if (f.Value.State != FrameState.Destroy)
                    f.Value.FixedUpdate();
                else
                    removeList.Add(f.Value);
            }
            if (removeList.Count > 0)
            {
                List<Frame> layerFrames = null;
                foreach (Frame v in removeList)
                {
                    frameDic.Remove(v.frameType);
                    if (frames.TryGetValue(v.layer, out layerFrames))
                    {
                        layerFrames.Remove(v);
                    }
                }
                removeList.Clear();
            }
        }
        public Frame Show(FrameType frameType, System.Object[] arg = null, ShowLayer layer = ShowLayer.Layer_4)
        {
            Frame frame = null;
            if (frameDic.TryGetValue(frameType, out frame) == false)
            {
                string name = Enum.GetName(typeof(FrameType), frameType);
                Assembly am = Assembly.GetAssembly(typeof(UIManager));
                frame = (Frame)am.CreateInstance(name);
                if (frame == null)
                {
                    Debug.LogError($"Not Exist Panel Class {name}");
                }
                GameObject template = BundleManager.Instance.LoadAsset<GameObject>(ConstDefine.ui_prefab_path + frame.GetType().Name + ".prefab");
                frame.prefab = template;
                frame.panel = GameObject.Instantiate<GameObject>(template, layers[(int)layer].transform);

                frame.Init();
            }
            frameDic[frameType] = frame;
            frame.layer = layer;
            frame.frameType = frameType;
            List<Frame> layerFrames = null;
            if (frames.TryGetValue(layer, out layerFrames))
            {
                layerFrames.Remove(frame);
            }
            else
            {
                layerFrames = new List<Frame>();
                frames.Add(layer, layerFrames);
            }
            if (layer == ShowLayer.Layer_4 && frameType != FrameType.ShelterPanel)
            {//自动加透明遮罩防止点击到其他底层界面
                Frame shelterPanel = layerFrames.Find(x => x.frameType == FrameType.ShelterPanel);
                if (shelterPanel == null)
                {
                    Show(FrameType.ShelterPanel);
                }
                else
                {
                    layerFrames.Remove(shelterPanel);
                    layerFrames.Add(shelterPanel);
                    shelterPanel.panel.transform.SetAsLastSibling();
                    shelterPanel.Show(null);
                }
            }
            layerFrames.Add(frame);
            frame.panel.transform.SetParent(layers[(int)layer].transform);
            frame.panel.transform.SetAsLastSibling();
            frame.Show(arg);
            return frame;
        }

        public Frame Find(FrameType frameType)
        {
            Frame frame = null;

            frameDic.TryGetValue(frameType, out frame);

            return frame;
        }
        public void DestroyFrame(FrameType frameType)
        {
            Frame frame = Find(frameType);
            if (frame != null)
            {
                frame.Destroy();
            }
        }
        public void HideFrame(FrameType frameType)
        {
            Frame frame = Find(frameType);
            if (frame != null)
            {
                frame.Hide();
            }
        }
        public void FrameAdjust(Frame frame)
        {
            if (frame == null || frame.State == FrameState.Show)
                return;
            if (frame.layer == ShowLayer.Layer_4 && frame.frameType != FrameType.ShelterPanel)
            {
                List<Frame> layerFrames = null;
                if (frames.TryGetValue(frame.layer, out layerFrames))
                {
                    if (layerFrames.Remove(frame))
                    {
                        layerFrames.Insert(0, frame);

                        Frame topFrame = layerFrames[layerFrames.Count - 1];
                        if (topFrame.frameType == FrameType.ShelterPanel)
                        {//隐藏或移除最后一个面板
                            Frame preFrame = layerFrames[layerFrames.Count - 2];
                            if (preFrame.State == FrameState.Show)
                            {
                                topFrame.panel.transform.SetAsLastSibling();
                                preFrame.panel.transform.SetAsLastSibling();

                                layerFrames[layerFrames.Count - 1] = preFrame;
                                layerFrames[layerFrames.Count - 2] = topFrame;
                            }
                            else
                            {
                                topFrame.Hide();
                            }
                        }
                    }
                }
            }
        }
        public Transform GetLayer(ShowLayer layer)
        {
            return layers[(int)layer].transform;
        }


    }
}


