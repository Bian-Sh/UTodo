
using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace UTODO
{
    // 服务器记录客户端连接的类型
    public class UTodoServerClient
    {
        private Socket client = null;
        public Socket ClientSocket { get { return client; } }
        private UTodoServer server = null;
        private UTodoMessage message = null;

        public UTodoServerClient(Socket client, UTodoServer server)
        {
            this.client = client;
            this.server = server;
            message = new UTodoMessage();
            message.clientSocket = client;
            client.BeginReceive(message.buffer, 0, UTodoMessage.bufferSize, SocketFlags.None, RecieveCallback, null);
        }

        private void RecieveCallback(IAsyncResult result)
        {  
            int byteCount = client.EndReceive(result); 
            if (byteCount > 0)
            {
                message.stringBuilder.Append(Encoding.ASCII.GetString(message.buffer, 0, byteCount));
                string content = message.stringBuilder.ToString(); 
                if (content.IndexOf("<EOF>") > -1)
                    UTodoMessage.ReadMessage(content, this);
                else
                    client.BeginReceive(message.buffer, 0, UTodoMessage.bufferSize, SocketFlags.None, RecieveCallback, message);
            }
        }

        /// <summary>
        /// 断开socket连接
        /// </summary>
        public void Close()
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}