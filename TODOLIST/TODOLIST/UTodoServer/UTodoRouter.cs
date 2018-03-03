
using System; 
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace UTODO
{
    // 负责路由
    public class UTodoRouter
    {
        private static IDictionary<string, object> instances = new Dictionary<string, object>();

        public static void route(string route, UTodoJsonObject jsonObject,UTodoServerClient client)
        { 
            string[] routes = route.Split('.');
            string type = routes[0];
            string typeName = routes[1];
            string methodName = routes[2];
            routeType(type, typeName, methodName, jsonObject, client);
        }

        private static void routeType(string type, string typeName, string methodName, UTodoJsonObject jsonObject, UTodoServerClient client)
        {
            switch (type)
            {
                case "request": // 要求即时返回数据
                    request(typeName, methodName, jsonObject);
                    break;
                case "notify":  // 无需返回数据
                    notify(typeName, methodName, jsonObject);
                    break;
                case "on":      // 客户端监听服务器事件
                    on(typeName, methodName, client);
                    break;
            }
        }

        private static void request(string typeName, string methodName, UTodoJsonObject jsonObject)
        {
            Debug.Log("client request : " + typeName + "." + methodName);
            Type type = Type.GetType(typeName, true, true);
            if (null == type)
                throw new NullReferenceException(typeName + "reflect class failed.Type if null.");
            object obj = GetInstance(typeName);
            if (null == obj)
            {
                obj = Activator.CreateInstance(type);
                instances.Add(typeName,obj);
            }
            MethodInfo method = type.GetMethod(methodName);
            BindingFlags flags = BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public;
            object[] parameters = { jsonObject };
            if (null == method)
                throw new NullReferenceException();
            object resultData = method.Invoke(obj, flags, Type.DefaultBinder, parameters, null);
            // resultData数据要发送回客户端
        }

        private static void notify(string typeName, string methodName, UTodoJsonObject jsonObject)
        {
            Debug.Log("client notify : " + typeName + "." + methodName);
            Type type = Type.GetType(typeName, true, true);
            if (null == type)
                throw new NullReferenceException(typeName + "reflect class failed.Type if null.");
            object obj = GetInstance(typeName);
            if (null == obj)
            {
                obj = Activator.CreateInstance(type);
                instances.Add(typeName, obj);
            }
            MethodInfo method = type.GetMethod(methodName);
            BindingFlags flags = BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public;
            object[] parameters = { jsonObject };
            if (null == method)
                throw new NullReferenceException();
            method.Invoke(obj, flags, Type.DefaultBinder, parameters, null);
        }

        private static void on(string typeName, string methodName, UTodoServerClient client)
        {
            Debug.Log("client on : " + typeName + "." + methodName);
            UTodoObserver.register(typeName + "." + methodName, client);
        }

        private static object GetInstance(string typeName)
        {
            return instances[typeName];
        }
    }
}