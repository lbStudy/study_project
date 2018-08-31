using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    public class CacheList
    {
        static Queue<List<long>> cache = new Queue<List<long>>();

        public static List<long> Take()
        {
            if(cache.Count > 0)
            {
                return cache.Dequeue();
            }
            List<long> ls = new List<long>();
            return ls;
        }
        public static void Back(List<long> ls)
        {
            ls.Clear();
            if(cache.Count < 50)
                cache.Enqueue(ls);
        }
    }
}
