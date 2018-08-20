using System.Collections.Generic;
using Data;
using Base;

namespace FuncHandle
{
    [Function((int)FunctionId.LoadHotfixModule)]
    public class LoadHotfixModule : IFunc<int>
    {
        public void Run(int module)
        {
            if (module <= 0)
            {
                return;
            }
            if ((module & (int)HotfixModule.Protocol) > 0)
            {
                HitfixComponent.Instance.LoadProtocolAssembly();
            }
            if ((module & (int)HotfixModule.Event) > 0)
            {
                HitfixComponent.Instance.LoadEventAssembly();
            }
            if ((module & (int)HotfixModule.Http) > 0)
            {
                HitfixComponent.Instance.LoadHttpAssembly();
            }
            if ((module & (int)HotfixModule.Func) > 0)
            {
                HitfixComponent.Instance.LoadFuncAssembly();
            }
        }
    }
}
