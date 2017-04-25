using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;    // Color

namespace IOControl
{
    public class GUI
    {
        public const int TIME_ANIMATION_MIN_MS = 600;

        public static Color COLOR_DEFAULT = Color.FromHex("#996d0a12");
        public static Color COLOR_SELECTED = Color.FromHex("#A0AAAAAA");
        public static Color COLOR_POPUP = Color.FromHex("#282828");
        public static Color COLOR_POPUP_HEADER = Color.FromRgb(49, 180, 224);

        //public class Animation : GUIAnimation { };
    }
}
