using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOControl
{
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public class Sess
    {
        public const String MC_MSG_REFRESH = "refresh";

        // ------------------------------------
        // ------------------------------------
        // ------------------------------------

        public static XML.SessionXML Xml { get; set; } = new XML.SessionXML();
        public static List<DTControl.DOButton> Buttons { get; set; } = new List<DTControl.DOButton>();

        // ------------------------------------
        // ------------------------------------
        // ------------------------------------

        public static void LogEX(String func, String ex) { xLog(func, ex); }
        public static void Log(String text) { xLog(null, text); }
        static void xLog(String func, String text)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("{0} ", ((func != null) ? "** EX " + func + " **\n" : ""), text));
        }

        // ------------------------------------
        // ------------------------------------
        // ------------------------------------
    }
}
