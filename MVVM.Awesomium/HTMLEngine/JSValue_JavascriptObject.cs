﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVM.HTML.Core.V8JavascriptObject;
using Awesomium.Core;

namespace MVVM.Awesomium
{
    internal class JSValue_JavascriptObject : IJavascriptObject
    {
        private JSValue _JSValue;
        public JSValue_JavascriptObject(JSValue iJSValue)
        {
            _JSValue = iJSValue;
        }

        public JSValue JSValue { get { return _JSValue; } }

        public bool HasRelevantId()
        {
            if (!_JSValue.IsObject)
                return false;

            var jso = (JSObject)_JSValue;

            if  (!jso.HasProperty("_MappedId"))
                return false;

            return jso["_MappedId"].IsInteger;
        }

        public uint GetID()
        {
            if (!_JSValue.IsObject)
                return 0;

            JSObject jso = _JSValue;

            if (!jso.HasProperty("_MappedId"))
                return 0;

            var v = jso["_MappedId"];

            return v.IsInteger ? (uint)v : 0;
        }

        public bool IsUndefined
        {
            get { return _JSValue.IsUndefined; }
        }

        public bool IsNull
        {
            get { return _JSValue.IsNull; }
        }

        public bool IsObject
        {
            get { return _JSValue.IsObject; }
        }

        public bool IsArray
        {
            get { return _JSValue.IsArray; }
        }

        public bool IsString
        {
            get { return _JSValue.IsString; }
        }

        public bool IsNumber
        {
            get { return _JSValue.IsNumber; }
        }

        public bool IsDate
        {
            get { return  false; }
        }

        public bool IsBool
        {
            get { return _JSValue.IsBoolean; }
        }

        public int GetArrayLength()
        {
            return ((JSValue[])_JSValue).Length;
        }

        public bool HasValue(string attributename)
        {
            if (!IsObject)
                return false;

            return ((JSObject)_JSValue).HasProperty(attributename);
        }

        public IJavascriptObject Invoke(string iFunctionName, HTML.Core.V8JavascriptObject.IWebView iContext, params IJavascriptObject[] iparam)
        {
            var res =((JSObject)_JSValue).Invoke(iFunctionName, iparam.Cast<IJavascriptObject>().Select(c => c.Convert()).ToArray());
            return res.Convert();
        }

        public Task<IJavascriptObject> InvokeAsync(string iFunctionName, HTML.Core.V8JavascriptObject.IWebView iContext, params IJavascriptObject[] iparam)
        {
            return Task.FromResult(Invoke(iFunctionName, iContext, iparam));
        }

        public void Bind(string iFunctionName, HTML.Core.V8JavascriptObject.IWebView iContext, Action<string, IJavascriptObject, IJavascriptObject[]> action)
        {
            JSObject ob = _JSValue;
            ob.Bind(iFunctionName, false, (o, e) => { action(iFunctionName, null, e.Arguments.Select(el => el.Convert()).ToArray()); });
        }

        public void SetValue(string AttributeName, IJavascriptObject element, CreationOption ioption = CreationOption.None)
        {
            ((JSObject)_JSValue)[AttributeName] = element.Convert();
        }

        public string GetStringValue()
        {
            return (string)_JSValue;
        }

        public double GetDoubleValue()
        {
            return (double)_JSValue;
        }

        public bool GetBoolValue()
        {
            return (bool)_JSValue;
        }

        public int GetIntValue()
        {
            return (int)_JSValue;
        }

        public IJavascriptObject ExecuteFunction()
        {
            throw new NotImplementedException();
        }

        public IJavascriptObject GetValue(string ivalue)
        {
            return ((JSObject)_JSValue)[ivalue].Convert();
        }

        public IJavascriptObject GetValue(int ivalue)
        {
            JSValue[] ar = (JSValue[])_JSValue;
            return ar[ivalue].Convert();
        }

        public IJavascriptObject[] GetArrayElements()
        {
            JSValue[] ar = (JSValue[])_JSValue;
            return ar.Cast<JSValue>().Select(el => el.Convert()).ToArray();
        }

        public void Dispose()
        {
        }
    }
}