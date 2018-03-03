   
namespace UTODO
{
    using System;
    using System.Collections.Generic;

    public class TodoListItem
    {
        public string scriptPath = String.Empty;
        public IList<TodoItem> m_items = new List<TodoItem>();
    }

    public class TodoItem
    {
        public int lineCount = 0; 
        public int todoLevel = 1;
        public string todoContext = null;
        public string scriptPath = null;

        public TodoItem(){}
        public TodoItem( int count,int level,string context )
        {
            this.lineCount = count;
            this.todoLevel = level;
            this.todoContext = context;
        }
    }
}