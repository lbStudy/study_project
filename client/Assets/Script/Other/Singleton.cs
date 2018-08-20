using System;
using System.Collections;
using UnityEngine;
using System.Reflection;


public class Singleton<T> where T : new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (Equals(instance, default(T)))
            {
                instance = new T();
            }
            return instance;
        }
    }

    public void DestorySingleton()
    {
        OnDestroy();
        instance = default(T);
    }

    protected virtual void OnDestroy()
    {

    }
}
//继承至MonoBehaviour，绑定在游戏对象上，切换场景游戏对象不会被销毁
public class BehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static bool NeedDestroyOnRestart = false ;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null || _instance.gameObject == null)
                {
                    _instance = (T) FindObjectOfType(typeof (T));

                    if (FindObjectsOfType(typeof (T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                                       " - there should never be more than 1 singleton! [" + typeof (T).ToString() +
                                       "] Reopenning the scene might fix it." + "\n");
                        DontDestroyOnLoad(_instance.gameObject);
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof (T).ToString();

                        DontDestroyOnLoad(singleton);
                    }
                    else
                    {
                        Debug.Log("[Singleton] Using instance already created: " +
                                  _instance.gameObject.name + "\n");
                        DontDestroyOnLoad(_instance.gameObject);
                    }


                }

                return _instance;
            }
        }
    }
    public virtual void OnDestroy()
    {
        DestorySingleton();
    }
    public void DestorySingleton()
    {
        if (_instance != null)
        {
            Debug.Log("BehaviourSingleton.DestorySingleton Instance: " + _instance.GetType());
            if (NeedDestroyOnRestart)
            {
                if (_instance.gameObject != null)
                {
                    Debug.Log("BehaviourSingleton.DestorySingleton destroy gameobject Instance: " + _instance.GetType());
                    DestroyImmediate(_instance.gameObject);
                }
            }
        }
        _instance = null;
    }
}
//继承至MonoBehaviour，绑定在游戏对象上，切换场景游戏对象可被销毁
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null || _instance.gameObject == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        return _instance;
                    }
                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = "(singleton) " + typeof(T).ToString();
                }

                return _instance;
            }
        }
    }
    public virtual void OnDestroy()
    {
        DestorySingleton();
    }
    public void DestorySingleton()
    {
        _instance = null;
    }
}
