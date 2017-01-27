//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//
//
//
//	delib_extended_functions.cs
//	project: DELIB
//
//
//	(c) DEDITEC GmbH, 2017
//	web: http://www.deditec.de/
//	mail: vertrieb@deditec.de
//
//
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

public partial class DT
{
    public class Ext : DeLib.DeLibNETExtended { };
}

namespace DeLib
{
    public class DeLibNETExtended
    {
        #if (!__MOBILE__)
            [DllImport("delib64.dll")]
            public static extern uint DapiProgDevice(uint moduleID, uint moduleNr, uint todo, uint mode, [MarshalAs(UnmanagedType.LPStr)]String filename, uint function);
            [DllImport("delib64.dll")]
            public static extern uint DapiProgDeviceNextState(uint prog_handle, StringBuilder msg, uint msg_size, ref uint progress_percent);

            [DllImport("delib64.dll")]
            public static extern uint DapiOpenDebugChannel(uint moduleID, uint moduleNr);
            [DllImport("delib64.dll")]
            public static extern uint DapiReadDebugChannel(uint handle, [MarshalAs(UnmanagedType.LPArray)]byte[] buff, uint bufflen);
            [DllImport("delib64.dll")]
            public static extern uint DapiWriteDebugChannel(uint handle, [MarshalAs(UnmanagedType.LPArray)]byte[] buff, uint bufflen);
            [DllImport("delib64.dll")]
            public static extern uint DapiCloseDebugChannel(uint handle);

            [DllImport("delib64.dll")]
            public static extern uint DapiInternCommand(uint cmd, string par1, string par2, [MarshalAs(UnmanagedType.LPArray)]byte[] par3, uint par4, ref uint par5);
            [DllImport("delib64.dll")]
            public static extern uint DapiOpenModuleIntern(uint moduleID, uint moduleNr, uint subdevice, [MarshalAs(UnmanagedType.LPStr)]String exbuffer, uint open_options);
        #endif


        public const uint ProgDeviceErr_NONE					    	= 0;
        public const uint ProgDeviceErr_END					    		= 100;
        public const uint ProgDeviceErr_DeviceOpenError				    = 1;
		public const uint ProgDeviceErr_MemoryAllocError				= 5;
        public const uint ProgDeviceErr_FileNotFound				    = 2;
        public const uint ProgDeviceErr_FlashDataError				    = 3;
        public const uint ProgDeviceErr_SyncError					    = 4;

		// --------------------------------------------------------
		// DAPI_INTERN Commands (ohne handle)

        public const uint DAPI_INTERN_CMD_GET_REGISTRY_STRING		    = 1;
        public const uint DAPI_INTERN_CMD_SET_REGISTRY_STRING		    = 2;
        public const uint DAPI_INTERN_CMD_GET_REGISTRY_DWORD		    = 3;
        public const uint DAPI_INTERN_CMD_SET_REGISTRY_DWORD		    = 4;


        public const uint DAPI_INTERN_CMD_BC_GET_MAC_LIST			    = 5;
        public const uint DAPI_INTERN_CMD_BC_GET_PARAMETER			    = 6;
        public const uint DAPI_INTERN_CMD_BC_SET_PARAMETER			    = 7;
        public const uint DAPI_INTERN_CMD_BC_ETH0_CONFIG			    = 8;
        public const uint DAPI_INTERN_CMD_BC_GET_MAC_LIST_WITH_DEV_CFG  = 14;

		public const uint DAPI_INTERN_GET_MODULE_NAME					= 9;
		public const uint DAPI_INTERN_GET_MODULE_BUS_TYPE				= 10;
		public const uint DAPI_INTERN_GET_MODULE_INTERFACE_TYPE			= 11;
		public const uint DAPI_INTERN_GET_REGISTRY_KEY_NAME				= 12;
        public const uint DAPI_INTERN_GET_MODULE_CONFIG_TEXT            = 13;
        public const uint DAPI_INTERN_GET_TCP_MODULE_ID_FROM_BL_BOARD_ID= 15;

		// --------------------------------------------------------
		// DAPI_INTERN Commands - CMD=DAPI_INTERN_GET_MODULE_INTERFACE_TYPE - Return

        public const uint DAPI_INTERN_RET_INTERFACE_UNDEFINED  			= 0;
        public const uint DAPI_INTERN_RET_INTERFACE_CAN                 = 1;
        public const uint DAPI_INTERN_RET_INTERFACE_USB                 = 2;
        public const uint DAPI_INTERN_RET_INTERFACE_ETH                 = 3;
        public const uint DAPI_INTERN_RET_INTERFACE_SER                 = 4;
		
		// --------------------------------------------------------
		// DAPI_SPECIAL Commands (die NICHT in der DELIB.h stehen sollen)

        public const uint DAPI_SPECIAL_CMD_MODULE_CONFIG				= 0xf0000001;
        public const uint DAPI_SPECIAL_MODULECFG_MODULE_ID_SET			= 1;

        public const uint DAPI_SPECIAL_CMD_ROCPU_EEPROM_ERASE			= 0x10;
        public const uint DAPI_SPECIAL_CMD_ROCPU_EEPROM_WRITE_1K_2K		= 0x11;
        public const uint DAPI_SPECIAL_CMD_ROCPU_EEPROM_READ_1K_2K		= 0x12;

        public const uint DAPI_SPECIAL_CMD_ROCPU_SUBMODULE_GET_INFOS	= 0xf0001003;
        public const uint DAPI_SPECIAL_CMD_ROCPU_SUBMODULE_GO_BOOTLOADER= 0xf0001004;
        public const uint DAPI_SPECIAL_CMD_ROCPU_SUBMODULE_EEPROM_WRITE	= 0xf0001005;

