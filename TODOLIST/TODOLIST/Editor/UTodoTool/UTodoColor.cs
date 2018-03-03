 
using UnityEngine;

namespace UTODO
{
    public static class UTodoColor
    {
        public static Color error = Color.red;
        public static Color warning = Color.yellow;
        public static Color normal = Color.green;

        public static Color errorBackground = new Color(1, 0.5f, 0.5f);
        public static Color warningBackground = new Color(1,1,0.5f);
        public static Color normalBackground = new Color(0.5f,1,0.5f);

        public static Color GetColorByLevel( UTaskLevel level )
        {
            switch (level)
            {
                case UTaskLevel.General:
                    return UTodoColor.normalBackground;
                case UTaskLevel.Prior:
                    return UTodoColor.warningBackground;
                case UTaskLevel.Urgency:
                    return UTodoColor.errorBackground;
                default:
                    return new Color(1,0.5f,1);
            }
        }
    }
}