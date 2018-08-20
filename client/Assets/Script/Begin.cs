using Base;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Begin : MonoBehaviour {

    public static Begin Instance;

    public string updateUrl = "http://127.0.0.1:8888/";
    public AssetLoadMode assetLoadMode = AssetLoadMode.StreamingAssets;
    public string ip = "127.0.0.1";
    public int port = 20001;
    public ILRuntime.Runtime.Enviorment.AppDomain appdomain;
    ILStaticMethod update;
    ILStaticMethod fixedUpdate;
    ILStaticMethod lateUpdate;
    public bool isAwait5S = false;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
        appdomain.DebugService.StartDebugService(56000);
    }
    IEnumerator Start () {
        if(isAwait5S)
            yield return new WaitForSeconds(5);
        LoadHotFixAssembly();
        LoadProtocol();
        update = new ILStaticMethod(appdomain, "Hotfix.Game", "Update", 0);
        fixedUpdate = new ILStaticMethod(appdomain, "Hotfix.Game", "FixedUpdate", 0);
        lateUpdate = new ILStaticMethod(appdomain, "Hotfix.Game", "LateUpdate", 0);
        appdomain.Invoke("Hotfix.Game", "Init", null, null);
    }
    private void Update()
    {
        if(update != null)
            update.Run();
    }
    private void LateUpdate()
    {
        if(lateUpdate != null)
            lateUpdate.Run();
    }

    private void FixedUpdate()
    {
        if(fixedUpdate != null)
            fixedUpdate.Run();
    }
    void LoadHotFixAssembly()
    {
        //PDB文件是调试数据库，如需要在日志中显示报错的行号，则必须提供PDB文件，不过由于会额外耗用内存，正式发布时请将PDB去掉，下面LoadAssembly的时候pdb传null即可
        TextAsset dllAsset = BundleManager.Instance.LoadAsset<TextAsset>("Hotfix_ab/Hotfix.dll.bytes");
        TextAsset pdbAsset = null;
        pdbAsset = BundleManager.Instance.LoadAsset<TextAsset>("Hotfix_ab/Hotfix.pdb.bytes");
        using (System.IO.MemoryStream fs = new MemoryStream(dllAsset.bytes))
        {
            using (System.IO.MemoryStream p = new MemoryStream(pdbAsset.bytes))
            {
                appdomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());
            }
        }
        // 注册重定向函数

        // 注册委托
        appdomain.DelegateManager.RegisterMethodDelegate<System.Object>();        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>();
        //由于CLR重定向只能重定向一次，并且CLR绑定就是利用的CLR重定向，所以请在初始化最后阶段再执行下面的代码，以保证CLR重定向生效
        ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);

        //这里做一些ILRuntime的注册，这里我们注册值类型Binder，注释和解注下面的代码来对比性能差别
        appdomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
        appdomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
        appdomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());

        // 注册适配器
        //使用Couroutine时，C#编译器会自动生成一个实现了IEnumerator，IEnumerator<object>，IDisposable接口的类，因为这是跨域继承，所以需要写CrossBindAdapter（详细请看04_Inheritance教程），Demo已经直接写好，直接注册即可
        appdomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        appdomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineClassInheritanceAdaptor());

        LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appdomain);
    }
    public void DoCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
    void LoadProtocol()
    {
        //List<Type> allTypes = GetHotfixTypes();
        TextAsset ta = BundleManager.Instance.LoadAsset<TextAsset>("Config/Other_ab/client_protocol.xml");
        if (null != ta)
        {
            using (System.IO.MemoryStream ms = new MemoryStream(ta.bytes))
            {

                //ProtocolDispatcher.Instance.LoadProtocol(ms, allTypes);
                //ProtocolDispatcher.Instance.Load(allTypes);
                ProtocolDispatcher.Instance.LoadProtocol(ms, typeof(Begin).Assembly);
                ProtocolDispatcher.Instance.Load(typeof(Begin).Assembly);
            }
        }
        else
        {
            Debug.LogError($"protocol path error :");
        }
    }
    public List<Type> GetHotfixTypes()
    {
        if (this.appdomain == null)
        {
            return new List<Type>();
        }

        return this.appdomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToList();
    }
}
