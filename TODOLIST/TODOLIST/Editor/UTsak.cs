 
using System;
using UnityEngine;

namespace UTODO
{
    public enum UTaskState
    {
        Planning,   //规划期
        Developing, //开发期
        Finish      //已完成
    }

    public enum UTaskLevel : int
    {
        General = 3,
        Prior =2,
        Urgency =1,
        Special = 0
    }

    public enum UTaskType : int
    {
        Code,
        Doc,
        Interface,
    }

    [Serializable]
    public class UTsak
    {
        public int id;
        public UTaskLevel level;
        public string name;
        public string context;
        public string pricipal = "springdong";// 主要负责人
        public string icon = "flag_red";
        public UTaskType type = UTaskType.Code;
        public DateTime initDate;
        public DateTime startDate;
        public DateTime endDate;
        public UTaskState state;

        public UTsak()
        {
            initDate = DateTime.Now;
            state = UTaskState.Planning;
        }

        public UTsak( string taskName, UTaskLevel taskLevel,string taskContext )
        {
            this.name = taskName;
            this.level = taskLevel;
            this.context = taskContext;
            initDate = DateTime.Now;
        }

        public void Start()
        {
            startDate = DateTime.Now;
            state = UTaskState.Developing;
        }

        public void End()
        {
            startDate = DateTime.Now;
            state = UTaskState.Finish;
        }
    }

    public class UTaskSetting
    {
        public bool showFinishedTask = true;
        public bool showPlanningTask = true;
        public bool showDevelopingTask = true;

        public bool orderById = true;
        public bool orderByLevel = false;
        public bool orderByState = false;

        public Color planningColor = Color.cyan;
        public Color developingColor = Color.yellow;
        public Color finishedColor = Color.green; 
    }
}