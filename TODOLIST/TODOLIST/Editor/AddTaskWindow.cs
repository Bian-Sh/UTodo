
using System;
using UnityEngine;
using UnityEditor;

namespace UTODO
{
    public class AddTaskWindow : EditorWindow
    {
        private static AddTaskWindow window;

        public static void OpenWindow() 
        {
            window = GetWindow<AddTaskWindow>(false);
            window.minSize = new Vector2(400, 200);
            window.titleContent = new GUIContent("Add Task");
            window.Show(); 
        }

        private Vector2 m_scrollPos;

        public int tId ;
        public UTaskLevel tLevel = UTaskLevel.Prior;
        public UTaskType tType = UTaskType.Code;
        public string tName = "New Task";
        public string tContext = "New Context";
        public string tPricipal = "springdong";// 主要负责人

        public DateTime tInitDate;
        public DateTime tStartDate;
        public DateTime tEndDate;
        public UTaskState tState;

        private int userIndex = 0;

        private void OnGUI()
        {
            using (new GUILayout.ScrollViewScope(m_scrollPos))
            {
                using (new GUILayout.VerticalScope())
                {
                    GUI.color = new Color(0.85f, 0.85f, 0.85f);
                    using (new GUILayout.VerticalScope(GUI.skin.box,GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                    {
                        GUI.color = Color.white;
                        using (new GUILayout.HorizontalScope())
                        {  
                            GUILayout.Label("Task Index",GUILayout.Width(90));
                            tId = UTodoWindow.TaskRec.NextTaskId;
                            EditorGUILayout.IntField(tId); 
                        }
                    }
                    GUI.color = UTodoColor.GetColorByLevel(tLevel);
                    using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                    {
                        GUI.color = Color.white;
                        using (new GUILayout.HorizontalScope())
                        { 
                            GUILayout.Label("Task Level", GUILayout.Width(90)); 
                            tLevel = (UTaskLevel)EditorGUILayout.EnumPopup(tLevel);
                        }
                    }
                    using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                    { 
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label("Task Type", GUILayout.Width(90));
                            tType = (UTaskType)EditorGUILayout.EnumPopup(tType); 
                        }
                    }
                    GUI.color = new Color(0.85f, 0.85f, 0.85f);
                    using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                    {
                        GUI.color = Color.white;
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label("Task Pricipal", GUILayout.Width(90));
                            string[] names = UTodoWindow.UserGroup.userNames;
                            userIndex = EditorGUILayout.Popup(userIndex, names);
                            tPricipal = names[userIndex];
                        }
                    } 
                    using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                    { 
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label("Task Name", GUILayout.Width(90));
                            tName = EditorGUILayout.TextField(tName);
                        }
                    }
                    GUI.color = new Color(0.85f, 0.85f, 0.85f);
                    using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                    {
                        GUI.color = Color.white;
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label("Task Context", GUILayout.Width(90));
                            tContext = EditorGUILayout.TextField(tContext);
                        }
                    } 
                } 

                GUILayout.FlexibleSpace();
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    if (GUILayout.Button("确定添加"))
                    {
                        UTsak task = new UTsak();
                        task.id = tId;
                        task.level = tLevel;
                        task.name = tName;
                        task.context = tContext;
                        task.pricipal = tPricipal;
                        task.type = tType;
                        UTodoWindow.TaskRec.AddTask(task);
                       
                        tId = -1;
                        tLevel = UTaskLevel.Prior;
                        tName = "New Task";
                        tContext = "New Context";
                        tPricipal = "springdong";

                        // 先提示
                        window.Close();
                    }
                    if (GUILayout.Button("关闭窗口"))
                    {
                        window.Close();
                    }
                }
            }
        }
    }
}