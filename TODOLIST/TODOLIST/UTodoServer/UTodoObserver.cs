
using System.Collections.Generic;

namespace UTODO
{
    // 记录客户端注册到服务器的监听事件
    public static class UTodoObserver
    {
        private static IDictionary<string, List<UTodoServerClient>> observers = new Dictionary<string, List<UTodoServerClient>>();

        // 注册
        public static void register(string router,UTodoServerClient client)
        {
            if (observers.ContainsKey(router))
                observers[router].Add(client);
            else
                observers.Add(router, new List<UTodoServerClient>() { client });
        }

        // 取消注册
        public static void unRegister(string router , UTodoServerClient client )
        {
            if (observers.ContainsKey(router))
                observers[router].Remove(client);
        }

        public static void handleObserver(string router, UTodoJsonObject jsonObject)
        {
            var clients = observers[router];
            foreach (var client in clients)
                client.ClientSocket.Send(UTodoMessage.PackMessage(jsonObject)); //todo error
        }
    }
}