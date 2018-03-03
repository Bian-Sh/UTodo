
//todo1 功能需求
//todo2 1.读取C#中的标记，并显示在列表中
//todo0 2.读取Lua中的标记，并显示在列表中
//todo3 3.C#与Lua标记语法共用，切实现自由配置，不同语法做出等级区分
//todo3 4.将自身作为服务器，可将局域网内的其他机器的todolist链接起来
//todo3 5.做不同账号的等级区分，以实现主机的通知下放
//todo3 6.可导出接口文档
 
using UnityEditor.IMGUI.Controls;


namespace UTODO
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using System.Collections.Generic;
    using System; 

    public enum EnvType
    {
        CODETODO,
        TASKLIST, 
        USER
    }

    [InitializeOnLoad]
    public class UTodoWindow : EditorWindow 
    {
        [InitializeOnLoadMethod]
        public static void InitializeMethod()
        {
            m_todos = new Dictionary<int, TodoListItem>();
            m_showTodos = new List<TodoListItem>();
        }

        private Vector2 m_treePos;
        private Vector2 m_listPos;
        private Vector2 m_tasksPos;

        private string m_searchContent = null;
        private EnvType m_workEnv = EnvType.CODETODO;

        [SerializeField] private TreeViewState m_TreeViewState;
        private static CatalogTree m_catalogTree;
        [SerializeField] private TreeViewState m_todolistTreeState;
        private static TodolistTree m_todolistTree;

        private static Dictionary<int, TodoListItem> m_todos = null;
        private static List<TodoListItem> m_showTodos = null;

        // UTask
        public static UTaskObject TaskRec = null; 
        private UTaskSetting m_uTaskSetting = new UTaskSetting();

        // UUser
        public static UTodoUserGroup UserGroup = null;

        private void Update()
        {

        }

        private void OnEnable()
        {
            if (m_TreeViewState == null)
            {
                m_TreeViewState = new TreeViewState();
                m_todolistTreeState = new TreeViewState();
            }
            m_catalogTree = new CatalogTree(m_TreeViewState);
            m_todolistTree = new TodolistTree(m_todolistTreeState);
            // 注册事件
            m_catalogTree.refreshList += () =>
            {
                m_showTodos.Clear();
                m_todolistTree.Reset();
            };
            m_catalogTree.contextClick += (id) =>
            {
                if (id < 4)
                    return;
                m_showTodos.Add(m_todos[id]);
                for (int i = 0; i < m_showTodos.Count; i++)
                {
                    var item = m_showTodos[i];
                    for (int j = 0; j < item.m_items.Count; j++)
                    {
                        var todoItem = item.m_items[j];
                        m_todolistTree.AddChild(new TreeViewItem(j + 4, 1, todoItem.todoContext));
                    }
                }
                m_todolistTree.ExpandAll();
            };
            m_catalogTree.contextDoubleClick += (id) =>
            { 
                if (id < 4)
                    return;
                var todo = m_todos[id];
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(todo.scriptPath, todo.m_items[0].lineCount);
            };
            GetFiles(Application.dataPath);

            // 加载task数据
            TaskRec = AssetDatabase.LoadAssetAtPath<UTaskObject>("Assets/TODOLIST/TaskRec.asset");
            UserGroup = AssetDatabase.LoadAssetAtPath<UTodoUserGroup>("Assets/TODOLIST/UserDB.asset");
            m_catalogTree.ExpandAll();
        } 

        private void OnGUI() 
        { 
            int sum = 0;
            foreach (var todos in m_todos)
                sum += todos.Value.m_items.Count;
            EditorGUILayout.LabelField("目前共有" + sum + "件事项，合计" + m_todos.Count + "个脚本加入待办事项列表");
            
            DrawToolbar(); 
            switch (m_workEnv)
            {
                case EnvType.CODETODO:
                    DrawCodeTodoPage();
                    break;
                case EnvType.TASKLIST:
                    DrawTodolistPage();
                    break; 
                case EnvType.USER:
                    DrawUserPage();
                    break;
            } 
        }
        private void DrawToolbar()
        {
            using (new HorizontalBlock(EditorStyles.toolbar))
            {
                EditorGUILayout.LabelField("Working Environment", EditorStyles.boldLabel, GUILayout.Width(150));
                m_workEnv = (EnvType)EditorGUILayout.EnumPopup(m_workEnv, EditorStyles.toolbarPopup, GUILayout.Width(110));
                switch (m_workEnv)
                {
                    case EnvType.CODETODO:
                        if (GUILayout.Button("Scan", EditorStyles.toolbarButton))
                            OnEnable();
                        break;
                    case EnvType.TASKLIST:
                        if (GUILayout.Button("Add Task", EditorStyles.toolbarButton))
                        {
                            // 显示增加界面
                            AddTaskWindow.OpenWindow();
                        }
                        break; 
                    case EnvType.USER:

                        break;
                }

                GUILayout.FlexibleSpace();
                m_searchContent = DrawSearchField(m_searchContent, GUILayout.Width(200));
            }
        }

        #region Draw Code Todo Page 

        private void DrawCodeTodoPage()
        { 
            using (new GUILayout.HorizontalScope())
            { 
                using (new VerticalBlock(GUI.skin.box, GUILayout.Width(position.width / 4f), GUILayout.ExpandHeight(true)))
                { 
                    Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
                    m_catalogTree.OnGUI(rect); 
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
                        // todo 绘制todolist 树
                        //Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
                        //if (m_todolistTree != null)
                        //    m_todolistTree.OnGUI(rect);
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
                    // 根据等级区分颜色
                    Color levelColor = UTodoColor.GetColorByLevel((UTaskLevel)item.todoLevel);
                    using (new HorizontalBlock(GUILayout.Height(40),GUILayout.ExpandWidth(true)))
                    {
                        GUI.color = levelColor;
                        using (new VerticalBlock(EditorStyles.textArea,GUILayout.Width(20),GUILayout.ExpandHeight(true)))
                        {
                            GUILayoutUtility.GetRect(0, 100000, 0, 100000);
                        }
                        GUI.color = Color.white;
                        using (new VerticalBlock())
                        {
                            using (new HorizontalBlock())
                            { 
                                GUILayout.Label("LEVEL" + item.todoLevel); 
                                GUILayout.FlexibleSpace();
                                GUILayout.Label(item.scriptPath, EditorStyles.miniBoldLabel);
                            }
                            GUILayout.Space(5f);
                            GUILayout.Label(item.todoContext, EditorStyles.largeLabel);
                            
                        } 
                        GUILayout.FlexibleSpace();
                        using (new GUILayout.VerticalScope(GUILayout.Width(position.width/10),GUILayout.ExpandHeight(true)))
                        {
                            GUI.color = Color.red;
                            if (GUILayout.Button("delete"))
                            {

                            }
                            GUI.color = Color.green;
                            if (GUILayout.Button("finish"))
                            {

                            }
                            GUI.color = Color.white;
                        }
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

        #endregion

        #region Draw Todo List Page
         
        private void DrawTodolistPage()
        { 
            using (new GUILayout.HorizontalScope())
            { 
                using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(position.width / 4), GUILayout.ExpandHeight(true)))
                {
                    using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                    {
                        EditorGUILayout.LabelField("UTask Tree", EditorStyles.label,GUILayout.Width(80)); 
                        GUILayout.FlexibleSpace(); 
                    }
                }

                using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                {
                    using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                    {
                        EditorGUILayout.LabelField("Order by", EditorStyles.label, GUILayout.Width(60));
                        m_uTaskSetting.orderById = GUILayout.Toggle(m_uTaskSetting.orderById, "Id", EditorStyles.toolbarButton);
                        m_uTaskSetting.orderByLevel = GUILayout.Toggle(m_uTaskSetting.orderByLevel, "Level", EditorStyles.toolbarButton);
                        m_uTaskSetting.orderByState = GUILayout.Toggle(m_uTaskSetting.orderByState, "State", EditorStyles.toolbarButton);

                        GUILayout.FlexibleSpace();
                        GUI.color = m_uTaskSetting.planningColor ;
                        m_uTaskSetting.showPlanningTask = GUILayout.Toggle(m_uTaskSetting.showPlanningTask, "Planning", EditorStyles.toolbarButton);
                        GUI.color = m_uTaskSetting.developingColor;
                        m_uTaskSetting.showDevelopingTask = GUILayout.Toggle(m_uTaskSetting.showDevelopingTask, "Developing", EditorStyles.toolbarButton);
                        GUI.color = m_uTaskSetting.finishedColor;
                        m_uTaskSetting.showFinishedTask = GUILayout.Toggle(m_uTaskSetting.showFinishedTask, "Finished", EditorStyles.toolbarButton);
                        GUI.color = Color.white; 
                    }

                    using (new GUILayout.ScrollViewScope(m_tasksPos))
                    {
                        DrawTsakList();
                    }
                }
            }
        }

        private void DrawTsakList()
        {
            using (new GUILayout.VerticalScope())
            {
                for (int i = 0; i < TaskRec.tasks.Count; i++)
                {
                    UTsak task = TaskRec.tasks[i];
                    GUI.color = UTodoColor.GetColorByLevel(task.level);
                    using (new GUILayout.HorizontalScope(GUI.skin.box, GUILayout.Height(40), GUILayout.ExpandHeight(true)))
                    {
                        GUI.color = Color.white;
                        using (new GUILayout.HorizontalScope( GUILayout.Width(20), GUILayout.ExpandHeight(true)))
                        {
                            if (GUILayout.Button(UTodoIcon.GetTypeIcon(task.type.ToString().ToLower()), GUILayout.Width(30), GUILayout.Height(30)))
                            {

                            }
                            if (GUILayout.Button(UTodoIcon.GetFlagIcon("yellow"), GUILayout.Width(30), GUILayout.Height(30)))
                            {

                            }
                        } 
                        using (new VerticalBlock(GUILayout.ExpandHeight(true)))
                        {
                            using ( new GUILayout.HorizontalScope())
                            {
                                GUILayout.Label("任务序号" + (i + 1));
                                GUILayout.Label(task.name);
                                GUILayout.Label("负责人："+task.pricipal); 
                            }
                            using (new GUILayout.HorizontalScope())
                            { 
                                GUILayout.Label("任务内容："+task.context);
                            }
                        }
                        GUILayout.FlexibleSpace();
                        using (new GUILayout.HorizontalScope(GUILayout.Width(position.width / 15),GUILayout.ExpandHeight(true)))
                        { 
                            GUILayout.FlexibleSpace(); 
                            if (GUILayout.Button(UTodoIcon.GetForkIcon("red"), GUILayout.Width(30), GUILayout.Height(30)))
                            {
                                TaskRec.RemoveTask(task);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Draw User Page

        private bool m_showLoginWindow = true;
        private bool m_isHost = false;
        private string m_userName = "";
        private string m_userPassword = "";
        private string m_hostIP = "192.168.9.130";
        private int m_hostPort = 33633;
        private UTodoServer utodoServer;
        private UTodoClient utodoClient;
        private bool m_isBillboard = true;
        private bool m_isChatroom = false;
        private Vector2 m_notificationsPos;
        private string m_currentChatContent;

        public void DrawUserPage()
        {
            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(position.width / 4), GUILayout.ExpandHeight(true))) 
                {
                    using (new GUILayout.VerticalScope(GUI.skin.box,GUILayout.ExpandWidth(true)))
                    {
                        if (GUILayout.Button("登录主机"))
                        {
                            m_showLoginWindow = !m_showLoginWindow;
                        }
                        if (m_showLoginWindow)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField("Account",GUILayout.Width(60)); 
                                m_userName = EditorGUILayout.TextField(m_userName);
                            }
                            using (new GUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField("Password", GUILayout.Width(60)); 
                                m_userPassword = EditorGUILayout.PasswordField(m_userPassword);
                            }
                            using (new GUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField("HostIP", GUILayout.Width(60)); 
                                m_hostIP = EditorGUILayout.TextField(m_hostIP);
                            }
                            GUI.color = new Color(0.5f, 1, 0.5f);
                            if (GUILayout.Button("确定登录"))
                            {
                                utodoClient = new UTodoClient();
                                utodoClient.connect(m_hostIP, m_hostPort);
                            }
                            GUI.color = Color.white;
                        }
                    }
                    using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.ExpandWidth(true)))
                    {
                        GUI.color = new Color(1, 0.3f, 0.3f);
                        if (GUILayout.Button("申请为主机"))
                        {
                            // 启动Server端socket的过程，带有简单的动画
                            // 启动成功显示主机IP port方便其他客户端连接
                            utodoServer = new UTodoServer(m_hostIP, m_hostPort);
                            m_isHost = !m_isHost;
                        }
                        GUI.color = Color.white;
                        if (m_isHost)
                        {
                            using (new GUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                            {
                                EditorGUILayout.LabelField("IP", GUILayout.Width(60));
                                EditorGUILayout.LabelField(m_hostIP);
                            }
                            using (new GUILayout.HorizontalScope(GUILayout.ExpandWidth(true))) 
                            {
                                EditorGUILayout.LabelField("Port", GUILayout.Width(60));
                                EditorGUILayout.LabelField(m_hostPort.ToString() );
                            }
                            using (new GUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                            {
                                EditorGUILayout.LabelField("Time", GUILayout.Width(60));
                                EditorGUILayout.LabelField(m_hostPort.ToString());
                            }
                            using (new GUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                            {
                                EditorGUILayout.LabelField("Online", GUILayout.Width(60));
                                EditorGUILayout.LabelField(utodoServer.OLCount.ToString() + "人");
                            }
                            if (GUILayout.Button("关闭服务器"))
                            {
                                // 启动Server端socket的过程，带有简单的动画
                                // 启动成功显示主机IP port方便其他客户端连接
                                utodoServer.Close();
                                m_isHost = !m_isHost;
                            }
                        }  
                    } 
                } 
                using (new GUILayout.VerticalScope(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                {
                    using (new GUILayout.VerticalScope(GUI.skin.box,GUILayout.Height(position.height / 4), GUILayout.ExpandWidth(true)))
                    {
                        using (new GUILayout.HorizontalScope(EditorStyles.toolbar,GUILayout.ExpandWidth(true)))
                        {
                            EditorGUILayout.LabelField("Notifications"); 
                        }
                        using (new GUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                        {
                            using (new ScrollviewBlock(ref m_notificationsPos))
                            {
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                                GUILayout.Label("xxx");
                            }
                        } 
                    }
                    using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true)))
                    {
                        using (new GUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.ExpandWidth(true)))
                        {
                            EditorGUILayout.LabelField("Chatroom");
                        }
                        using (new GUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
                        {
                            using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.ExpandWidth(true)))
                            {
                                GUILayout.Label("haha , 这事第一条消息");
                                GUILayoutUtility.GetRect(0, 10000, 0, 10000);
                                
                            }
                            GUILayout.FlexibleSpace();
                            using (new GUILayout.VerticalScope(GUI.skin.box,GUILayout.Height(position.height / 10), GUILayout.ExpandWidth(true)))
                            {
                                using (new GUILayout.HorizontalScope(GUILayout.ExpandHeight(true)))
                                {
                                    m_currentChatContent = GUILayout.TextArea(m_currentChatContent,GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                                    if (GUILayout.Button("Send",GUILayout.Width(50),GUILayout.ExpandHeight(true)))
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

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

        #region Window 相关基本操作  
        [MenuItem("UTodo/Open")]
        public static void OpenTodolist()
        {
            var window = GetWindow<UTodoWindow>();
            window.minSize = new Vector2(400, 300);
            window.titleContent = new GUIContent("UTodo");
            window.Show();
            window.ShowNotification(new GUIContent("欢迎使用UTodo插件"));
        } 
        #endregion
          
        #region 检索脚本中关键字核心算法

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
        private static void ReadFile(string filePath, string fileName, string type)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                List<TodoItem> todos = new List<TodoItem>();
                int lineCounter = 0;
                while (reader.Peek() > -1)
                {
                    lineCounter++;
                    string content = reader.ReadLine();
                    if (string.IsNullOrEmpty(content))
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
                        m_catalogTree.AddSharpChild(new TreeViewItem(m_todos.Count + 3, 2, fileName));
                    if (type == "lua")
                        m_catalogTree.AddLuaChild(new TreeViewItem(m_todos.Count + 3, 2, fileName));
                }
            }
        }
         
        #endregion
    }

    #region 准备去掉的内容

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

    #endregion
}