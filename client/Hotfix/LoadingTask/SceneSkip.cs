using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hotfix
{
    public enum EnterSceneEvent
    {
        None,
        PlaybackEnd,
        ShowJiesuan
    }
    public enum SceneType
    {
        None,
        Init,
        Login,
        Main,
        Loading,
        Battle
    }
    public class SceneSkip
    {
        public static SceneSkip Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SceneSkip();
                }
                return instance;
            }
        }
        private static SceneSkip instance;
        public AsyncOperation async;
        public SceneType curScene = SceneType.None;
        public SceneType fromScene = SceneType.None;
        public SceneType toScene = SceneType.None;
        Dictionary<SceneType, string> sceneDic = new Dictionary<SceneType, string>() {
            {SceneType.Login, "login" },
            {SceneType.Main, "main" },
            {SceneType.Loading, "loading" },
            {SceneType.Battle, "battle" }
        };
        List<LoadingTask> synTasks = new List<LoadingTask>();
        List<LoadingTask> asynTasks = new List<LoadingTask>();
        List<LoadingTask> synAfterTasks = new List<LoadingTask>();
        List<LoadingTask> asynAfterTasks = new List<LoadingTask>();
        public float taskTotalTime;
        private SceneSkip()
        {

        }
        public void Init(SceneType sceneType)
        {
            curScene = sceneType;
            fromScene = SceneType.None;
            toScene = SceneType.None;
            synTasks.Clear();
            asynTasks.Clear();
            synAfterTasks.Clear();
            asynAfterTasks.Clear();
            async = null;
        }
        public void Clear()
        {
            curScene = SceneType.None;
            fromScene = SceneType.None;
            toScene = SceneType.None;
            synTasks.Clear();
            asynTasks.Clear();
            synAfterTasks.Clear();
            asynAfterTasks.Clear();
            async = null;
        }
        public float SurplusTime()
        {
            float surplusTime = 0;
            for (int i = 0; i < asynTasks.Count; i++)
            {
                surplusTime += asynTasks[i].needTime;
            }
            for (int i = 0; i < synTasks.Count; i++)
            {
                surplusTime += synTasks[i].needTime;
            }
            return surplusTime;
        }
        public void GoScene(SceneType toScene, bool isLoading = true)
        {
            Debug.Log($"GoScene {toScene.ToString()}");

            if (toScene == curScene)
            {
                return;
            }
            SceneManager.sceneLoaded += SceneManager_activeSceneChanged;
            this.fromScene = curScene;
            this.toScene = toScene;
            if (isLoading)
            {
                SceneManager.LoadScene(sceneDic[SceneType.Loading]);
            }
            else
            {
                curScene = toScene;
                SceneManager.LoadScene(sceneDic[toScene]);
            }
        }
        public void AddTaskLoading(LoadingTask task)
        {
            if (task.isDoAfterEnter)
            {
                if (task.isAsyn)
                {
                    if (asynAfterTasks.Contains(task))
                        return;
                    asynAfterTasks.Add(task);
                }
                else
                {
                    if (synAfterTasks.Contains(task))
                        return;
                    synAfterTasks.Add(task);
                }
            }
            else
            {
                if (task.isAsyn)
                {
                    if (asynTasks.Contains(task))
                        return;
                    asynTasks.Add(task);
                }
                else
                {
                    if (synTasks.Contains(task))
                        return;
                    synTasks.Add(task);
                }
            }

        }
        public bool RemvoeTaskLoading(LoadingTask task)
        {
            if (task.isDoAfterEnter)
            {
                if (task.isAsyn)
                {
                    return asynAfterTasks.Remove(task);
                }
                else
                {
                    return synAfterTasks.Remove(task);
                }
            }
            else
            {
                if (task.isAsyn)
                {
                    return asynTasks.Remove(task);
                }
                else
                {
                    return synTasks.Remove(task);
                }
            }
        }
        private void SceneManager_activeSceneChanged(Scene arg0, LoadSceneMode arg1)
        {
            if (curScene == this.fromScene)
            {
                StartLoading();
            }
            else
            {
                LoadFinish();
            }
        }
        public void OneLoadingTaskFinish(LoadingTask task)
        {
            if (RemvoeTaskLoading(task))
            {
                if (task.isDoAfterEnter)
                {
                    if (asynAfterTasks.Count == 0 & synAfterTasks.Count == 0)
                    {
                        //GlobalEventManager.Trigger(GlobalEventType.EnterScene, new EventPackage((long)curScene));
                    }
                    else
                    {
                        if (!task.isAsyn)
                        {
                            DoAfterSynTask();
                        }
                    }
                }
                else
                {
                    if (asynTasks.Count == 0 && synTasks.Count == 0)
                    {
                        async = SceneManager.LoadSceneAsync(sceneDic[toScene]);
                    }
                    else
                    {
                        if (!task.isAsyn)
                        {
                            DoSynTask();
                        }
                    }
                }
            }
        }
        public void StartLoading()
        {
            curScene = SceneType.Loading;
            //GlobalEventManager.Trigger(GlobalEventType.EnterScene, new EventPackage((long)curScene));
            GC.Collect();
            taskTotalTime = 0;
            if (asynTasks.Count == 0 && synTasks.Count == 0)
            {
                async = SceneManager.LoadSceneAsync(sceneDic[toScene]);
            }
            else
            {
                for (int i = 0; i < asynTasks.Count; i++)
                {
                    taskTotalTime += asynTasks[i].needTime;
                }
                for (int i = 0; i < synTasks.Count; i++)
                {
                    taskTotalTime += synTasks[i].needTime;
                }
                DoAsynTask();
                DoSynTask();
            }
        }
        void DoAsynTask()
        {
            for (int i = 0; i < asynTasks.Count; i++)
            {
                asynTasks[i].Do();
            }
        }
        void DoSynTask()
        {
            if (synTasks.Count > 0)
                synTasks[0].Do();
        }
        void DoAfterAsynTask()
        {
            for (int i = 0; i < asynAfterTasks.Count; i++)
            {
                asynAfterTasks[i].Do();
            }
        }
        void DoAfterSynTask()
        {
            if (synAfterTasks.Count > 0)
                synAfterTasks[0].Do();
        }
        public void LoadFinish()
        {
            SceneManager.sceneLoaded -= SceneManager_activeSceneChanged;
            async = null;
            curScene = toScene;
            if (asynTasks.Count == 0 && synTasks.Count == 0)
            {
                //GlobalEventManager.Trigger(GlobalEventType.EnterScene, new EventPackage((long)curScene));
            }
            else
            {
                DoAfterAsynTask();
                DoAfterSynTask();
            }
        }
        public void GoBack()
        {
            if (curScene != SceneType.Loading)
            {
                return;
            }
            synTasks.Clear();
            asynTasks.Clear();
            synAfterTasks.Clear();
            asynAfterTasks.Clear();
            toScene = fromScene;
            async = SceneManager.LoadSceneAsync(sceneDic[fromScene]);
        }
    }
}
