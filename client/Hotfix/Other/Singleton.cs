using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{

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
}
