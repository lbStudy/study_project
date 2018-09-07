using System;
using System.Collections.Generic;


namespace Base
{
    public class ObjectPool
    {
        private Type createTyep;
        private Queue<Disposer> cacheObj = new Queue<Disposer>();
        private int maxCount;
        public ObjectPool(Type createTyep, int maxCount)
        {
            this.createTyep = createTyep;
            this.maxCount = maxCount;
        }
        object TakeObj()
        {
            object obj = null;
            if (cacheObj.Count > 0)
            {
                obj = cacheObj.Dequeue();
            }
            else
            {
                obj = Activator.CreateInstance(createTyep);
            }

            Disposer disposer = obj as Disposer;
            disposer.IsDisposed = false;
            disposer.IsBackPool = true;
            return obj;
        }
        public object Take()
        {
            object obj = TakeObj();
            Initer initer = obj as Initer;
            if (initer != null)
                initer.Init();

            return obj;
        }
        public object Take<T1>(T1 t1)
        {
            object obj = TakeObj();

            Initer<T1> initer = obj as Initer<T1>;
            if (initer != null)
                initer.Init(t1);

            return obj;
        }

        public object Take<T1, T2>(T1 t1, T2 t2)
        {
            object obj = TakeObj();

            Initer<T1, T2> initer = obj as Initer<T1, T2>;
            if (initer != null)
                initer.Init(t1, t2);

            return obj;
        }

        public object Take<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            object obj = TakeObj();

            Initer<T1, T2, T3> initer = obj as Initer<T1, T2, T3>;
            if (initer != null)
                initer.Init(t1, t2, t3);

            return obj;
        }

        public void Back(Disposer disposer)
        {
            disposer.IsBackPool = false;
            if(disposer.IsDisposed == false)
                disposer.Dispose();
            if(cacheObj.Count < maxCount)
            {
                cacheObj.Enqueue(disposer);
            }
        }
    }
}
