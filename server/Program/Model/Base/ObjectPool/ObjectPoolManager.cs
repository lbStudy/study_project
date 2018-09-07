using System;
using System.Collections.Generic;
using System.Reflection;
namespace Base
{
    public class ObjectPoolManager
    {
        Dictionary<Type, ObjectPool> poolDic = new Dictionary<Type, ObjectPool>();
        Type inheritClass = typeof(Disposer);
        private static ObjectPoolManager instance;
        public static ObjectPoolManager Instance {
            get
            {
                if(instance == null)
                {
                    instance = new ObjectPoolManager();
                }
                return instance;
            }
        }
        PoolAttribute GetPoolAttribute(Type type)
        {
            object[] attrs = type.GetCustomAttributes(typeof(PoolAttribute), true);
            if (attrs.Length == 0)
            {
                Console.WriteLine($"Error: {type.Name} have not PoolAttribute, so can not take from pool.");
                return null;
            }
            return (PoolAttribute)attrs[0];
        }
        public T Take<T>() where T : Disposer
        {
            ObjectPool pool = TakePool<T>();
            if (pool == null)
                return null;
            return pool.Take() as T;
        }
        public T Take<T, T1>(T1 t1) where T : Disposer
        {
            ObjectPool pool = TakePool<T>();
            if (pool == null)
                return null;
            return pool.Take(t1) as T;
        }
        public T Take<T, T1, T2>(T1 t1, T2 t2) where T : Disposer
        {
            ObjectPool pool = TakePool<T>();
            if (pool == null)
                return null;
            return pool.Take(t1, t2) as T;
        }
        public T Take<T, T1, T2, T3>(T1 t1, T2 t2, T3 t3) where T : Disposer
        {
            ObjectPool pool = TakePool<T>();
            if (pool == null)
                return null;
            return pool.Take(t1, t2, t3) as T;
        }
        ObjectPool TakePool<T>()
        {
            Type type = typeof(T);
            ObjectPool pool = null;
            if (poolDic.TryGetValue(type, out pool) == false)
            {
                if (type.IsSubclassOf(inheritClass))
                {
                    PoolAttribute attr = GetPoolAttribute(type);
                    if (attr == null)
                    {
                        return null;
                    }
                    pool = new ObjectPool(type, attr.maxCount);
                    poolDic.Add(type, pool);
                }
                else
                {
                    Console.WriteLine($"Error: {type.Name} need inherit {inheritClass.Name}, so can not take from pool.");
                    return null;
                }
            }
            return pool;
        }
        public void Back(object obj)
        {
            Disposer disposer = obj as Disposer;
            if (disposer == null)
            {
                Console.WriteLine($"Error: {obj.GetType().Name} need Disposer, so can not back to pool.");
                return;
            }
            if (disposer.IsBackPool == false)
            {
                return;
            }
            Type type = obj.GetType();
            ObjectPool pool = null;
            if (poolDic.TryGetValue(type, out pool))
            {
                pool.Back(disposer);
            }
            else
            {
                Console.WriteLine($"Error: {type.Name} not exist pool, so can not back to pool.");
            }
        }
    }
}
