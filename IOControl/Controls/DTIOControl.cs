using System.Collections.Generic;
using Xamarin.Forms;

namespace IOControl
{
    public class DTIOControl
    {

        public static Layout PWMView(Dictionary<int, View> dict, string text, int id)
        {
            return DTControl.Slider(dict, text, id);
        }

        public static Layout DOSwitch()
        {
            return DTControl.Switch("Digital Output 1", true, false);
        }

        public static Layout DOSwitch(Dictionary<int, View> dict, string text, int id)
        {
            return DTControl.Switch(dict, text, true, false, id);
        }

        public static Layout DISwitch()
        {
            return DTControl.Switch("Digital Input 2 (on)", false, true);
        }

        public static Layout DISwitch(Dictionary<int, View> dict, string text, int id)
        {
            return DTControl.Switch(dict, text, false, false, id);
        }

        public static Layout DISwitch2()
        {
            return DTControl.Switch("Digital Input 3 (off)", false, false);
        }

        public static Layout ADView(Dictionary<int, View> dict, string text, int id)
        {
            return DTControl.View(dict, text, DTControl.IOViewStyle.AD, id);

        }

        //public static Layout View(Dictionary<int, View> dict, string text, IOViewStyle style, float value, int id)

        /*
        public static Layout DAView()
        {
            //return DTControl.View("Analog Output 5", DTControl.IOViewStyle.DA);
        }

        public static Layout TempView()
        {
            //return DTControl.View("Temperature Input 6", DTControl.IOViewStyle.TEMP);
        }

        public static Layout TempView2()
        {
            //return DTControl.View("Temperature Input 7", DTControl.IOViewStyle.TEXT);
        }
        */

    }
}
