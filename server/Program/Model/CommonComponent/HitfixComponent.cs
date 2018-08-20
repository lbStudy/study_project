
using System;
using System.IO;
using System.Reflection;
using Base;

public class HitfixComponent : Component, IAwake
{
    public static HitfixComponent Instance;

    public void Awake()
    {
        Instance = this;
        ProtocolDispatcher.Instance.LoadConfig();
        LoadProtocolAssembly();
        LoadEventAssembly();
        LoadFuncAssembly();
        LoadHttpAssembly();
    }
    public override void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }
        base.Dispose();
        Instance = null;
    }
    public void LoadProtocolAssembly()
    {
        try
        {
            byte[] dllBytes = File.ReadAllBytes("./ProtocolHandle.dll");
            byte[] pdbBytes = File.ReadAllBytes("./ProtocolHandle.pdb");
            ProtocolDispatcher.Instance.Load(Assembly.Load(dllBytes, pdbBytes), Game.Instance.AppType);
        }
        catch
        {
            Console.WriteLine($"error : fail Load ProtocolHandle.dll/ProtocolHandle.pdb");
        }
    }
    public void LoadEventAssembly()
    {
        try
        {
            byte[] dllBytes = File.ReadAllBytes("./EventHandle.dll");
            byte[] pdbBytes = File.ReadAllBytes("./EventHandle.pdb");
            EventDispatcher.Instance.Load(Assembly.Load(dllBytes, pdbBytes), Game.Instance.AppType, (int)Data.EventIdType.Max);
        }
        catch
        {
            Console.WriteLine($"error : fail Load EventHandle.dll/EventHandle.pdb");
        }
    }
    public void LoadFuncAssembly()
    {
        try
        {
            byte[] dllBytes = File.ReadAllBytes("./FuncHandle.dll");
            byte[] pdbBytes = File.ReadAllBytes("./FuncHandle.pdb");
            FuncDispatcher.Instance.Load(Assembly.Load(dllBytes, pdbBytes), (int)Data.FunctionId.Max);
        }
        catch
        {
            Console.WriteLine($"error : fail Load FuncHandle.dll/FuncHandle.pdb");
        }
    }
    public void LoadHttpAssembly()
    {
        try
        {
            if(Game.Instance.AppType == AppType.LoginServer || Game.Instance.AppType == AppType.ManagerServer)
            {
                byte[] dllBytes = File.ReadAllBytes("./HttpHandle.dll");
                byte[] pdbBytes = File.ReadAllBytes("./HttpHandle.pdb");
                HttpDispatcher.Instance.Load(Assembly.Load(dllBytes, pdbBytes), Game.Instance.AppType);
            }
        }
        catch
        {
            Console.WriteLine($"error : fail Load HttpHandle.dll/HttpHandle.pdb");
        }
    }
}
