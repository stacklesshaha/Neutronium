﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.CefGlueHelper
{
    public static class CefV8ValueExtension
    {
        #region array

        private  static IEnumerable<CefV8Value> GetElements(this CefV8Value @this)
        {
            if (!@this.IsArray)
                throw new ArgumentException("Method valid only for array");

            int count = @this.GetArrayLength();

            for(int i=0; i< count; i++)
            {
                yield return @this.GetValue(i);
            }
        }

        public static CefV8Value[] GetArrayElements(this CefV8Value @this)
        {
            return @this.GetElements().ToArray();     
        }

        #endregion

        #region function

        #region call function
        public static CefV8Value Invoke(this CefV8Value @this, string functionname, CefV8Context iCefV8Context, params CefV8Value[] args)
        {
            return @this.InvokeAsync(functionname, iCefV8Context, args).Result;
        }

        public static CefV8Value ExecuteFunction(this CefV8Value @this)
        {
            return @this.ExecuteFunction(null, new CefV8Value[] { });
        }

        public static Task<CefV8Value> InvokeAsync(this CefV8Value @this, string functionname, CefV8Context context, params CefV8Value[] args)
        {
            
            return context.EvaluateInCreateContextAsync(() =>
                {
                    var fn = @this.GetValue(functionname);
                    if ((fn==null) && !fn.IsFunction)
                        return CefV8Value.CreateUndefined();
                    return fn.ExecuteFunction(@this, args);
                }
                );
        }

        #endregion

        #region Bind : creation of native method



        public static void Bind(this CefV8Value @this, string functionname, CefV8Context iCefV8Context, Action<CefV8Context,string, CefV8Value, CefV8Value[]> iaction)
        {
            iCefV8Context.CreateInContextAsync(() =>
                {
                    var function = CefV8Value.CreateFunction(functionname, new CefV8Handler_Action(iCefV8Context,iaction));
                    @this.SetValue(functionname, function, CefV8PropertyAttribute.None);
                });
        }

        public static void Bind(this CefV8Value @this,string functionname, CefV8Context iCefV8Context,  Func<string, CefV8Value, CefV8Value[], Tuple<CefV8Value, string>> iFunction)
        {
            iCefV8Context.CreateInContextAsync(() =>
              {
                  var function = CefV8Value.CreateFunction(functionname, new CefV8Handler_Function(iFunction));
                  @this.SetValue(functionname, function, CefV8PropertyAttribute.None);
              });
        }

        public static void Bind(this CefV8Value @this, string functionname, CefV8Context iCefV8Context, Action<CefV8Context,CefV8Value, CefV8Value[]> iaction)
        {
            iCefV8Context.CreateInContextAsync(() =>
             {
                 var function = CefV8Value.CreateFunction(functionname, new CefV8Handler_Simple_Action(iCefV8Context,iaction));
                 @this.SetValue(functionname, function, CefV8PropertyAttribute.None);
             });
        }

        public static void Bind(this CefV8Value @this,string functionname, CefV8Context iCefV8Context,  Func<CefV8Value, CefV8Value[], Tuple<CefV8Value, string>> iFunction)
        {
            iCefV8Context.CreateInContextAsync(() =>
             {
                 var function = CefV8Value.CreateFunction(functionname, new CefV8Handler_Simple_Function(iFunction));
                 @this.SetValue(functionname, function, CefV8PropertyAttribute.None);
             });
        }

        #endregion

        #endregion
    }
}