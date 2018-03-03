 
using UnityEditor;
using UnityEngine;

namespace UTODO
{ 
    public static class UTodoIcon
    { 
        public static Texture GetFlagIcon( string flagColor )
        {
            string iconName = "utodo_flag_" + flagColor + ".png";
            return AssetDatabase.LoadAssetAtPath<Texture>("Assets/TODOLIST/Icons/" + iconName);
        }

        public static Texture GetMarkIcon( string markColor )
        {
            string iconName = "utodo_mark_" + markColor + ".png";
            return AssetDatabase.LoadAssetAtPath<Texture>("Assets/TODOLIST/Icons/" + iconName);
        }

        public static Texture GetTypeIcon(string typeName)
        {
            string iconName = "utodo_type_" + typeName + ".png";
            return AssetDatabase.LoadAssetAtPath<Texture>("Assets/TODOLIST/Icons/" + iconName);
        }

        public static Texture GetForkIcon(string forkColor)
        {
            string iconName = "utodo_fork_" + forkColor + ".png";
            return AssetDatabase.LoadAssetAtPath<Texture>("Assets/TODOLIST/Icons/" + iconName);
        }

        public static Texture GetCheckIcon(string checkColor)
        {
            string iconName = "utodo_check_" + checkColor + ".png";
            return AssetDatabase.LoadAssetAtPath<Texture>("Assets/TODOLIST/Icons/" + iconName);
        }
    } 
}