        public const uint DAPI_SPECIAL_CMD_RS485_MODULE_NR_SET			= 0xf0000002;
        public const uint DAPI_SPECIAL_CMD_TCP_TX_RX_TESTDATA			= 0xf0000003;
        public const uint DAPI_SPECIAL_CMD_DELIB_SET_PROG_NAME			= 0xf0000004;
        public const uint DAPI_SPECIAL_CMD_RENESAS_RESET				= 0xf0000005;
        public const uint DAPI_SPECIAL_CMD_FT_CBUS_SET					= 0xf0000006;
        public const uint DAPI_SPECIAL_CMD_FT_CBUS_GET					= 0xf0000007;
        public const uint DAPI_SPECIAL_CMD_RENESAS_RESET_MODE0          = 0xf0000008;
        public const uint DAPI_SPECIAL_CMD_GET_PKT_STAT                 = 0xf0000009;
        public const uint DAPI_SPECIAL_CMD_SET_OUTPUT_WITH_ENCRYPTION   = 0xffef0123;
		// EXT
        public const uint DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_CONFIG_DATA= 0xf0000009;
        public const uint DAPI_SPECIAL_CMDEXT_TCP_MODULE_SET_CONFIG_DATA= 0xf000000A;
        public const uint DAPI_SPECIAL_CMDEXT_TCP_MODULE_IDENTIFY       = 0xf000000B;
        public const uint DAPI_SPECIAL_CMDEXT_TCP_MODULE_ACTIVATE_ADMIN_TEMP = 0xf000000C;
        public const uint DAPI_SPECIAL_CMDEXT_TCP_MODULE_RELOAD_ETH_CONFIG = 0xf000000D;
        public const uint DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_CURRENT_CONFIG = 0xf000000E;
        public const uint DAPI_SPECIAL_CMDEXT_TCP_MODULE_SET_MAC_ADDR   = 0xf000000F;
        public const uint DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_MAC_ADDR   = 0xf0000010;
        public const uint DAPI_SPECIAL_CMDEXT_READ_EE_DIRECTORY			= 0xf0000011;
        public const uint DAPI_SPECIAL_CMDEXT_WRITE_EE_DIRECTORY		= 0xf0000012;
        public const uint DAPI_SPECIAL_CMDEXT_CALL_SPECIAL_CMD_TYP_BUFF = 0xf00000FF;
        public const uint DAPI_SPECIAL_CMDEXT_DELIB_SET_PROG_NAME		= 0xf0000013;
        public const uint DAPI_SPECIAL_CMDEXT_ROCPU_SUBMODULE_GET_INFOS = 0xf0000014;
        public const uint DAPI_SPECIAL_CMDEXT_NETCPU_SUBMODULE_GET_INFOS= 0xf0000015;
        public const uint DAPI_SPECIAL_CMDEXT_ROCPU_EEPROM_READ_1K_2K   = 0xf0000016;
        public const uint DAPI_SPECIAL_CMDEXT_ROCPU_EEPROM_WRITE_1K_2K  = 0xf0000017;
        public const uint DAPI_SPECIAL_CMDEXT_SET_RTC                   = 0xf0000018;
        public const uint DAPI_SPECIAL_CMDEXT_GET_RTC					= 0xf0000019;
        public const uint DAPI_SPECIAL_CMDEXT_SUBMODULE_RW_BUFFER       = 0xf000001A;

        public const String DEDITEC_PARAM_IP_MAC_ADDR = "config-ip-mac-addr";

        // --------------------------------------------------------
        // par1 für DAPI_SPECIAL_CMD_GET_PKT_STAT
        public const uint DAPI_SPECIAL_CMD_GET_PKT_STAT_PAR_CNT_DELIB_ERROR_NONE				= 0;
        public const uint DAPI_SPECIAL_CMD_GET_PKT_STAT_PAR_CNT_DELIB_ERROR_CLASS_GEN			= 1;
        public const uint DAPI_SPECIAL_CMD_GET_PKT_STAT_PAR_CNT_DELIB_ERROR_CLASS_COM			= 2;
        public const uint DAPI_SPECIAL_CMD_GET_PKT_STAT_PAR_CNT_DELIB_ERROR_CLASS_DEV_PKT		= 3;
        public const uint DAPI_SPECIAL_CMD_GET_PKT_STAT_PAR_CNT_DELIB_ERROR_CLASS_DEV_IO		= 4;
        public const uint DAPI_SPECIAL_CMD_GET_PKT_STAT_PAR_CNT_DELIB_ERROR_CLASS_DEV_OTHER 	= 5;
        public const uint DAPI_SPECIAL_CMD_GET_PKT_STAT_PAR_TIME_MIN_TRANS_NORMAL 			    = 100;
        public const uint DAPI_SPECIAL_CMD_GET_PKT_STAT_PAR_TIME_MAX_TRANS_NORMAL 			    = 101;
        public const uint DAPI_SPECIAL_CMD_GET_PKT_STAT_PAR_CNT_RETRY_TRANS_NORMAL 			    = 102;
        public const uint DAPI_SPECIAL_CMD_GET_PKT_STAT_PAR_CNT_RECON_TRANS_NORMAL 			    = 103;
        public const uint DAPI_SPECIAL_CMD_GET_PKT_STAT_PAR_TIME_MIN_TRANS_MULTIPLE 			= 110;
        public const uint DAPI_SPECIAL_CMD_GET_PKT_STAT_PAR_TIME_MAX_TRANS_MULTIPLE  			= 111;
        public const uint DAPI_SPECIAL_CMD_GET_PKT_STAT_PAR_CNT_RETRY_TRANS_MULTIPLE            = 112;
        public const uint DAPI_SPECIAL_CMD_GET_PKT_STAT_PAR_CNT_RECON_TRANS_MULTIPLE            = 113;
    }
}
