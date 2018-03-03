
using System.Collections.Generic;
using UnityEngine;

namespace UTODO
{
    [CreateAssetMenu(fileName = "UserDB", menuName = "Create User Group DataBase", order = 0)]
    public class UTodoUserGroup : ScriptableObject
    {
        [SerializeField]
        private List<UTodoUser> m_users = new List<UTodoUser>();

        internal List<UTodoUser> users
        {
            get { return m_users; }
            set { m_users = value;}
        }

        internal string[] userNames
        {
            get
            {
                string[] userNames = new string[users.Count];
                for (int i = 0; i < userNames.Length; i++)
                {
                    userNames[i] = users[i].name;
                    if (users[i].level == UTodoUserLevel.Admin)
                        userNames[i] = users[i].name + "(管理员)";
                }
                return userNames;
            }
        }

        public void AddUser( UTodoUser user )
        {
            users.Add(user);
        }

        public void RemoveUser( UTodoUser user )
        {
            users.Remove(user);
        } 
    }
}