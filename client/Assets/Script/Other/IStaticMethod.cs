using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IStaticMethod
{
    public abstract void Run();
    public abstract void Run(object a);
    public abstract void Run(object a, object b);
    public abstract void Run(object a, object b, object c);
}
