//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//
//
//
//	delib_Dapi_io_commands.cs
//	project: DELIB
//
//
//	(c) DEDITEC GmbH, 2009-2014
//	web: http://www.deditec.de/
//	mail: vertrieb@deditec.de
//
//
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************

using IOControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Net;
//using System.Net.Sockets;


namespace DELIB
{
    public partial class DeLibNET
    {
        static byte[] global_temp_buffer = new byte[128 * 8];
        static byte[] global_da_buffer = new byte[128 * 8];
        static byte[] global_ad_buffer = new byte[128 * 4];
        static byte[] global_pwm_buffer = new byte[128];

        //
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // Declarations
		// ----------------------------------------------------------------------------	
		// Intern
		// ----------------------------------------------------------------------------	
		public const uint DEDITEC_TCP_START_ID_FOR_MULTIPLE_BYTE_DATA						= 35;
		// ----------------------------------------------------------------------------	
		public const uint DEDITEC_PACKET_ID_0												= 0x63;
		public const uint DEDITEC_PACKET_ID_1												= 0x9a;
		public const uint DEDITEC_TCP_ANSWER_RO_1											= 0x81;
		public const uint DEDITEC_TCP_ANSWER_OK												= 0;
		// ----------------------------------------------------------------------------	
        // ----------------------------------------------------------------------------
        // ERROR Codes
        public const uint DAPI_ERR_NONE                                                     = 0;
        public const uint DAPI_ERR_DEVICE_NOT_FOUND                                         = 0xffffffff;	// -1
        public const uint DAPI_ERR_COMMUNICATION_ERROR                                      = 0xfffffffe;	// -2
        public const uint DAPI_ERR_ILLEGAL_HANDLE                                           = 0xfffffff6;   //-10
        public const uint DAPI_ERR_FUNCTION_NOT_DEFINED                                     = 0xfffffff5;   //-11
        public const uint DAPI_ERR_ILLEGAL_COM_HANDLE                                       = 0xfffffff4;   //-12
        public const uint DAPI_ERR_ILLEGAL_MODE                                             = 0xfffffff3;   //-13
        public const uint DAPI_ERR_WITH_TEXT                                                = 0xfffffff2;	//-14
        public const uint DAPI_ERR_SW_FEATURE_NOT_SUPPORTED                              = 0xfffffff1;	//-15
       	public const uint DAPI_ERR_ILLEGAL_IO_TYPE                                       = 0xfffffff0;	//-16
        public const uint DAPI_ERR_ILLEGAL_CHANNEL                                       = 0xffffffef;	//-17

        // ----------------------------------------------------------------------------
        // Special Function-Codes
        public const uint DAPI_SPECIAL_CMD_GET_MODULE_CONFIG                                = 1;
        public const uint DAPI_SPECIAL_CMD_TIMEOUT                                          = 2;
        public const uint DAPI_SPECIAL_CMD_DI                                               = 10;
        public const uint DAPI_SPECIAL_CMD_SET_DIR_DX_1                                     = 3;
        public const uint DAPI_SPECIAL_CMD_SET_DIR_DX_8                                     = 4;
        public const uint DAPI_SPECIAL_CMD_GET_MODULE_VERSION                               = 5;
        public const uint DAPI_SPECIAL_CMD_DA                                               = 6;
        public const uint DAPI_SPECIAL_CMD_WATCHDOG                                         = 7;
        public const uint DAPI_SPECIAL_CMD_COUNTER                                          = 8;
        public const uint DAPI_SPECIAL_CMD_AD                                               = 9;
        public const uint DAPI_SPECIAL_CMD_CNT48                                            = 11;
        public const uint DAPI_SPECIAL_CMD_SOFTWARE_FIFO                                    = 12;
        public const uint DAPI_SPECIAL_CMD_MODULE_REBOOT                                    = 13;
        public const uint DAPI_SPECIAL_CMD_MODULE_RESCAN                                    = 14;
        public const uint DAPI_SPECIAL_CMD_RESTART_CHECK_MODULE_CONFIG                      = 15;
        public const uint DAPI_SPECIAL_CMD_TEMP                                             = 16;
        public const uint DAPI_SPECIAL_CMD_PWM                                              = 17;

        // values for PAR1
        public const uint DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_DI                             = 1;
        public const uint DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_DI_FF                          = 7;
        public const uint DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_DI_COUNTER                     = 8;
        public const uint DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_DO                             = 2;
        public const uint DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_DX                             = 3;
        public const uint DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_AD                             = 4;
        public const uint DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_DA                             = 5;
        public const uint DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_TEMP                           = 9;
        public const uint DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_STEPPER                        = 6;
        public const uint DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_CNT48                          = 10;
        public const uint DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_PULSE_GEN                      = 11;
        public const uint DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_PWM_OUT                        = 12;
        public const uint DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_HW_INTERFACE1               = 13;
        public const uint DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_SW_FEATURE1                 = 14;
        public const uint DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_HW_GROUP                    = 15;
        public const uint DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_SW_CLASS                    = 16;
        //
        public const uint DAPI_SPECIAL_GET_MODULE_PAR_VERSION_0                             = 0;
        public const uint DAPI_SPECIAL_GET_MODULE_PAR_VERSION_1                             = 1;
        public const uint DAPI_SPECIAL_GET_MODULE_PAR_VERSION_2                             = 2;
        public const uint DAPI_SPECIAL_GET_MODULE_PAR_VERSION_3                             = 3;
        //
        public const uint DAPI_SPECIAL_TIMEOUT_SET_VALUE_SEC                                = 1;
        public const uint DAPI_SPECIAL_TIMEOUT_ACTIVATE                                     = 2;
        public const uint DAPI_SPECIAL_TIMEOUT_DEACTIVATE                                   = 3;
        public const uint DAPI_SPECIAL_TIMEOUT_GET_STATUS                                   = 4;
        //
        public const uint DAPI_SPECIAL_DI_FF_FILTER_VALUE_SET                               = 1;
        public const uint DAPI_SPECIAL_DI_FF_FILTER_VALUE_GET                               = 2;
        //
        public const uint DAPI_SPECIAL_AD_READ_MULTIPLE_AD                                  = 1;
        public const uint DAPI_SPECIAL_AD_FIFO_ACTIVATE                                     = 2;
        public const uint DAPI_SPECIAL_AD_FIFO_DEACTIVATE                                   = 3;
        public const uint DAPI_SPECIAL_AD_FIFO_GET_STATUS                                   = 4;
        public const uint DAPI_SPECIAL_AD_FIFO_SET_INTERVAL_MS                              = 5;
        public const uint DAPI_SPECIAL_AD_FIFO_SET_CHANNEL                                  = 6;
        public const uint DAPI_SPECIAL_AD_FIFO_INIT                                         = 7;
        public const uint DAPI_SPECIAL_AD_FILTER_SET                                        = 8;
        //
        public const uint DAPI_SPECIAL_DA_PAR_DA_LOAD_DEFAULT                               = 1;
        public const uint DAPI_SPECIAL_DA_PAR_DA_SAVE_EEPROM_CONFIG                         = 2;
        public const uint DAPI_SPECIAL_DA_PAR_DA_LOAD_EEPROM_CONFIG                         = 3;
        public const uint DAPI_SPECIAL_DA_READBACK_MULIPLE_DA                               = 4;
        //
        public const uint DAPI_SPECIAL_WATCHDOG_SET_TIMEOUT_MSEC                            = 1;
        public const uint DAPI_SPECIAL_WATCHDOG_GET_TIMEOUT_MSEC                            = 2;
        public const uint DAPI_SPECIAL_WATCHDOG_GET_STATUS                                  = 3;
        public const uint DAPI_SPECIAL_WATCHDOG_GET_WD_COUNTER_MSEC                         = 4;
        public const uint DAPI_SPECIAL_WATCHDOG_GET_TIMEOUT_RELAIS_COUNTER_MSEC             = 5;
        public const uint DAPI_SPECIAL_WATCHDOG_SET_TIMEOUT_REL1_COUNTER_MSEC               = 6;
        public const uint DAPI_SPECIAL_WATCHDOG_SET_TIMEOUT_REL2_COUNTER_MSEC               = 7;
        //
        public const uint DAPI_SPECIAL_COUNTER_LATCH_ALL                                    = 1;
        public const uint DAPI_SPECIAL_COUNTER_LATCH_ALL_WITH_RESET                         = 2;
        //
        public const uint DAPI_SPECIAL_CNT48_RESET_SINGLE                                   = 1;
        public const uint DAPI_SPECIAL_CNT48_RESET_GROUP8                                   = 2;
        public const uint DAPI_SPECIAL_CNT48_LATCH_GROUP8                                   = 3;
        public const uint DAPI_SPECIAL_CNT48_DI_GET1                                        = 4;
        //
        public const uint DAPI_SPECIAL_SOFTWARE_FIFO_ACTIVATE                               = 1;
        public const uint DAPI_SPECIAL_SOFTWARE_FIFO_DEACTIVATE                             = 2;
        public const uint DAPI_SPECIAL_SOFTWARE_FIFO_GET_STATUS                             = 3;
        //
        public const uint DAPI_SPECIAL_TEMP_READ_MULIPLE_TEMP                               = 1;
        //
        public const uint DAPI_SPECIAL_PWM_READBACK_MULIPLE_PWM                             = 1;

