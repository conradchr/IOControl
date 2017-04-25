using System;
using System.Collections.Generic;   // List
using Xamarin.Forms;    // Color

using DELIB;
using DeLib;
using IOControl;

public partial class DT
{

    public static int TEMP_MESSE_CH_NAME_CNT;

    public static Color COLOR = Color.FromHex("#996d0a12");
    public static Color COLOR_SELECTED = Color.FromHex("#A0AAAAAA");

    public static Color COLOR_POPUP = Color.FromHex("#282828");
    public static Color COLOR_POPUP_HEADER = Color.FromRgb(49, 180, 224);
    
    public static List<DT_BCFunctions.ETHDeviceConfig> eth_devs = new List<DT_BCFunctions.ETHDeviceConfig>();

    public const uint RETURN_OK =       0;
    public const uint RETURN_ERROR =    1;
    public const int NULL = -1337;
    //public const object object_null = new object();

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
}