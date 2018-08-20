using System;
using System.Collections.Generic;
using System.Reflection;

namespace Base
{
    public class FuncDispatcher
    {
        private object[] allFuncs;

        private static FuncDispatcher instance;
        public static FuncDispatcher Instance
        {
            get
            {
                if (instance == null)
                    instance = new FuncDispatcher();
                return instance;
            }
        }
        public void Load(Assembly assembly, int maxCount)
        {
            this.allFuncs = new object[maxCount];
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(FunctionAttribute), false);
                if (attrs == null || attrs.Length == 0)
                {
                    continue;
                }
                FunctionAttribute aEventAttribute = (FunctionAttribute)attrs[0];
                object obj = Activator.CreateInstance(type);
                this.allFuncs[aEventAttribute.funcid] = obj;
            }
        }

        public void Run(int funcId)
        {
            if (this.allFuncs[funcId] != null)
            {
                try
                {
                    IFunc iFunc = this.allFuncs[funcId] as IFunc;
                    if (iFunc != null)
                    {
                        iFunc.Run();
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
        }

        public void Run<A>(int funcId, A a)
        {
            if (this.allFuncs[funcId] != null)
            {
                try
                {
                    IFunc<A> iFunc = this.allFuncs[funcId] as IFunc<A>;
                    if (iFunc != null)
                    {
                        iFunc.Run(a);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
        }

        public void Run<A, B>(int funcId, A a, B b)
        {
            if (this.allFuncs[funcId] != null)
            {
                try
                {
                    IFunc<A, B> iFunc = this.allFuncs[funcId] as IFunc<A, B>;
                    if (iFunc != null)
                    {
                        iFunc.Run(a, b);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
        }

        public void Run<A, B, C>(int funcId, A a, B b, C c)
        {
            if (this.allFuncs[funcId] != null)
            {
                try
                {
                    IFunc<A, B, C> iFunc = this.allFuncs[funcId] as IFunc<A, B, C>;
                    if (iFunc != null)
                    {
                        iFunc.Run(a, b, c);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
        }

        public void Run<A, B, C, D>(int funcId, A a, B b, C c, D d)
        {
            if (this.allFuncs[funcId] != null)
            {
                try
                {
                    IFunc<A, B, C, D> iFunc = this.allFuncs[funcId] as IFunc<A, B, C, D>;
                    if (iFunc != null)
                    {
                        iFunc.Run(a, b, c, d);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
        }

        public void Run<A, B, C, D, E>(int funcId, A a, B b, C c, D d, E e)
        {
            if (this.allFuncs[funcId] != null)
            {
                try
                {
                    IFunc<A, B, C, D, E> iFunc = this.allFuncs[funcId] as IFunc<A, B, C, D, E>;
                    if (iFunc != null)
                    {
                        iFunc.Run(a, b, c, d, e);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
        }

        public void Run<A, B, C, D, E, F>(int funcId, A a, B b, C c, D d, E e, F f)
        {
            if (this.allFuncs[funcId] != null)
            {
                try
                {
                    IFunc<A, B, C, D, E, F> iFunc = this.allFuncs[funcId] as IFunc<A, B, C, D, E, F>;
                    if (iFunc != null)
                    {
                        iFunc.Run(a, b, c, d, e, f);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
        }

        public A Run<A>(int funcId)
        {
            if (this.allFuncs[funcId] != null)
            {
                try
                {
                    IFunc_R<A> iFunc = this.allFuncs[funcId] as IFunc_R<A>;
                    if (iFunc != null)
                    {
                        return iFunc.Run();
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
            return default(A);
        }

        public A Run<A, B>(int funcId, B b)
        {
            if (this.allFuncs[funcId] != null)
            {
                try
                {
                    IFunc_R<A, B> iFunc = this.allFuncs[funcId] as IFunc_R<A, B>;
                    if (iFunc != null)
                    {
                        return iFunc.Run(b);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
            return default(A);
        }

        public A Run<A, B, C>(int funcId, B b, C c)
        {
            if (this.allFuncs[funcId] != null)
            {
                try
                {
                    IFunc_R<A, B, C> iFunc = this.allFuncs[funcId] as IFunc_R<A, B, C>;
                    if (iFunc != null)
                    {
                        iFunc.Run(b, c);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
            return default(A);
        }

        public A Run<A, B, C, D>(int funcId, B b, C c, D d)
        {
            if (this.allFuncs[funcId] != null)
            {
                try
                {
                    IFunc_R<A, B, C, D> iFunc = this.allFuncs[funcId] as IFunc_R<A, B, C, D>;
                    if (iFunc != null)
                    {
                        return iFunc.Run(b, c, d);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
            return default(A);
        }

        public A Run<A, B, C, D, E>(int funcId, B b, C c, D d, E e)
        {
            if (this.allFuncs[funcId] != null)
            {
                try
                {
                    IFunc_R<A, B, C, D, E> iFunc = this.allFuncs[funcId] as IFunc_R<A, B, C, D, E>;
                    if (iFunc != null)
                    {
                        return iFunc.Run(b, c, d, e);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
            return default(A);
        }

        public A Run<A, B, C, D, E, F>(int funcId, B b, C c, D d, E e, F f)
        {
            if (this.allFuncs[funcId] != null)
            {
                try
                {
                    IFunc_R<A, B, C, D, E, F> iFunc = this.allFuncs[funcId] as IFunc_R<A, B, C, D, E, F>;
                    if (iFunc != null)
                    {
                        return iFunc.Run(b, c, d, e, f);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
            return default(A);
        }

        public A Run<A, B, C, D, E, F, G>(int funcId, B b, C c, D d, E e, F f, G g)
        {
            if (this.allFuncs[funcId] != null)
            {
                try
                {
                    IFunc_R<A, B, C, D, E, F, G> iFunc = this.allFuncs[funcId] as IFunc_R<A, B, C, D, E, F, G>;
                    if (iFunc != null)
                    {
                        return iFunc.Run(b, c, d, e, f, g);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
            return default(A);
        }
    }
}