        //
        // values for PAR2
        public const uint DAPI_SPECIAL_AD_CH0_CH15                                          = 0;
        public const uint DAPI_SPECIAL_AD_CH16_CH31                                         = 1;

        // ----------------------------------------------------------------------------
        // DapiScanModules-Codes
        public const uint DAPI_SCANMODULE_GET_MODULES_AVAILABLE                             = 1;

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // DI - Counter Mode

	    public const uint DAPI_CNT_MODE_READ_WITH_RESET							            = 0x01;
        public const uint DAPI_CNT_MODE_READ_LATCHED                                        = 0x02;

        // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // A/D and D/A Modes
    	
	    public const uint  ADDA_MODE_UNIPOL_10V 						                    = 0x00;
	    public const uint  ADDA_MODE_UNIPOL_5V 									            = 0x01;
	    public const uint  ADDA_MODE_UNIPOL_2V5 							                = 0x02;
    	
	    public const uint  ADDA_MODE_BIPOL_10V 									            = 0x40;
	    public const uint  ADDA_MODE_BIPOL_5V 									            = 0x41;
	    public const uint  ADDA_MODE_BIPOL_2V5 									            = 0x42;
    	
	    public const uint  ADDA_MODE_0_20mA 								                = 0x80;
	    public const uint  ADDA_MODE_4_20mA										            = 0x81;
	    public const uint  ADDA_MODE_0_24mA 									            = 0x82;

	    public const uint  ADDA_MODE_DA_DISABLE 							                = 0x100;
	    public const uint  ADDA_MODE_DA_ENABLE 									            = 0x200;
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // Stepper-Defines

	    // ------------------------------------
	    // ERROR Codes
	    public const uint  DAPI_STEPPER_ERR_NONE					                        = 0;		// es liegt kein Fehler vor 
	    public const uint  DAPI_STEPPER_ERR_PARAMETER					                    = 1;		// Parameter hat falschen Wertebereich 
	    public const uint  DAPI_STEPPER_ERR_MOTOR_MOVE							            = 2;		// Kommando abgelehnt, da sich der Motor dreht
	    public const uint  DAPI_STEPPER_ERR_DISABLE_MOVE					                = 3;		// Kommando abgehelnt, da Motorbewegung disabled ist
	    // public const uint  DAPI_STEPPER_ERR_DEVICE_NOT_FOUND				                = -1;		// es liegt kein Fehler vor 

	    // ------------------------------------
	    // Special Stepper Function-Codes
	    public const uint  DAPI_STEPPER_RETURN_0_BYTES 							            = 0x00000000;		// Kommando schickt 0 Byte als Antwort
	    public const uint  DAPI_STEPPER_RETURN_1_BYTES 							            = 0x40000000;		// Kommando schickt 1 Byte als Antwort
	    public const uint  DAPI_STEPPER_RETURN_2_BYTES 							            = 0x80000000;		// Kommando schickt 2 Byte als Antwort
	    public const uint  DAPI_STEPPER_RETURN_4_BYTES 							            = 0xc0000000;		// Kommando schickt 4 Byte als Antwort

	    public const uint  DAPI_STEPPER_CMD_SET_MOTORCHARACTERISTIC                         = ( 0x00000001 + DAPI_STEPPER_RETURN_0_BYTES );  
	    public const uint  DAPI_STEPPER_CMD_GET_MOTORCHARACTERISTIC                         = ( 0x00000002 + DAPI_STEPPER_RETURN_4_BYTES ); 
	    public const uint  DAPI_STEPPER_CMD_SET_POSITION                                    = ( 0x00000003 + DAPI_STEPPER_RETURN_0_BYTES );  
	    public const uint  DAPI_STEPPER_CMD_GO_POSITION                                     = ( 0x00000004 + DAPI_STEPPER_RETURN_0_BYTES );  
	    public const uint  DAPI_STEPPER_CMD_GET_POSITION                                    = ( 0x00000005 + DAPI_STEPPER_RETURN_4_BYTES );  
	    public const uint  DAPI_STEPPER_CMD_SET_FREQUENCY                                   = ( 0x00000006 + DAPI_STEPPER_RETURN_0_BYTES );  
	    public const uint  DAPI_STEPPER_CMD_SET_FREQUENCY_DIRECTLY                          = ( 0x00000007 + DAPI_STEPPER_RETURN_0_BYTES );  
	    public const uint  DAPI_STEPPER_CMD_GET_FREQUENCY                                   = ( 0x00000008 + DAPI_STEPPER_RETURN_2_BYTES );  
	    public const uint  DAPI_STEPPER_CMD_FULLSTOP                                        = ( 0x00000009 + DAPI_STEPPER_RETURN_0_BYTES );  
	    public const uint  DAPI_STEPPER_CMD_STOP                                            = ( 0x00000010 + DAPI_STEPPER_RETURN_0_BYTES );  
	    public const uint  DAPI_STEPPER_CMD_GO_REFSWITCH                                    = ( 0x00000011 + DAPI_STEPPER_RETURN_0_BYTES );  
	    public const uint  DAPI_STEPPER_CMD_DISABLE                                         = ( 0x00000014 + DAPI_STEPPER_RETURN_0_BYTES );  
	    public const uint  DAPI_STEPPER_CMD_MOTORCHARACTERISTIC_LOAD_DEFAULT                = ( 0x00000015 + DAPI_STEPPER_RETURN_0_BYTES );
	    public const uint  DAPI_STEPPER_CMD_MOTORCHARACTERISTIC_EEPROM_SAVE		            = ( 0x00000016 + DAPI_STEPPER_RETURN_0_BYTES );
	    public const uint  DAPI_STEPPER_CMD_MOTORCHARACTERISTIC_EEPROM_LOAD		            = ( 0x00000017 + DAPI_STEPPER_RETURN_0_BYTES );
	    public const uint  DAPI_STEPPER_CMD_GET_CPU_TEMP					                = ( 0x00000018 + DAPI_STEPPER_RETURN_1_BYTES );
	    public const uint  DAPI_STEPPER_CMD_GET_MOTOR_SUPPLY_VOLTAGE			            = ( 0x00000019 + DAPI_STEPPER_RETURN_2_BYTES );
	    public const uint  DAPI_STEPPER_CMD_GO_POSITION_RELATIVE                            = ( 0x00000020 + DAPI_STEPPER_RETURN_0_BYTES );  
	    public const uint  DAPI_STEPPER_CMD_EEPROM_ERASE						            = ( 0x00000021 + DAPI_STEPPER_RETURN_0_BYTES );  
	    public const uint  DAPI_STEPPER_CMD_SET_VECTORMODE                                  = ( 0x00000040 + DAPI_STEPPER_RETURN_0_BYTES );
	    //public const uint  DAPI_STEPPER_CMD_GET_STATUS                                    = ( 0x00000015 + DAPI_STEPPER_RETURN_1_BYTES );

