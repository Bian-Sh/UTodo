 
using System;

namespace UTODO
{
    public enum UTodoUserLevel
    {
        Normal,
        Admin
    }

    [Serializable]
    public class UTodoUser
    {
        public int id;
        public string name;
        public string password;
        public UTodoUserLevel level = UTodoUserLevel.Normal;
    }
}