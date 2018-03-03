  
using System; 
using System.Net;
using System.Net.Sockets;
using System.Text; 
using UnityEngine;

namespace UTODO
{
    // 负责用于启动基于tcp协议的socket服务器，用于多个unity客户端之间的相互数据访问，与事件通知
    public class UTodoServer
    {
        public UTodoChannel channel = null; 
        public int OLCount
        {
            get { return channel.clientCount; }
        }
        private Socket serverSocket = null; 

        public UTodoServer(string ipAddress, int port = 12581)
        { 
            channel = new UTodoChannel(1); 
            serverSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
            serverSocket.Listen(20);
            serverSocket.BeginAccept(AcceptCallback, serverSocket);
            Debug.Log("UTodo服务器已经正常启动...");
        }
        
        private void AcceptCallback( IAsyncResult result )
        {
            Socket server = result.AsyncState as Socket;
            if(null == server)
                throw new NullReferenceException("server is null");
            Socket client =  server.EndAccept(result); 
            Debug.Log("客户端加入服务器");
            channel.Add(new UTodoServerClient(client, this)); // 服务器记录所有客户端连接对象 接收数据的事情有clienttiem自行处理
            serverSocket.BeginAccept(AcceptCallback, serverSocket);
        } 

        public void send(Socket client, string message) // 向单独客户端发送消息
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            client.BeginSend(data, 0, data.Length, SocketFlags.None, sendCallback, client);
        }

        public void send(Socket[] clients, string message)
        {
            for (int i = 0; i < clients.Length; i++)
                send(clients[i], message);
        }

        public void send(Socket client, string message, Action<UTodoJsonObject> callback) // 向客户端发送消息，并期待回调
        {

        }

        private void sendCallback(IAsyncResult result)
        {
            try
            {
                Socket client = result.AsyncState as Socket;
                int byteCount = client.EndSend(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void broadcast2Channel(int channelId,string data) // 向所有客户端发送消息
        { 
            foreach (UTodoServerClient client in channel) 
                send(client.ClientSocket,data);
        }
         
        // 关闭服务器
        public void Close()
        {
            foreach (UTodoServerClient client in channel)
                send(client.ClientSocket, "quit");
            try
            {
                serverSocket.Shutdown(SocketShutdown.Both);
            }
            catch ( Exception e)
            {
                 
            } 
            serverSocket.Close();
            Debug.Log("UTodo服务器已经正常关闭...");
        } 
    }
}