
using System.Collections.Generic;
using UnityEngine;

namespace UTODO
{
    [CreateAssetMenu(fileName = "UTodoTasks",menuName = "UTodoRec",order = 0)]
    public class UTaskObject : ScriptableObject
    {
        [SerializeField]
        private List<UTsak> m_tasks = new List<UTsak>();

        internal List<UTsak> tasks
        {
            get { return m_tasks; }
            set { m_tasks = value; }
        }

        internal int NextTaskId
        {
            get { return m_tasks.Count + 1; }
        }

        public void AddTask( UTsak task )
        {
            tasks.Add(task);
        }

        public void RemoveTask( UTsak task )
        {
            tasks.Remove(task);
        }
    }
}