using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class UIListenEvent_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UIListenEvent);

            field = type.GetField("drag", flag);
            app.RegisterCLRFieldGetter(field, get_drag_0);
            app.RegisterCLRFieldSetter(field, set_drag_0);


        }



        static object get_drag_0(ref object o)
        {
            return ((UIListenEvent)o).drag;
        }
        static void set_drag_0(ref object o, object v)
        {
            ((UIListenEvent)o).drag = (UIListenEvent.Drag)v;
        }


    }
}
