using System;
using System.Collections.Generic;   // List
using Xamarin.Forms;    // Color

using DELIB;
using DeLib;
using IOControl;

public partial class DT
{
    public static Color COLOR = Color.FromHex("#996d0a12");
    public static Color COLOR_SELECTED = Color.FromHex("#A0AAAAAA");

    public static Color COLOR_POPUP = Color.FromHex("#282828");
    public static Color COLOR_POPUP_HEADER = Color.FromRgb(49, 180, 224);
    
    public static List<DT_BCFunctions.ETHDeviceConfig> eth_devs = new List<DT_BCFunctions.ETHDeviceConfig>();

    public const uint RETURN_OK =       0;
    public const uint RETURN_ERROR =    1;
    public const int NULL = -1337;

    public static uint dummy_uint = 0;
    public static byte[] dummy_byte = new byte[] { 0 };

    public class Error : DeLibErrorCodes { };
    public class Conv : DT_ConvertUtils { };
    
	public class Delib : DeLibNET { };

	public class String : DT_StringUtils { };
	public class Bc : DT_BCFunctions { };
    public class File: DT_FileUtils { };
    public class ETH : DT_ETH { };

	// neu
	//public class CP : DT_CommunicationParams { };
    
    public class ENC : DT_EncryptionUtils { };
    
    //public class DBG : DT_DebugUtils { };
	public class TCP : DT_TCPUtils { };
	//public class DELIB : DT_DELIB { };
    
	public class Mobile
	{
		public class BC : DT_Mobile_BCFunctions { };
		//public class SM : DT_AndroidStorageManager { };
		//public class GUI : GUIBuilder { };
	}
    
	public class Session
	{
        public static XMLContent xmlContent = new XMLContent();
        public static int deviceIP;

        // TabItems 
        public static List<string>tabItems = new List<string>();

        // IO Refresh + Set
        //public static System.Threading.Thread todoThread;
        //public static bool todoThreadActive = false;
        //public static int todoThreadTimer = 0;

        // IO Refresh
        //public static List<ModuleTask>[] refreshTasks;
        //public static bool refreshTaskInit;

        // IO Set
        //public static bool blockTodoTask = false;
        //public static List<Action> todoTasks = new List<Action>();
	}

    public class Const
    {
        // Config File
        public const int CONFIG_FILE_MAX = 10;
        //public const float CONFIG_FILE_VERSION = 1.0F;

        public const int CONFIG_FILE_VERSION_CONTENT = 100;
        public const int CONFIG_FILE_VERSION_MODULE = 100;
        public const int CONFIG_FILE_VERSION_CONFIG = 100;

        public const string CONFIG_FILES_DIRECTORY = "DEDITEC.IO.Control";

        public const string CONFIG_FILE_NAME_CONTENT = "custom_view_{0}.xml";
        public const string CONFIG_FILE_NAME_MODULE = "modules.xml";
        public const string CONFIG_FILE_NAME_CONFIG = "app_config.xml";

        //public static string CONFIG_FILE_DIR

        public const int TIME_ANIMATION_MIN_MS = 600;


        public const string FRAGMENT_ID = "curFragment";

        public const string MSG_REFRESH = "Refresh";

    }

//    public static Dictionary<int, Xamarin.Forms.View> pageControls = new Dictionary<int, Xamarin.Forms.View>();

    public static void Log(System.String text)
    {
        System.Diagnostics.Debug.WriteLine(text);
    }
}