	    // ------------------------------------
	    // values for PAR1 for DAPI_STEPPER_CMD_SET_MOTORCHARACTERISTIC

	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_STEPMODE					            = 1;					// Schrittmode (Voll-, Halb-, Viertel-, Achtel-, Sechszehntelschritt 
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_GOFREQUENCY				            = 2;					// Schrittfrequenz bei GoPosition [Vollschritt / s]
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_STARTFREQUENCY		                = 3;					// Startfrequenz [Vollschritt / s]
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_STOPFREQUENCY				            = 4;					// Stopfrequenz [Vollschritt / s]
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_MAXFREQUENCY				            = 5;					// maximale Frequenz [Vollschritt / s]
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_ACCELERATIONSLOPE			            = 6;					// Beschleunigung in [Vollschritten / ms]
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_DECELERATIONSLOPE			            = 7;					// Bremsung in [Vollschritten / ms]
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_PHASECURRENT				            = 8;					// Phasenstrom [mA]
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_HOLDPHASECURRENT			            = 9;					// Phasenstrom bei Motorstillstand [mA]
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_HOLDTIME 					            = 10;					// Zeit in der der Haltestrom fließt nach Motorstop [s]
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_STATUSLEDMODE				            = 11;					// Betriebsart der Status-LED
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_INVERT_ENDSW1				            = 12;					// invertiere Funktion des Endschalter1  
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_INVERT_ENDSW2				            = 13;					// invertiere Funktion des Endschalter12 
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_INVERT_REFSW1				            = 14;					// invertiere Funktion des Referenzschalterschalter1
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_INVERT_REFSW2				            = 15;					// invertiere Funktion des Referenzschalterschalter2
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_INVERT_DIRECTION 			            = 16;					// invertiere alle Richtungsangaben
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_ENDSWITCH_STOPMODE			        = 17;					// Bei Endschalter soll (0=full stop/1=stop mit rampe)
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_GOREFERENCEFREQUENCY_TOENDSWITCH	    = 18;			        // Motor Frequency for GoReferenceCommand
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_GOREFERENCEFREQUENCY_AFTERENDSWITCH	= 19;			        // Motor Frequency for GoReferenceCommand
	    public const uint  DAPI_STEPPER_MOTORCHAR_PAR_GOREFERENCEFREQUENCY_TOOFFSET 		= 20;			        // Motor Frequency for GoReferenceCommand

	    // ----------------------------------------------------------------------------
	    // values for PAR1 for DAPI_STEPPER_CMD_GO_REFSWITCH

	    public const uint  DAPI_STEPPER_GO_REFSWITCH_PAR_REF1						        = 1;
	    public const uint  DAPI_STEPPER_GO_REFSWITCH_PAR_REF2						        = 2;
	    public const uint  DAPI_STEPPER_GO_REFSWITCH_PAR_REF_LEFT					        = 4;
	    public const uint  DAPI_STEPPER_GO_REFSWITCH_PAR_REF_RIGHT				            = 8;
	    public const uint  DAPI_STEPPER_GO_REFSWITCH_PAR_REF_GO_POSITIVE			        = 16;
	    public const uint  DAPI_STEPPER_GO_REFSWITCH_PAR_REF_GO_NEGATIVE			        = 32;
	    public const uint  DAPI_STEPPER_GO_REFSWITCH_PAR_SET_POS_0				            = 64;

	    // ------------------------------------
	    // Stepper Read Status
    	
	    public const uint  DAPI_STEPPER_STATUS_GET_POSITION						            = 0x01;
	    public const uint  DAPI_STEPPER_STATUS_GET_SWITCH							        = 0x02;
	    public const uint  DAPI_STEPPER_STATUS_GET_ACTIVITY						            = 0x03;
    	
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // CNT48 Commands

	    public const uint  DAPI_CNT48_FILTER_20ns									        = 0x0000;
	    public const uint  DAPI_CNT48_FILTER_100ns								            = 0x1000;
	    public const uint  DAPI_CNT48_FILTER_250ns								            = 0x2000;
	    public const uint  DAPI_CNT48_FILTER_500ns								            = 0x3000;
	    public const uint  DAPI_CNT48_FILTER_1us									        = 0x4000;
	    public const uint  DAPI_CNT48_FILTER_2_5us								            = 0x5000;
	    public const uint  DAPI_CNT48_FILTER_5us									        = 0x6000;
	    public const uint  DAPI_CNT48_FILTER_10us									        = 0x7000;
	    public const uint  DAPI_CNT48_FILTER_25us									        = 0x8000;
	    public const uint  DAPI_CNT48_FILTER_50us									        = 0x9000;
	    public const uint  DAPI_CNT48_FILTER_100us								            = 0xA000;
	    public const uint  DAPI_CNT48_FILTER_250us								            = 0xB000;
	    public const uint  DAPI_CNT48_FILTER_500us								            = 0xC000;
	    public const uint  DAPI_CNT48_FILTER_1ms									        = 0xD000;
	    public const uint  DAPI_CNT48_FILTER_2_5ms								            = 0xE000;
	    public const uint  DAPI_CNT48_FILTER_5ms									        = 0xF000;

	    public const uint  DAPI_CNT48_MODE_COUNT_RISING_EDGE						        = 0x0000;
	    public const uint  DAPI_CNT48_MODE_T										        = 0x000D;
	    public const uint  DAPI_CNT48_MODE_FREQUENCY								        = 0x000E;
	    public const uint  DAPI_CNT48_MODE_PWM									            = 0x000F;

	    public const uint  DAPI_CNT48_SUBMODE_NO_RESET							            = 0x0000;
	    public const uint  DAPI_CNT48_SUBMODE_RESET_WITH_READ 					            = 0x0010;
	    public const uint  DAPI_CNT48_SUBMODE_RESET_ON_CH_7 						        = 0x0070;
	    public const uint  DAPI_CNT48_SUBMODE_LATCH_COMMON 						            = 0x0080;

		public const uint DAPI_OPEN_MODULE_OPTION_USE_EXBUFFER			= (1<<0);	// Bit 0
		public const uint DAPI_OPEN_MODULE_OPTION_NO_RESCAN				= (1<<1);	// Bit 1

		public const uint DAPI_OPEN_MODULE_ENCRYPTION_TYPE_NONE			= 0;
		public const uint DAPI_OPEN_MODULE_ENCRYPTION_TYPE_NORMAL		= 1;
		public const uint DAPI_OPEN_MODULE_ENCRYPTION_TYPE_ADMIN		= 2;

        // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // DELIB-Functions
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
	    // ----------------------------------------------------------------------------
    

