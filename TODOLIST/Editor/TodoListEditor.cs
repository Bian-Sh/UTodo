
//todo 功能需求
//todo 1.读取C#中的标记，并显示在列表中
//todo 2.读取Lua中的标记，并显示在列表中
//todo 3.C#与Lua标记语法共用，切实现自由配置，不同语法做出等级区分
//todo 4.将自身作为服务器，可将局域网内的其他机器的todolist链接起来
//todo 5.做不同账号的等级区分，以实现主机的通知下放
//todo 6.可导出接口文档

using LuaMVC;
using UnityEditor.IMGUI.Controls;


namespace TODOLIST
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using System.Collections.Generic;
    using System;
    using Object = UnityEngine.Object;

    [InitializeOnLoad]
    public class TodoListEditor : EditorWindow
    {
        [InitializeOnLoadMethod]
        private static void InitLoad()
        { 
            m_todos = new Dictionary<int, TodoListItem>();
            m_showTodos = new List<TodoListItem>();
        }

        private Vector2 m_treePos;
        private Vector2 m_listPos;
        private string m_searchContent = null;
        private string m_currentPage = "Todo";

        [SerializeField]
        TreeViewState m_TreeViewState;
        static TodoListTree m_TodoListTree;
         
        private static Dictionary<int, TodoListItem> m_todos = null;
        private static List<TodoListItem> m_showTodos = null;
        private static void GetFiles(string path)
        {  
            string[] filesPath = Directory.GetFiles(path);
            for (int i = 0; i < filesPath.Length; i++)
            { 
                FileInfo fileInfo = new FileInfo(filesPath[i]);
                if (fileInfo.Name.Contains(".cs") && !fileInfo.Name.Contains(".meta"))
                    ReadFile(fileInfo.FullName, fileInfo.Name, "cs");
                if ((fileInfo.Name.Contains(".lua") || fileInfo.Name.Contains(".lua.txt")) && !fileInfo.Name.Contains(".meta"))
                    ReadFile(fileInfo.FullName, fileInfo.Name, "lua");
            }
            string[] childrenPath = Directory.GetDirectories(path);
            for (int i = 0; i < childrenPath.Length; i++)
                GetFiles(childrenPath[i]); 
        } 

        //todo 优化一下提升效率
        private static void ReadFile(string filePath,string fileName,string type)
        { 
            using (StreamReader reader = new StreamReader(filePath))
            {
                List<TodoItem> todos = new List<TodoItem>();
                int lineCounter = 0;
                while (reader.Peek() > -1)
                {
                    lineCounter++;
                    string content = reader.ReadLine();
                    if(string.IsNullOrEmpty(content))
                        continue;
                    string lower = content.ToLower(); 
                    //todo3 新的规则也将在这里添加 规则需要重新改动
                    if (lower.Contains("todo") && (lower.Contains("//") || lower.Contains("--")))
                    { 
                        string[] strs = null; 
                        strs = content.Trim().Split(' ');
                        TodoItem todo = new TodoItem();
                        todo.lineCount = lineCounter;
                        todo.scriptPath = filePath;
                        if (strs.Length >= 2)
                            todo.todoContext = strs[strs.Length - 1];
                        if (lower.Contains("todo1"))
                            todo.todoLevel = 1;
                        if (lower.Contains("todo2"))
                            todo.todoLevel = 2;
                        if (lower.Contains("todo3"))
                            todo.todoLevel = 3;
                        todos.Add(todo);
                    } 
                }
                if (todos.Count > 0)
                { 
                    TodoListItem listItem = new TodoListItem();
                    listItem.scriptPath = filePath;
                    listItem.m_items = todos;
                    m_todos.Add(m_todos.Count + 4, listItem);
                    if (type == "cs")
                        m_TodoListTree.AddSharpChild(new TreeViewItem(m_todos.Count + 3, 2, fileName));
                    if (type == "lua")
                        m_TodoListTree.AddLuaChild(new TreeViewItem(m_todos.Count + 3, 2, fileName));
                }
            } 
        }

        void OnEnable()
        { 
            if (m_TreeViewState == null)
                m_TreeViewState = new TreeViewState(); 
            m_TodoListTree = new TodoListTree(m_TreeViewState);
            m_TodoListTree.refreshList += () => { m_showTodos.Clear(); };
            m_TodoListTree.contextClick += (id) =>
            {
                if (id < 4)
                    return;
                m_showTodos.Add(m_todos[id]);
            };
            m_TodoListTree.contextDoubleClick += (id) =>
            {
                if (id < 4)
                    return;
                var todo = m_todos[id];
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(todo.scriptPath, todo.m_items[0].lineCount);
            };
            GetFiles(Application.dataPath); 
        } 

        private void OnGUI()
        {
            int sum = 0;
            foreach (var todos in m_todos)
                sum += todos.Value.m_items.Count;
            EditorGUILayout.LabelField("目前共有" + sum + "件事项，合计" + m_todos.Count + "个脚本加入待办事项列表");
            DrawToolbar();
            DrawMainList(); 
        }

        private void DrawToolbar()
        {
            using (new HorizontalBlock(EditorStyles.toolbar))
            {
                GUILayout.Label(m_currentPage);
                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
                {

                }
                if (GUILayout.Button("Todo", EditorStyles.toolbarButton))
                {
                    m_currentPage = "Todo";
                }
                if (GUILayout.Button("List", EditorStyles.toolbarButton))
                {
                    m_currentPage = "List";
                }
                if (GUILayout.Button("Notification", EditorStyles.toolbarButton))
                {
                    m_currentPage = "Notification";
                }
                if (GUILayout.Button("Account", EditorStyles.toolbarButton))
                {
                    m_currentPage = "Account";
                }
                GUILayout.FlexibleSpace();
                m_searchContent = DrawSearchField(m_searchContent, GUILayout.Width(200));
            }
        } 
        private void DrawMainList()
        {
            using (new HorizontalBlock())
            {
                using (new VerticalBlock(GUI.skin.box, GUILayout.Width(position.width / 4f), GUILayout.ExpandHeight(true)))
                { 
                    Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
                    m_TodoListTree.OnGUI(rect);
                }
                using (new VerticalBlock(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                {
                    using (new ScrollviewBlock(ref m_listPos))
                    {
                        using (new HorizontalBlock(EditorStyles.toolbar))
                        { 
                            if (GUILayout.Button("Order By Level", EditorStyles.toolbarButton))
                            {

                            }
                            if (GUILayout.Button("Order By Type", EditorStyles.toolbarButton))
                            {

                            } 
                            GUILayout.FlexibleSpace();
                            GUILayout.Label("当前共有10个项目");
                        }
                        DrawTodoList();
                    }
                }
            }
        }

        private void DrawTodoList()
        {
            for (int i = 0; i < m_showTodos.Count; i++)
            {
                var todos = m_showTodos[i].m_items;
                // todo 在此绘制列表样式
                foreach (TodoItem item in todos)
                {
                    using (new VerticalBlock(EditorStyles.helpBox))
                    {
                        using (new HorizontalBlock())
                        {
                            GUIStyle style = new GUIStyle();
                            style.active.textColor = Color.red;
                            GUILayout.Label("LEVEL" + item.todoLevel, style);
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(item.scriptPath, EditorStyles.miniBoldLabel);
                        }
                        GUILayout.Space(5f);
                        GUILayout.Label(item.todoContext, EditorStyles.largeLabel);
                    }

                    Event e = Event.current;
                    var rect = GUILayoutUtility.GetLastRect();
                    if (e.isMouse && e.type == EventType.MouseDown && rect.Contains(e.mousePosition) && e.clickCount == 2)
                    {
                        var todoItem = item;
                        EditorApplication.delayCall += () =>
                        {
                            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(todoItem.scriptPath, todoItem.lineCount);
                        };
                    }
                }
            }
        }

        private string DrawSearchField(string searchStr, params GUILayoutOption[] options)
        {
            searchStr = GUILayout.TextField(searchStr, "ToolbarSeachTextField", options);
            if (GUILayout.Button("", "ToolbarSeachCancelButton"))
            {
                searchStr = "";
                GUI.FocusControl(null);
            }
            return searchStr;
        }
          
        [MenuItem("TodoList/Open")]
        public static void OpenTodolist()
        { 
            var window = GetWindow<TodoListEditor>();
            window.minSize = new Vector2(400,300);
            window.titleContent = new GUIContent("TodoList");
            window.Show();
            window.ShowNotification(new GUIContent("xxx"));
        } 
        [MenuItem("TodoList/Quit")]
        public static void QuitTodoList()
        {

        }
    }


    public class VerticalBlock : IDisposable
    {
        public VerticalBlock(params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
        }

        public VerticalBlock(GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(style, options);
        }

        public void Dispose()
        {
            GUILayout.EndVertical();
        }
    } 
    public class ScrollviewBlock : IDisposable
    {
        public ScrollviewBlock(ref Vector2 scrollPos, params GUILayoutOption[] options)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, options);
        }

        public void Dispose()
        {
            GUILayout.EndScrollView();
        }
    } 
    public class HorizontalBlock : IDisposable
    {
        public HorizontalBlock(params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(options);
        }

        public HorizontalBlock(GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(style, options);
        }

        public void Dispose()
        {
            GUILayout.EndHorizontal();
        }
    } 
    public class ColoredBlock : IDisposable
    {
        public ColoredBlock(Color color)
        {
            GUI.color = color;
        }

        public void Dispose()
        {
            GUI.color = Color.white;
        }
    }
}