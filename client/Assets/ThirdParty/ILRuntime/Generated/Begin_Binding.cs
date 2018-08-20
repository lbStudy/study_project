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
    unsafe class Begin_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Begin);

            field = type.GetField("Instance", flag);
            app.RegisterCLRFieldGetter(field, get_Instance_0);
            app.RegisterCLRFieldSetter(field, set_Instance_0);
            field = type.GetField("ip", flag);
            app.RegisterCLRFieldGetter(field, get_ip_1);
            app.RegisterCLRFieldSetter(field, set_ip_1);
            field = type.GetField("port", flag);
            app.RegisterCLRFieldGetter(field, get_port_2);
            app.RegisterCLRFieldSetter(field, set_port_2);


        }



        static object get_Instance_0(ref object o)
        {
            return Begin.Instance;
        }
        static void set_Instance_0(ref object o, object v)
        {
            Begin.Instance = (Begin)v;
        }
        static object get_ip_1(ref object o)
        {
            return ((Begin)o).ip;
        }
        static void set_ip_1(ref object o, object v)
        {
            ((Begin)o).ip = (System.String)v;
        }
        static object get_port_2(ref object o)
        {
            return ((Begin)o).port;
        }
        static void set_port_2(ref object o, object v)
        {
            ((Begin)o).port = (System.Int32)v;
        }


    }
}
