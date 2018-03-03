
using System.Collections;
using System.Collections.Generic;

namespace UTODO
{
    public class UTodoChannel : IEnumerable
    {
        public int channelId;
        public int clientCount { get { return clients.Count; } }

        private List<UTodoServerClient> clients = null;

        public UTodoChannel(int channelId)
        {
            this.channelId = channelId;
            clients = new List<UTodoServerClient>();
        }

        public void Add( UTodoServerClient client )
        {
            clients.Add(client);
        }

        public void Remove( UTodoServerClient client )
        {
            clients.Remove(client);
        }

        public void Close()
        {
            foreach (UTodoServerClient client in clients)
                client.Close();
            clients.Clear();
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < clients.Count; i++)
                yield return clients[i];
        }
    }
}