        public static void DapiDOSet1_WithTimer(uint handle, uint ch, uint data, uint time_ms)
        {
            DapiWriteLong(handle, 0x200, time_ms);
            DapiWriteWord(handle, 0x204, (ch << 8) | data);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiDOSet1(uint handle, uint ch, uint data)
        {   
            if (data != 0)
            {
                DapiDOSetBit32(handle, (uint)(ch & (~31)), (uint)(1 << (int)(ch & 0x1f)));
            }
            else
            {
                DapiDOClrBit32(handle, (uint)(ch & (~31)), (uint)(1 << (int)(ch & 0x1f)));
            }
        }

        public static void DapiDOClrBit32(uint handle, uint ch, uint data)
        {
            uint addr = 0xa0 | ((ch>>3) & 0x1f);
            DapiWriteLong(handle, addr, data);            
        }

        public static void DapiDOSetBit32(uint handle, uint ch, uint data)
        {   
            uint addr = 0x80 | ((ch>>3) & 0x1f);
            DapiWriteLong(handle, addr, data);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiDOSet8(uint handle, uint ch, uint data)
        {
            
            DapiWriteByte(handle, (ch >> 3) & 0x1f, data);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiDOSet16(uint handle, uint ch, uint data)
        {
            DapiWriteWord(handle, (uint) ((ch >> 3) & (~1) & 0x1f), data);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiDOSet32(uint handle, uint ch, uint data)
        {
            DapiWriteLong(handle, (uint)((ch >> 3) & (~3) & 0x1f), data);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiDOSet64(uint handle, uint ch, ulong data)
		{
			DapiWriteLongLong(handle, (uint)((ch >> 3) & (~3) & 0x1f), data);
		}


        /*


DAPI_FUNCTION_PRE64 void DAPI_FUNCTION_PRE DapiDOSetBit32(ULONG handle, ULONG ch, ULONG data)
{
	DAPI_HANDLE *DapiHandle;
	DAPI_MODULE_PARAMS *DapiModuleParams;
	char debug_msg[200];
	ULONG addr;
	
	addr = 0x80 | ((ch>>3) & 0x1f);
	
	DapiHandle = (DAPI_HANDLE*) handle;
	DapiModuleParams = (DAPI_MODULE_PARAMS *) DapiHandle->addr_dapi_module_params;

	// DEBUG
	#ifdef DEBUG_OUTPUT
		debug_set_function_name("DapiDOSetBit32");
		sprintf(debug_msg, "DapiDOSetBit32(handle=0x%lx, ch=%ld, data=0x%lx)", handle, ch, data);
		debug_print(debug_msg);
	#endif
	// -----
	
	// module does not support new Software-Features OR
	// module does not have the accessed I/O-type OR 
	// command is not supported ?
	if ( ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_SUPPORTED_BY_FIRMWARE) == 0) || \
		 ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_CFG_DO) == 0) || \
		 ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_CFG_DO_CMD_SET_CLR_BIT_32) == 0) )
	{
		sprintf(debug_msg, "DapiDOSetBit32 -> Module does not support this command");
		DapiSetError(DAPI_ERR_ILLEGAL_IO_TYPE, 0, debug_msg, __FILE__, __LINE__);
		return;
	}
	
	// illegal channel ?
	if (DapiModuleParams->anz_do <= ch)
	{
		sprintf(debug_msg, "DapiDOSetBit32 -> Error accessing channel %ld - Module only supports %ld channels", ch, DapiModuleParams->anz_do);
		DapiSetError(DAPI_ERR_ILLEGAL_CHANNEL, 0, debug_msg, __FILE__, __LINE__);
		return;
	}
	
	// illegal channel parameter ? 
	if ((ch & 0x1f) != 0)	
	{
		sprintf(debug_msg, "DapiDOSetBit32 -> Illegal channel parameter for this command (Valid: 0, 32, 64, ..)");
		DapiSetError(DAPI_ERR_ILLEGAL_CHANNEL, 0, debug_msg, __FILE__, __LINE__);
		return;
	}

	DapiWriteLong(handle, addr, data);
}

// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------

         * 

DAPI_FUNCTION_PRE64 void DAPI_FUNCTION_PRE DapiDOSetBit32(ULONG handle, ULONG ch, ULONG data)
{
	DAPI_HANDLE *DapiHandle;
	DAPI_MODULE_PARAMS *DapiModuleParams;
	char debug_msg[200];
	ULONG addr;
	
	addr = 0x80 | ((ch>>3) & 0x1f);
	
	DapiHandle = (DAPI_HANDLE*) handle;
	DapiModuleParams = (DAPI_MODULE_PARAMS *) DapiHandle->addr_dapi_module_params;

	// DEBUG
	#ifdef DEBUG_OUTPUT
		debug_set_function_name("DapiDOSetBit32");
		sprintf(debug_msg, "DapiDOSetBit32(handle=0x%lx, ch=%ld, data=0x%lx)", handle, ch, data);
		debug_print(debug_msg);
	#endif
	// -----
	
	// module does not support new Software-Features OR
	// module does not have the accessed I/O-type OR 
	// command is not supported ?
	if ( ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_SUPPORTED_BY_FIRMWARE) == 0) || \
		 ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_CFG_DO) == 0) || \
		 ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_CFG_DO_CMD_SET_CLR_BIT_32) == 0) )
	{
		sprintf(debug_msg, "DapiDOSetBit32 -> Module does not support this command");
		DapiSetError(DAPI_ERR_ILLEGAL_IO_TYPE, 0, debug_msg, __FILE__, __LINE__);
		return;
	}
	
	// illegal channel ?
	if (DapiModuleParams->anz_do <= ch)
	{
		sprintf(debug_msg, "DapiDOSetBit32 -> Error accessing channel %ld - Module only supports %ld channels", ch, DapiModuleParams->anz_do);
		DapiSetError(DAPI_ERR_ILLEGAL_CHANNEL, 0, debug_msg, __FILE__, __LINE__);
		return;
	}
	
	// illegal channel parameter ? 
	if ((ch & 0x1f) != 0)	
	{
		sprintf(debug_msg, "DapiDOSetBit32 -> Illegal channel parameter for this command (Valid: 0, 32, 64, ..)");
		DapiSetError(DAPI_ERR_ILLEGAL_CHANNEL, 0, debug_msg, __FILE__, __LINE__);
		return;
	}

	DapiWriteLong(handle, addr, data);
}
         * 
DAPI_FUNCTION_PRE64 void DAPI_FUNCTION_PRE DapiDOClrBit32(ULONG handle, ULONG ch, ULONG data)
{
	DAPI_HANDLE *DapiHandle;
	DAPI_MODULE_PARAMS *DapiModuleParams;
	char debug_msg[200];
	ULONG addr;
	
	addr = 0xa0 | ((ch>>3) & 0x1f);
	
	DapiHandle = (DAPI_HANDLE*) handle;
	DapiModuleParams = (DAPI_MODULE_PARAMS *) DapiHandle->addr_dapi_module_params;

	// DEBUG
	#ifdef DEBUG_OUTPUT
		debug_set_function_name("DapiDOClrBit32");
		sprintf(debug_msg, "DapiDOClrBit32(handle=0x%lx, ch=%ld, data=0x%lx)", handle, ch, data);
		debug_print(debug_msg);
	#endif
	// -----
	
	// module does not support new Software-Features OR
	// module does not have the accessed I/O-type OR 
	// command is not supported ?
	if ( ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_SUPPORTED_BY_FIRMWARE) == 0) || \
		 ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_CFG_DO) == 0) || \
		 ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_CFG_DO_CMD_SET_CLR_BIT_32) == 0) )
	{
		sprintf(debug_msg, "DapiDOClrBit32 -> Module does not support this command");
		DapiSetError(DAPI_ERR_ILLEGAL_IO_TYPE, 0, debug_msg, __FILE__, __LINE__);
		return;
	}
	
	// illegal channel ?
	if (DapiModuleParams->anz_do <= ch)
	{
		sprintf(debug_msg, "DapiDOClrBit32 -> Error accessing channel %ld - Module only supports %ld channels", ch, DapiModuleParams->anz_do);
		DapiSetError(DAPI_ERR_ILLEGAL_CHANNEL, 0, debug_msg, __FILE__, __LINE__);
		return;
	}
	
	// illegal channel parameter ? 
	if ((ch & 0x1f) != 0)	
	{
		sprintf(debug_msg, "DapiDOClrBit32 -> Illegal channel parameter for this command (Valid: 0, 32, 64, ..)");
		DapiSetError(DAPI_ERR_ILLEGAL_CHANNEL, 0, debug_msg, __FILE__, __LINE__);
		return;
	}

	DapiWriteLong(handle, addr, data);
}

        */
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static uint DapiDIGet1(uint handle, uint ch)
        {
            uint xx;
            int mask;

            xx = DapiDIGet8(handle, ch & 0xfff8);

            mask = 1 << ((int)ch & 7);
            if ((xx & mask) == 0)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static uint DapiDIGet8(uint handle, uint ch)
        {
            uint ret;

            ret = DapiReadByte(handle, 0x20 | ((ch >> 3) & 0x1f));
            return ret;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static uint DapiDIGet16(uint handle, uint ch)
        {
            uint ret;

            ret = DapiReadWord(handle, 0x20 | ((ch >> 3) & 0x1f));
            return ret;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static uint DapiDIGet32(uint handle, uint ch)
        {
            uint ret;

            ret = DapiReadLong(handle, 0x20 | ((ch >> 3) & 0x1f));
            return ret;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static ulong DapiDIGet64(uint handle, uint ch)
		{

			ulong ret;

			ret = DapiReadLongLong(handle, 0x20 | ((ch >> 3) & 0x1f));
			return ret;

		}

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static uint DapiDIGetFF32(uint handle, uint ch)
        {
            uint ret;

            ret = DapiReadLong(handle, 0x40 | ((ch >> 3) & 0x1f));
            return ret;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static uint DapiDIGetCounter(uint handle, uint ch, uint mode)
        {
            uint adr;
            uint ret;

            adr = 0x100 + (ch & 0x1f) * 2;
            if (mode == DAPI_CNT_MODE_READ_WITH_RESET)
            {
                adr += 0x100;
            }
            else if (mode == DAPI_CNT_MODE_READ_LATCHED)
            {
                adr += 0x200;
            }

            ret = DapiReadWord(handle, adr);
            return ret;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static uint DapiDOReadback32(uint handle, uint ch)
        {
            uint ret;

            ret = DapiReadLong(handle, (ch >> 3) & 0x1f);
            return ret;
        }

		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------

		public static ulong DapiDOReadback64(uint handle, uint ch)
		{
			ulong ret;

			ret = DapiReadLongLong(handle, (ch >> 3) & 0x1f);
			return ret;
		}

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiADSetMode(uint handle, uint ch, uint mode)
        {
            DapiWriteByte(handle, 0x1000 + ch * 4 + 3, mode);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static uint DapiADGetMode(uint handle, uint ch)
        {
            return DapiReadByte(handle, 0x1000 + ch * 4 + 3);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static uint DapiADGet(uint handle, uint ch)
        {
            uint ret;

            if ((ch & 0x8000) == 0)
            {
                return DapiReadWord(handle, 0x1000 + ch * 4);
            }
            else
            {
                ret = ((uint)global_ad_buffer[(int)((ch & 63) * 4) + 0]);
                ret |= ((uint)global_ad_buffer[(int)((ch & 63) * 4) + 1]) << 8;
                return ret;
            }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static float DapiADGetVolt(uint handle, uint ch)
        {
            uint data;
            uint mode;
            float value = 0;

            if ((ch & 0x8000) == 0)
            {
                data = DapiReadLong(handle, 0x1000 + ch * 4);
            }
            else
            {
                data = ((uint)global_ad_buffer[(int)((ch & 63) * 4) + 0]);
                data |= ((uint)global_ad_buffer[(int)((ch & 63) * 4) + 1]) << 8;
                data |= ((uint)global_ad_buffer[(int)((ch & 63) * 4) + 2]) << 16;
                data |= ((uint)global_ad_buffer[(int)((ch & 63) * 4) + 3]) << 24;
            }

            mode = (data >> 24) & 0xff;

            switch ((int)mode)
            {
                case (int)ADDA_MODE_UNIPOL_10V:
                    // 0-10V
                    value = (((float)(data & 0xffff)) * 10.0F / 65536.0F);
                    break;

                case (int)ADDA_MODE_UNIPOL_5V:
                    // 0-5V
                    value = (((float)(data & 0xffff)) * 5.0F / 65536.0F);
                    break;

                case (int)ADDA_MODE_BIPOL_10V:
                    // +-10V
                    value = (((float)(data & 0xffff)) * 20.0F / 65536.0F) - 10.0F;
                    break;

                case (int)ADDA_MODE_BIPOL_5V:
                    // +-5V
                    value = (((float)(data & 0xffff)) * 10.0F / 65536.0F) - 5.0F;
                    break;

                default:
                    break;
            }
            return value;
        }




        //				printf("A/D Ch %d = 0x%4x  Wert=%f modus=%x\n", i, data_ad, (((float) data_ad) *20.0 / 65536) - 10, modus_ad);

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static float DapiADGetmA(uint handle, uint ch)
        {
            uint data;
            uint mode;
            float value = 0;

            if ((ch & 0x8000) == 0)
            {
                data = DapiReadLong(handle, 0x1000 + ch * 4);
            }
            else
            {
                data = global_ad_buffer[(int)ch & 63];
            }


            mode = data >> 24;

            switch ((int)mode)
            {
                /*
                    case (int) ADDA_MODE_0_50mA:
                        // 0-5V entspricht 0-50mA (100 Ohm)
                        value = (((float) (data&0xffff)) *50.0F / 65536.0F);
                        break;
                */
                case (int)ADDA_MODE_0_24mA:
                case (int)ADDA_MODE_0_20mA:
                case (int)ADDA_MODE_4_20mA:
                    // 0-5V entspricht 0-25mA (100 Ohm) und Spannungsverdopplung !
                    value = (((float)(data & 0xffff)) * 25.0F / 65536.0F);
                    break;

                default:
                    break;

            }


            return value;
        }

        public static void DapiDASetMode(uint handle, uint ch, uint mode)
        {
            DapiWriteByte(handle, 0x2000 + ch * 8 + 2, mode & 255);

            if ((mode & ADDA_MODE_DA_DISABLE) == ADDA_MODE_DA_DISABLE)
            {
                DapiWriteByte(handle, 0x2000 + ch * 8 + 3, 1);	// Disable D/A Channel
            }
            if ((mode & ADDA_MODE_DA_ENABLE) == ADDA_MODE_DA_ENABLE)
            {
                DapiWriteByte(handle, 0x2000 + ch * 8 + 3, 0);	// Enable D/A Channel
            }

        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static uint DapiDAGetMode(uint handle, uint ch)
        {
            return DapiReadByte(handle, 0x2000 + ch * 8 + 2);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiDASet(uint handle, uint ch, uint data)
        {
            DapiWriteWord(handle, 0x2000 + ch * 8, data);

        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static float DapiDAGetVolt(uint handle, uint ch)
        {
            uint data = 0;
            uint mode;
            float value = 0;

            if ((ch & 0x8000) == 0)
            {
                //data = DapiReadLong(handle, 0x1000 + ch * 4);
            }
            else
            {
                data =  ((uint)global_da_buffer[(int)((ch & 127) * 8) + 0]);
                data |= ((uint)global_da_buffer[(int)((ch & 127) * 8) + 1]) << 8;
                data |= ((uint)global_da_buffer[(int)((ch & 127) * 8) + 2]) << 16;
                data |= ((uint)global_da_buffer[(int)((ch & 127) * 8) + 3]) << 24;
            }

            mode = (data >> 24) & 0xff;

            switch ((int)mode)
            {
                case (int)ADDA_MODE_UNIPOL_10V:
                    // 0-10V
                    value = (((float)(data & 0xffff)) * 10.0F / 65536.0F);
                    break;

                case (int)ADDA_MODE_UNIPOL_5V:
                    // 0-5V
                    value = (((float)(data & 0xffff)) * 5.0F / 65536.0F);
                    break;

                case (int)ADDA_MODE_BIPOL_10V:
                    // +-10V
                    value = (((float)(data & 0xffff)) * 20.0F / 65536.0F) - 10.0F;
                    break;

                case (int)ADDA_MODE_BIPOL_5V:
                    // +-5V
                    value = (((float)(data & 0xffff)) * 10.0F / 65536.0F) - 5.0F;
                    break;

                default:
                    break;
            }

            return value;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiDASetVolt(uint handle, uint ch, float volt)
        {
            uint mode;
            uint value;
            mode = DapiDAGetMode(handle, ch);

            switch ((int)mode)
            {
                case (int)ADDA_MODE_UNIPOL_10V:
                    value = (uint)(3276.8 * 2.0 * volt);
                    if (value > 0xffff) value = 0xffff;
                    DapiDASet(handle, ch, value);
                    break;

                case (int)ADDA_MODE_UNIPOL_5V:
                    value = (uint)(3276.8 * 4.0 * volt);
                    if (value > 0xffff) value = 0xffff;
                    DapiDASet(handle, ch, value);
                    break;

                case (int)ADDA_MODE_UNIPOL_2V5:
                    value = (uint)(3276.8 * 8.0 * volt);
                    if (value > 0xffff) value = 0xffff;
                    DapiDASet(handle, ch, value);
                    break;

                case (int)ADDA_MODE_BIPOL_10V:
                    value = (uint)(32768.0 + 3276.8 * volt);
                    if (value > 0xffff) value = 0xffff;
                    DapiDASet(handle, ch, value);
                    break;

                case (int)ADDA_MODE_BIPOL_5V:
                    value = (uint)(32768.0 + 3276.8 * 2.0 * volt);
                    if (value > 0xffff) value = 0xffff;
                    DapiDASet(handle, ch, value);
                    break;

                case (int)ADDA_MODE_BIPOL_2V5:
                    value = (uint)(32768.0 + 3276.8 * 4.0 * volt);
                    if (value > 0xffff) value = 0xffff;
                    DapiDASet(handle, ch, value);
                    break;

                default:
                    break;
            }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiDASetmA(uint handle, uint ch, float data)
        {
            uint mode;
            uint value;

            mode = DapiDAGetMode(handle, ch);

            switch ((int)mode)
            {
                case (int)ADDA_MODE_0_20mA:
                    if (data < 0) data = 0;
                    value = (uint)(3276.8 * data);
                    if (value > 0xffff) value = 0xffff;
                    DapiDASet(handle, ch, value);
                    break;

                case (int)ADDA_MODE_4_20mA:
                    if (data < 4) data = 4;
                    value = (uint)(4096 * (data - 4.0));
                    if (value > 0xffff) value = 0xffff;
                    DapiDASet(handle, ch, value);
                    break;

                case (int)ADDA_MODE_0_24mA:
                    if (data < 0) data = 0;
                    value = (uint)(65536 / 24 * data);
                    if (value > 0xffff) value = 0xffff;
                    DapiDASet(handle, ch, value);
                    break;

                default:
                    break;
            }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static float DapiTempGet(uint handle, uint ch)
        {
            float f;
            uint d;

            //DT.DBG.

            if ((ch & 0x8000) == 0)
            {
                if ((ch & 0x4000) == 0)
                {
                    // Temperatur soll gelesen werden
                    d = DapiReadLong(handle, 0x4000 + ch * 8);    
                }
                else
                {
                    // Temperatur soll aus buffer gelesen werden
                    d =  ((uint)global_temp_buffer[(int)((ch & 127) * 8) + 0]);
                    d |= ((uint)global_temp_buffer[(int)((ch & 127) * 8) + 1]) << 8;
                    d |= ((uint)global_temp_buffer[(int)((ch & 127) * 8) + 2]) << 16;
                    d |= ((uint)global_temp_buffer[(int)((ch & 127) * 8) + 3]) << 24;                    
                }


                //sprintf(msg,"|DELIB|--------------------DapiTempGet ch=%d ret=0x%lx", (unsigned int) ch, d);
                //debug_print(msg);


                switch ((int)(d >> 16) & 0xff)
                {
                    case 1: f = ((float)(d & 0x7fff)) / 10; break;              // Faktor 10
                    case 2: f = ((float)(d & 0x7fff)) / 100; break;             // Faktor 100
                    case 0: f = -9999; break;                                   // Faktor Sensor disconnected
                    default: f = 0; break;

                        //sprintf(msg,"|DELIB|--------------------DapiTempGet 222222 f=%f", f);
                        //debug_print(msg);
                }

                if (((d >> 15) & 1) != 0) f = -f;           // Negative Temp

                //sprintf(msg,"|DELIB|--------------------DapiTempGet 3333333 f=%f", f);
                //debug_print(msg);
            }
            else
            {
                // Der Widerstandswert soll gelesen werden
                d = DapiReadWord(handle, 0x4000 + ch * 8 + 6);				// Widerstandswert lesen

                f = ((float)(d & 0x7fff)) / 100;								// Faktor 100
                if (((d >> 15) & 1) != 0) f = -f;									// Negative
            }

            return f;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiCnt48ModeSet(uint handle, uint ch, uint mode)
        {
            DapiWriteWord(handle, 0x5000 + ch * 8 + 6, mode);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static uint DapiCnt48ModeGet(uint handle, uint ch)
        {
            return DapiReadWord(handle, 0x5000 + ch * 8 + 6);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static uint DapiCnt48CounterGet32(uint handle, uint ch)
        {
            return DapiReadLong(handle, 0x5000 + ch * 8);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        /*
        DapiCnt48CounterGet48(uint handle, uint ch)
        {
            return DapiReadLongLong(handle, 0x5000 + ch*8);
        }
        */

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiPulseGenSet(uint handle, uint ch, uint mode, uint par0, uint par1, uint par2)
        {
            //	uint ret;
            //	char msg[200];

            /*
            sprintf(msg,"|DELIB|--------------------DapiPulseGenSet Reg A = %x", (unsigned int) mode);
            debug_print(msg);
	
            sprintf(msg,"|DELIB|--------------------DapiPulseGenSet Reg B = %x", (unsigned int) par0);
            debug_print(msg);
	
            sprintf(msg,"|DELIB|--------------------DapiPulseGenSet Reg C = %x", (unsigned int) par1);
            debug_print(msg);
	
            sprintf(msg,"|DELIB|--------------------DapiPulseGenSet Reg D = %x", (unsigned int) par2);
            debug_print(msg);
            */

            DapiWriteLong(handle, 0x5800 + ch * 16 + 0, mode);
            DapiWriteLong(handle, 0x5800 + ch * 16 + 4, par0);
            DapiWriteLong(handle, 0x5800 + ch * 16 + 8, par1);
            DapiWriteLong(handle, 0x5800 + ch * 16 + 12, par2);

            /*
            ret = DapiReadLong(handle, 0x5800 + ch*16 + 0);
            sprintf(msg,"|DELIB|--------------------DapiPulseGenSet Reg A (readback) = %x", (unsigned int) ret);
            debug_print(msg);
	
            ret = DapiReadLong(handle, 0x5800 + ch*16 + 4);
            sprintf(msg,"|DELIB|--------------------DapiPulseGenSet Reg B (readback) = %x", (unsigned int) ret);
            debug_print(msg);
	
            ret = DapiReadLong(handle, 0x5800 + ch*16 + 8);
            sprintf(msg,"|DELIB|--------------------DapiPulseGenSet Reg C (readback) = %x", (unsigned int) ret);
            debug_print(msg);
	
            ret = DapiReadLong(handle, 0x5800 + ch*16 + 12);
            sprintf(msg,"|DELIB|--------------------DapiPulseGenSet Reg D (readback) = %x", (unsigned int) ret);
            debug_print(msg);
            */


            /*	printf("Mode  readback = %x\n", DapiReadLong(handle, 0x5800 + chan*16 + 0));IsError();
                printf("ModeA readback = %x\n", DapiReadLong(handle, 0x5800 + chan*16 + 4));IsError();
                printf("ModeB readback = %x\n", DapiReadLong(handle, 0x5800 + chan*16 + 8));IsError();
                printf("ModeC readback = %x\n", DapiReadLong(handle, 0x5800 + chan*16 + 12));IsError();
            */
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiPWMOutSet(uint handle, uint ch, float data)
        {
            DapiWriteByte(handle, 0x0800 + ch, (uint)data);
        }

        public static int DapiPWMOutReadback(uint handle, uint ch)
        {
            if ((ch & 0x8000) == 0)
            {
                return (int)DapiReadByte(handle, 0x0800 + (ch & 127));
            }
            else
            {
                return global_pwm_buffer[ch & 127];
            }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static uint DapiSpecialCommand(uint handle, uint cmd, uint par1, uint par2, uint par3)
        {
            uint ret = 0;
			//uint i;
			//uint bank;
			//byte[] buff;
			//uint pos;
			//uint result;
            uint cnt;

            switch ((int)cmd)
            {

                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------

                case (int)DAPI_SPECIAL_CMD_DA:
                    switch ((int)par1)
                    {
                        // ----------------------------------------
                        case (int)DAPI_SPECIAL_DA_READBACK_MULIPLE_DA:
                            ret = DapiReadMultipleBytes(handle, 0x2000 + par2*8, (par3-par2+1)*8, 1, global_da_buffer, (par3-par2)*8);
                            break;
                        // ----------------------------------------
                        case (int)DAPI_SPECIAL_DA_PAR_DA_LOAD_DEFAULT:
                            DapiWriteByte(handle, 0x2000 + par2 * 8 + 7, 0x12);		// Auslieferungszustand laden

                            cnt = 100;
                            do
                            {
                                --cnt;
                            } while ((DapiReadByte(handle, 0x2000 + par2 * 8 + 7) != 0xff) && (cnt != 0));

                            break;
                        // ----------------------------------------
                        case (int)DAPI_SPECIAL_DA_PAR_DA_SAVE_EEPROM_CONFIG:
                            DapiWriteByte(handle, 0x2000 + par2 * 8 + 7, 0x10);		// Ins EEPROM schreiben

                            cnt = 100;
                            do
                            {
                                --cnt;
                            } while ((DapiReadByte(handle, 0x2000 + par2 * 8 + 7) != 0xff) && (cnt != 0));

                            break;
                        // ----------------------------------------
                        case (int)DAPI_SPECIAL_DA_PAR_DA_LOAD_EEPROM_CONFIG:
                            DapiWriteByte(handle, 0x2000 + par2 * 8 + 7, 0x11);		// Aus EEPROM laden

                            cnt = 100;
                            do
                            {
                                --cnt;
                            } while ((DapiReadByte(handle, 0x2000 + par2 * 8 + 7) != 0xff) && (cnt != 0));

                            break;
                        // ----------------------------------------




                        //DapiWriteByte(handle, 0x2000 + i*8 + 7, 0x10);		// Diesen Zustand ins EEPROM
                    }
                    break;

                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------

                case (int) DAPI_SPECIAL_CMD_AD:
                    switch((int) par1)
                    {
                        // ----------------------------------------
                        case (int) DAPI_SPECIAL_AD_READ_MULTIPLE_AD:


					/*

						if(par2>128) return DT.RETURN_ERROR;
						if(par3>128) return DT.RETURN_ERROR;
						if(par2>par3) return DT.RETURN_ERROR;
*/

						ret = DapiReadMultipleBytes(handle, 0x1000 + par2*4, (par3-par2+1)*4, 1, global_ad_buffer, (par3-par2)*4);
						
                        break;
                        // ----------------------------------------
					/*
                        case (int) DAPI_SPECIAL_AD_FIFO_DEACTIVATE:
                            DapiWriteWord(handle, 0x1400 + par2*4, RO_FIFO_CMD_fifo_enable_disable | (0<<8));
                            break;
                        // ----------------------------------------
                        case (int) DAPI_SPECIAL_AD_FIFO_ACTIVATE:
                            DapiWriteWord(handle, 0x1400 + par2*4, RO_FIFO_CMD_fifo_enable_disable | (1<<8));
                            break;
                        // ----------------------------------------
                        case (int) DAPI_SPECIAL_AD_FIFO_INIT:
                            DapiWriteWord(handle, 0x1400 + par2*4, RO_FIFO_CMD_fifo_init | (0<<8));
                            break;
                        // ----------------------------------------
                        case (int) DAPI_SPECIAL_AD_FIFO_GET_STATUS:
                            //DapiWriteByte(handle, 0xfe02, RO_CPU_CMD_FIFO_GET_STATUS);
                            //ret = DapiReadByte(handle, 0xfe05);
                            break;
                        // ----------------------------------------
                        case (int) DAPI_SPECIAL_AD_FIFO_SET_INTERVAL_MS:
                            DapiWriteWord(handle, 0x1400 + par2*4, RO_FIFO_CMD_fifo_set_intevalL | ((par3&255)           <<8));
                            DapiWriteWord(handle, 0x1400 + par2*4, RO_FIFO_CMD_fifo_set_intevalH | ((par3>>8) & 255)     <<8);
                            break;
                        // ----------------------------------------
                        case (int) DAPI_SPECIAL_AD_FIFO_SET_CHANNEL:
                            DapiWriteWord(handle, 0x1400 + par2*4, RO_FIFO_CMD_fifo_enable_chL | ((par3&255)          <<8));
                            DapiWriteWord(handle, 0x1400 + par2*4, RO_FIFO_CMD_fifo_enable_chH | ((par3>>8) & 255)    <<8);
                            break;
                        // ----------------------------------------
                        */
                    }
                    break;
        
                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------
		
                case (int)DAPI_SPECIAL_CMD_SET_DIR_DX_8:
                    DapiWriteLong(handle, 0x100 + par1 / 64, par2);
                    break;

                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------

                case (int)DAPI_SPECIAL_CMD_SET_DIR_DX_1:
                    DapiWriteLong(handle, 0x120 + par1 / 8, par2);
                    break;

                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------

                case (int)DAPI_SPECIAL_CMD_GET_MODULE_CONFIG:
                    switch ((int)par1)
                    {
                        case (int)DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_DO:
                            ret = DapiReadByte(handle, 0xff00);
                            break;

                        case (int)DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_DI:
                            ret = DapiReadByte(handle, 0xff02);
                            break;

                        case (int)DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_DI_FF:
                            ret = DapiReadByte(handle, 0xff0c);
                            break;

                        case (int)DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_DI_COUNTER:
                            ret = DapiReadByte(handle, 0xff0e);
                            break;

                        case (int)DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_DX:
                            ret = DapiReadByte(handle, 0xff04);
                            break;

                        case (int)DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_AD:
                            ret = DapiReadByte(handle, 0xff08);
                            break;

                        case (int)DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_DA:
                            ret = DapiReadByte(handle, 0xff06);
                            break;

                        case (int)DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_STEPPER:
                            ret = DapiReadByte(handle, 0xff0a);
                            break;

                        case (int)DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_TEMP:
                            ret = DapiReadByte(handle, 0xff10);
                            break;

                        case (int)DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_CNT48:
                            ret = DapiReadByte(handle, 0xff12);
                            break;

                        case (int)DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_PULSE_GEN:
                            ret = DapiReadByte(handle, 0xff14);
                            break;

                        case (int)DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_PWM_OUT:
                            ret = DapiReadByte(handle, 0xff16);
                            break;

                    }
                    break;

                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------

                case (int)DAPI_SPECIAL_CMD_GET_MODULE_VERSION:
                    switch ((int)par1)
                    {
                        case (int)DAPI_SPECIAL_GET_MODULE_PAR_VERSION_0:
                            ret = DapiReadByte(handle, 0xfff4) - '0';
                            break;

                        case (int)DAPI_SPECIAL_GET_MODULE_PAR_VERSION_1:
                            ret = DapiReadByte(handle, 0xfff5) - '0';
                            break;

                        case (int)DAPI_SPECIAL_GET_MODULE_PAR_VERSION_2:
                            ret = DapiReadByte(handle, 0xfff6) - '0';
                            break;

                        case (int)DAPI_SPECIAL_GET_MODULE_PAR_VERSION_3:
                            ret = DapiReadByte(handle, 0xfff7) - '0';
                            break;
                    }
                    break;

                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------

                case (int)DAPI_SPECIAL_CMD_TIMEOUT:
                    switch ((int)par1)
                    {
                        case (int)DAPI_SPECIAL_TIMEOUT_SET_VALUE_SEC:
                            DapiWriteWord(handle, 0xfd02, par2 * 10 + (par3 % 10));
                            break;

                        case (int)DAPI_SPECIAL_TIMEOUT_ACTIVATE:
                            DapiWriteByte(handle, 0xfd00, 1);
                            break;

                        case (int)DAPI_SPECIAL_TIMEOUT_DEACTIVATE:
                            DapiWriteByte(handle, 0xfd00, 0);
                            break;

                        case (int)DAPI_SPECIAL_TIMEOUT_GET_STATUS:
                            ret = DapiReadByte(handle, 0xfd01);
                            break;
                    }
                    break;

                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------

                case (int)DAPI_SPECIAL_CMD_DI:
                    switch ((int)par1)
                    {
                        case (int)DAPI_SPECIAL_DI_FF_FILTER_VALUE_SET:
                            DapiWriteByte(handle, 0xfd10, par2);
                            break;
                        case (int)DAPI_SPECIAL_DI_FF_FILTER_VALUE_GET:
                            ret = DapiReadByte(handle, 0xfd10);
                            break;
                    }
                    break;

                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------

                case (int)DAPI_SPECIAL_CMD_WATCHDOG:
                    switch ((int)par1)
                    {
                        case (int)DAPI_SPECIAL_WATCHDOG_SET_TIMEOUT_MSEC:
                            DapiWriteLong(handle, 0xe004, par2);
                            break;

                        case (int)DAPI_SPECIAL_WATCHDOG_GET_TIMEOUT_MSEC:
                            ret = DapiReadLong(handle, 0xe004);
                            break;

                        case (int)DAPI_SPECIAL_WATCHDOG_GET_STATUS:
                            ret = DapiReadByte(handle, 0xe000);
                            break;

                        case (int)DAPI_SPECIAL_WATCHDOG_GET_WD_COUNTER_MSEC:
                            ret = DapiReadLong(handle, 0xe008);
                            break;

                        case (int)DAPI_SPECIAL_WATCHDOG_GET_TIMEOUT_RELAIS_COUNTER_MSEC:
                            ret = DapiReadLong(handle, 0xe00c);
                            break;

                        case (int)DAPI_SPECIAL_WATCHDOG_SET_TIMEOUT_REL1_COUNTER_MSEC:
                            DapiWriteLong(handle, 0xe008, par2);
                            break;

                        case (int)DAPI_SPECIAL_WATCHDOG_SET_TIMEOUT_REL2_COUNTER_MSEC:
                            DapiWriteLong(handle, 0xe00c, par2);
                            break;


                    }
                    break;

                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------

                case (int)DAPI_SPECIAL_CMD_COUNTER:
                    switch ((int)par1)
                    {
                        case (int)DAPI_SPECIAL_COUNTER_LATCH_ALL:
                            DapiWriteByte(handle, 0xfe12, 0x19);					// Latch all Counter
                            break;
                        case (int)DAPI_SPECIAL_COUNTER_LATCH_ALL_WITH_RESET:
                            DapiWriteByte(handle, 0xfe12, 0x1a);					// Latch all Counter WITH RESET
                            break;
                    }
                    break;

                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------

                case (int)DAPI_SPECIAL_CMD_CNT48:
                    switch ((int)par1)
                    {
                        case (int)DAPI_SPECIAL_CNT48_RESET_SINGLE:
                            DapiWriteByte(handle, 0x5000 + par2 * 8, 0x00);			// Reset Counter Nr. "par2"
                            break;
                        case (int)DAPI_SPECIAL_CNT48_RESET_GROUP8:
                            DapiWriteByte(handle, 0x5003 + (par2 / 8) * 0x40, 0x00);	// Reset Counter Group Nr. "par2"
                            break;
                        case (int)DAPI_SPECIAL_CNT48_LATCH_GROUP8:
                            DapiWriteByte(handle, 0x5002 + (par2 / 8) * 0x40, 0x00);	// Latch Counter Group Nr. "par2"
                            break;
                        case (int)DAPI_SPECIAL_CNT48_DI_GET1:
                            ret = DapiReadByte(handle, 0x5007 + (par2 * 8)) & 1;			// Bit 0 Latch Counter Group Nr. "par2"

                            //sprintf(msg,"|DELIB|DAPI_SPECIAL_CMD_CNT48: Erase State 2   par2=%d    ret = 0x%x\n", par2, ret);
                            //debug_print(msg);

                            break;
                    }
                    break;

                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------

                case (int)DAPI_SPECIAL_CMD_SOFTWARE_FIFO:
                    switch ((int)par1)
                    {
                        case (int)DAPI_SPECIAL_SOFTWARE_FIFO_ACTIVATE:
                            DapiWriteByte(handle, 0xfe30, 0x01);					// activate software fifo
                            break;
                        case (int)DAPI_SPECIAL_SOFTWARE_FIFO_DEACTIVATE:
                            DapiWriteByte(handle, 0xfe30, 0x00);					// deactivate software fifo
                            break;
                        case (int)DAPI_SPECIAL_SOFTWARE_FIFO_GET_STATUS:
                            ret = DapiReadByte(handle, 0xfe30) & 1;					// read software-fifo status (activated / deactivated)
                            break;
                    }
                    break;


                case (int)DAPI_SPECIAL_CMD_TEMP:

                    switch ((int)par1)
                    {
                        case (int)DAPI_SPECIAL_TEMP_READ_MULIPLE_TEMP:
                            //ret = DapiReadMultipleBytes(handle, 0x1000 + par2*4, (par3-par2+1)*4, 1, global_ad_buffer, (par3-par2)*4);
                            ret = DapiReadMultipleBytes(handle, 0x4000 + par2 * 8, (par3 - par2 + 1) * 8, 1, global_temp_buffer, (par3 - par2) * 8);
                            break;
                    }
                    break;

                case (int)DAPI_SPECIAL_CMD_PWM:

                    switch ((int)par1)
                    {
                        case (int)DAPI_SPECIAL_PWM_READBACK_MULIPLE_PWM:
                            //ret = DapiReadMultipleBytes(handle, 0x1000 + par2*4, (par3-par2+1)*4, 1, global_ad_buffer, (par3-par2)*4);
                            ret = DapiReadMultipleBytes(handle, 0x800 + par2, (par3-par2+1), 1, global_pwm_buffer, (par3-par2));
                            Sess.Log(string.Format("DapiReadMultipleBytes(0x{0}, {1}, 1, buffer, {2}) ** ret = {3}", (0x800 + par2).ToString("X"), (par3 - par2 + 1), (par3 - par2), ret));
                            break;
                    }


                    break;
            }
            return ret;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiWatchdogEnable(uint handle)
        {
            DapiWriteByte(handle, 0xe000, 0x23);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiWatchdogDisable(uint handle)
        {
            DapiWriteByte(handle, 0xe000, 0x12);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiWatchdogRetrigger(uint handle)
        {
            DapiWriteByte(handle, 0xe001, 0x34);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
    }
}
