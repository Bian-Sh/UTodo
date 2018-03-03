
using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace UTODO
{
    public class UTodoClient
    {
        private Socket clientSocket = null;
        private UTodoMessage message = null;
         
        public UTodoClient()
        {
            message = new UTodoMessage();
            clientSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp); 
        }

        public void connect( string host,int port )
        {
            clientSocket.BeginConnect(host, port, ConnectCallback, null);
        }

        private void ConnectCallback( IAsyncResult result )
        {
            clientSocket.EndConnect(result);
            clientSocket.BeginReceive(message.buffer, 0, UTodoMessage.bufferSize, SocketFlags.None, RecieveCallback, null);
        }

        private void RecieveCallback( IAsyncResult result )
        {
            int byteCount = clientSocket.EndReceive(result);
            if (byteCount > 0)
            {
                message.stringBuilder.Append(Encoding.ASCII.GetString(message.buffer, 0, byteCount));
                string content = message.stringBuilder.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    UTodoMessage.ReadMessage(content, Debug.Log);
                }
                else
                {
                    clientSocket.BeginReceive(message.buffer, 0, UTodoMessage.bufferSize, SocketFlags.None, RecieveCallback, message);
                }
            }
        }

        public void request(string route,Action<UTodoJsonObject> callback)
        {
            var data = UTodoMessage.PackMessage("request."+route);
            clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, callback);
        }

        public void request(string route, UTodoJsonObject jsonObject,Action<UTodoJsonObject> callback)
        {
            var data = UTodoMessage.PackMessage("request." + route, jsonObject);
            clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, callback);
        }

        public void notify(string route)
        {
            var data = UTodoMessage.PackMessage("notify." + route);
            clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, null);
        }

        public void notify( string route, UTodoJsonObject jsonObject)
        {
            var data = UTodoMessage.PackMessage("notify." + route, jsonObject);
            clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, null);
        }

        public void on( string route ,Action<UTodoJsonObject> callback)
        {
            var data = UTodoMessage.PackMessage("on." + route);
            clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, callback); 
        }

        private void SendCallback(IAsyncResult result)
        {
            int byteCount = clientSocket.EndSend(result);
            // var callback = (Action<UTodoJsonObject>) result.AsyncState; 回调函数的问题
        }
    }
}