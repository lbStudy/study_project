using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    //从池中取出时默认调用
    public interface Initer
    {
        void Init();
    }
    public interface Initer<T1>
    {
        void Init(T1 t);
    }
    public interface Initer<T1, T2>
    {
        void Init(T1 t, T2 t2);
    }
    public interface Initer<T1, T2, T3>
    {
        void Init(T1 t, T2 t2, T3 t3);
    }
}
