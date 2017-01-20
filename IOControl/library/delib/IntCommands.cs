using System;

public class IntCommands
{
	// --------------------------------------------------------------------
	// --------------------------------------------------------------------
	// --------------------------------------------------------------------
	// --------------------------------------------------------------------

	public class ModuleID
        {
			// all Modul-ID's
            public const uint USB_Interface8 					= 1;
            public const uint USB_CAN_STICK 					= 2;
            public const uint USB_LOGI_500 						= 3;
            public const uint USB_SER_DEBUG						= 4;
            public const uint RO_SER 							= 5;
            public const uint USB_BITP_200 						= 6;
            public const uint RO_USB1 							= 7;
			public const uint RO_USB							= 7;
            public const uint RO_ETH 							= 8;
            public const uint USB_MINI_STICK 					= 9;
            public const uint USB_LOGI_18 						= 10;
            public const uint RO_CAN 							= 11;
            public const uint USB_SPI_MON 						= 12;
            public const uint USB_WATCHDOG 						= 13;
			public const uint USB_OPTOIN_8 						= 14;
			public const uint USB_RELAIS_8 						= 14;
			public const uint USB_OPTOIN_8_RELAIS_8 			= 15;
			public const uint USB_OPTOIN_16_RELAIS_16			= 16;
			public const uint USB_OPTOIN_32						= 16;
			public const uint USB_RELAIS_32						= 16;
			public const uint USB_OPTOIN_32_RELAIS_32			= 17;
			public const uint USB_OPTOIN_64						= 17;
			public const uint USB_RELAIS_64						= 17;
			public const uint BS_USB_8							= 15;
			public const uint BS_USB_16							= 16;
			public const uint BS_USB_32							= 17;
			public const uint USB_TTL_32						= 18;
			public const uint USB_TTL_64						= 18;
			public const uint RO_ETH_INTERN						= 19;

			public const uint BS_SER							= 20;
			public const uint BS_CAN							= 21;
			public const uint BS_ETH							= 22;

			public const uint NET_ETH							= 23;

			public const uint RO_CAN2							= 24;
			public const uint RO_USB2							= 25;
			public const uint RO_ETH_LC							= 26;
			//
            public const uint ETH_RELAIS_8                      = 27;
            public const uint ETH_OPTOIN_8                      = 27;
            public const uint ETH_O4_R4_ADDA                    = 28;
            //
            public const uint ETHERNET_MODULE                   = 29;
            //
            public const uint ETH_TTL_64                        = 30;
            //
            public const uint NET_USB                           = 31;
        }

	// --------------------------------------------------------------------
	// --------------------------------------------------------------------
	// --------------------------------------------------------------------
	// --------------------------------------------------------------------

	public static String DapiInternGetModuleName(uint moduleID)
	{

		switch(moduleID)
		{
			case ModuleID.USB_Interface8:				return "USB-TTL-IN8-OUT8";
			case ModuleID.USB_CAN_STICK:				return "USB-CAN-Stick";	
			case ModuleID.USB_LOGI_500:					return "USB-LOGI-500";	
			case ModuleID.USB_SER_DEBUG:				return "USB-SER-DEBUG";	
			case ModuleID.USB_BITP_200:					return "USB-BITP-200";	
			case ModuleID.USB_OPTOIN_8_RELAIS_8:		return "BS-USB-8";		
			case ModuleID.USB_OPTOIN_16_RELAIS_16:		return "BS-USB-16";		
			case ModuleID.USB_OPTOIN_32_RELAIS_32:		return "BS-USB-32";		
			case ModuleID.BS_CAN:						return "BS-CAN";		
			case ModuleID.RO_USB1:						return "RO-USB1";		
			case ModuleID.RO_SER:						return "RO-SER";		
			case ModuleID.BS_SER:						return "BS-SER";		
			case ModuleID.RO_CAN:						return "RO-CAN";		
			case ModuleID.RO_ETH:						return "RO-ETH";		
			case ModuleID.RO_ETH_LC:					return "RO-ETH/LC";		
			case ModuleID.RO_ETH_INTERN:				return "RO-ETH-INTERN";	
			case ModuleID.BS_ETH:						return "BS-ETH";		
			case ModuleID.NET_ETH:						return "NET-ETH";		
			case ModuleID.USB_WATCHDOG:					return "USB-Watchdog";	
			case ModuleID.USB_MINI_STICK:				return "USB-Mini-Stick";
			case ModuleID.USB_RELAIS_8:					return "USB-OPT/REL-8";
			case ModuleID.ETH_RELAIS_8:					return "ETH-OPT/REL-8";
			case ModuleID.ETH_O4_R4_ADDA:				return "ETH-O4-R4-ADDA";
			case ModuleID.ETH_TTL_64:					return "ETH-TTL-64";
			case ModuleID.ETHERNET_MODULE:				return "ETHERNET-MODULE";
			case ModuleID.USB_LOGI_18:					return "USB-Logi-18";
			case ModuleID.USB_SPI_MON:					return "USB-SPI-MON";
			case ModuleID.USB_TTL_64:					return "USB-TTL-64";
			case ModuleID.RO_USB2:						return "RO-USB2";
			case ModuleID.RO_CAN2:						return "RO-CAN2";
			case ModuleID.NET_USB:						return "NET-USB";
			default:									return "UNKNOWN";		
		}
	}
	// --------------------------------------------------------------------
	// --------------------------------------------------------------------
	// --------------------------------------------------------------------
	// --------------------------------------------------------------------
}


