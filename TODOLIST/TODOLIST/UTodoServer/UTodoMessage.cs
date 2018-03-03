
using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace UTODO
{
    public class UTodoMessage
    {
        public Socket clientSocket = null;
        public const int bufferSize = 1024;
        public byte[] buffer = new byte[bufferSize];
        public string content = String.Empty;
        public StringBuilder stringBuilder = new StringBuilder();

        public static void ReadMessage( string content ,UTodoServerClient client ) // 包含路由和数据 和客户端定制相同的规则 
        { 
            // "type.classname.methodname|data" 数据格式
            string[] routeAndData = content.Split('|');
            string route = routeAndData[0];
            string data = routeAndData[1].Replace("<EOF>",""); 
            UTodoJsonObject jsonObject = JsonUtility.FromJson<UTodoJsonObject>(data); 
            UTodoRouter.route(route, jsonObject, client);
        }

        public static void ReadMessage( string content,Action<UTodoJsonObject> callback )
        {
            // "type.classname.methodname|data" 数据格式
            string[] routeAndData = content.Split('|');
            string route = routeAndData[0];
            string data = routeAndData[1];
            UTodoJsonObject jsonObject = JsonUtility.FromJson<UTodoJsonObject>(data);
            callback(jsonObject);
        }

        public static byte[] PackMessage(string router)
        {
            return Encoding.ASCII.GetBytes(router + "|<EOF>");
        }

        public static byte[] PackMessage( UTodoJsonObject jsonObject ) // 打包数据并返回给客户端
        {
            string data = "|"+JsonUtility.ToJson(jsonObject) + "<EOF>";
            return Encoding.ASCII.GetBytes(data);
        }

        public static byte[] PackMessage( string router,UTodoJsonObject jsonObject )
        {
            string data = router + "|" + JsonUtility.ToJson(jsonObject) + "<EOF>";
            return Encoding.ASCII.GetBytes(data);
        }
    }
}