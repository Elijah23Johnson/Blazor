﻿using Blazor.Util;
using System;
using System.Runtime.CompilerServices;
using Blazor.VirtualDom;

namespace WebAssembly
{
    public sealed class Runtime
    {
        [MethodImplAttribute(/* InternalCall */ (MethodImplOptions)4096)]
        static extern string InvokeJS(string str, out int exceptional_result);

        [MethodImplAttribute(/* InternalCall */ (MethodImplOptions)4096)]
        static extern string SetElemFromVNodeImpl(string elementRef, int componentRef, VDomItem[] oldVDom, VDomItem[] newVDom, int replaceContainer);

        public static string InvokeJS(string str)
        {
            int exception = 0;
            var res = InvokeJS(str, out exception);
            if (exception != 0)
                throw new JSException(res);
            return res;
        }

        public static TResult CallJS<TResult>(string method, params object[] args)
        {
            var resultJson = InvokeJS($"Module.receiveInvocationFromDotNet({ JsonUtil.Serialize(new { method, args }) })");
            return JsonUtil.Deserialize<TResult>(resultJson);
        }

        public static void SetElemFromVNode(string elementRef, int componentRef, VDomItem[] oldVDom, VDomItem[] newVDom, bool replaceContainer)
        {
            var res = SetElemFromVNodeImpl(elementRef, componentRef, oldVDom, newVDom, replaceContainer ? 1 : 0);
            if (res != null)
            {
                throw new JSException(res);
            }
        }
    }

    public class JSException : Exception
    {
        public JSException(string msg) : base(msg) { }
    }
}
