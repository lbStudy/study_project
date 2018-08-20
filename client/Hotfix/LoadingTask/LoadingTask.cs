using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Hotfix
{
    public class LoadingTask
    {
        public float needTime;
        public bool isAsyn;
        public bool isDoAfterEnter;
        public LoadingTask(float needTime, bool isAsyn, bool isDoAfterEnter)
        {
            this.needTime = needTime;
            this.isAsyn = isAsyn;
            this.isDoAfterEnter = isDoAfterEnter;
        }
        public virtual void Do()
        {

        }

    }

}
