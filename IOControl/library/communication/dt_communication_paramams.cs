//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//
//
//
//	dt_communication_paramams.cs
//	project: DELIB
//
//
//	(c) DEDITEC GmbH, 2015
//	web: http://www.deditec.de/
//	mail: vertrieb@deditec.de
//
// 
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************

using System;

public partial class DT
{

//
// ----------------------------------------------------------------------------
public const uint DT_FTDI_USB_CMD_PING		= 0x12;
public const uint DT_FTDI_USB_CMD_SEND8BYTE	= 0x23;
public const uint DT_FTDI_USB_CMD_GET8BYTE	= 0x34;
public const uint DT_FTDI_USB_CMD_SENDNBYTE	= 0x45;
public const uint DT_FTDI_USB_CMD_GETNBYTE	= 0x56;

// ----------------------------------------------------------------------------
public const uint CAN_CMD_REG_READ	= 0x10;
public const uint CAN_CMD_REG_WRITE	= 0x20;
public const uint CAN_CMD_GET_INFO	= 0xf0;

// ----------------------------------------------------------------------------
	public const uint CAN_DATA_NOTHING	= 0x00;
	public const uint CAN_DATA_BIT = 0x01;
	public const uint CAN_DATA_BYTE = 0x02;
	public const uint CAN_DATA_WORD = 0x03;
	public const uint CAN_DATA_LONG = 0x04;

// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
// TCP-IP und BC Parameter
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------

// ------------------------------------
		public const uint DEDITEC_EMBEDDED_BC_PORT							= 9912;
	public const uint DEDITEC_DEFAULT_TCP_PORT_NO							= 9912;

// ------------------------------------
// TCP TX and RX ID's
public const uint DEDITEC_TCP_PACKET_ID_0								= 0x63;
public const uint DEDITEC_TCP_PACKET_ID_1_NORMAL						= 0x9a;
public const uint DEDITEC_TCP_PACKET_ID_1_ENCRYPT_NORMAL				= 0x9b;
public const uint DEDITEC_TCP_PACKET_ID_1_ENCRYPT_ADMIN				= 0x9c;
public const uint DEDITEC_TCP_PACKET_ID_1_ENCRYPT_ADMIN_TEMP			= 0x9d;

public const uint DEDITEC_TCP_PACKET_ID_2_CMD_RO_1					= 0x01;
public const uint DEDITEC_TCP_PACKET_ID_2_CMD_RO_BIG					= 0x02;
public const uint DEDITEC_TCP_PACKET_ID_2_CMD_SEND_FILE				= 0x03;
public const uint DEDITEC_TCP_PACKET_ID_2_CMD_SPECIAL					= 0x04;

public const uint DEDITEC_TCP_PACKET_ID_2_ANSWER_RO_1					= 0x81;
public const uint DEDITEC_TCP_PACKET_ID_2_ANSWER_RO_BIG				= 0x82;
public const uint DEDITEC_TCP_PACKET_ID_2_ANSWER_CMD_SEND_FILE		= 0x83;
public const uint DEDITEC_TCP_PACKET_ID_2_ANSWER_CMD_SPECIAL			= 0x84;

// ------------------------------------
public const uint DEDITEC_TCPSPECIAL_CMD_CMD_LINUX_INSTALL_UPD		= 0x0001;
public const uint DEDITEC_TCPSPECIAL_CMD_CMD_BOOTLDR_DATA				= 0x0002;
public const uint DEDITEC_TCPSPECIAL_CMD_PARAMETER_READ				= 0x524e;	//RN
public const uint DEDITEC_TCPSPECIAL_CMD_PARAMETER_WRITE				= 0x574e;	//WN
public const uint DEDITEC_TCPSPECIAL_CMD_IDENTIFY						= 0x0003;
public const uint DEDITEC_TCPSPECIAL_CMD_ACTIVATE_ADMIN_TEMP			= 0x0004;
public const uint DEDITEC_TCPSPECIAL_CMD_RELOAD_ETH_CONFIG			= 0x0005;
public const uint DEDITEC_TCPSPECIAL_CMD_GET_CURRENT_CONFIG			= 0x0006;
public const uint DEDITEC_TCPSPECIAL_CMD_SET_MAC_ADDR					= 0x0007;
public const uint DEDITEC_TCPSPECIAL_CMD_GET_MAC_ADDR					= 0x0008;

public const uint DEDITEC_TCPSPECIAL_CMD_PARAMETER_READ_MULTIPLE		= 0x0010;
public const uint DEDITEC_TCPSPECIAL_CMD_PARAMETER_WRITE_MULTIPLE		= 0x0011;
// ------------------------------------
public const uint CONNECTION_TIMEOUT_TO_CLIENT 						= 5;
// timeout value in seconds; server closes connection to client after x seconds

// ------------------------------------
public const uint DEDITEC_TCP_START_ID_FOR_MULTIPLE_BYTE_DATA			= 35;

// ------------------------------------
// Broadcast TX and RX ID's

public const uint DEDITEC_BC_PACKET_TX_ID_0 							= 0x12;
public const uint DEDITEC_BC_PACKET_TX_ID_1 							= 0x47;
public const uint DEDITEC_BC_PACKET_TX_ID_2 							= 0x22;
public const uint DEDITEC_BC_PACKET_TX_ID_3 							= 0x01;

public const uint DEDITEC_BC_PACKET_RX_ID_0 							= 0x13;
public const uint DEDITEC_BC_PACKET_RX_ID_1 							= 0x47;
public const uint DEDITEC_BC_PACKET_RX_ID_2 							= 0x22;
public const uint DEDITEC_BC_PACKET_RX_ID_3 							= 0x01;

public const uint DEDITEC_BC_PACKET_CMD_GLOBAL_CALL					= 0x01;
public const uint DEDITEC_BC_PACKET_CMD_PARAM_GET						= 0x02;
public const uint DEDITEC_BC_PACKET_CMD_PARAM_SET						= 0x03;
public const uint DEDITEC_BC_PACKET_CMD_ETH0_CONFIGURE				= 0x04;
public const uint DEDITEC_BC_PACKET_CMD_GLOBAL_CALL_WITH_DEV_CFG		= 0x05;

public const uint DEDITEC_BC_PACKET_PARAM_BOARD_NAME					= 0x01;
public const uint DEDITEC_BC_PACKET_PARAM_IP_ADDR						= 0x02;
public const uint DEDITEC_BC_PACKET_PARAM_NETMASK						= 0x03;
public const uint DEDITEC_BC_PACKET_PARAM_STDGATEWAY					= 0x04;
public const uint DEDITEC_BC_PACKET_PARAM_DNS1						= 0x05;
public const uint DEDITEC_BC_PACKET_PARAM_DHCP						= 0x06;
public const uint DEDITEC_BC_PACKET_PARAM_DELIB_MODULE_ID				= 0x07;
public const uint DEDITEC_BC_PACKET_PARAM_MAC_ADDR					= 0x08;
public const uint DEDITEC_BC_PACKET_PARAM_INFO_STRING					= 0x09;
public const uint DEDITEC_BC_PACKET_PARAM_TCP_CONFIG_USED				= 0x0a;
public const uint DEDITEC_BC_PACKET_PARAM_KEY_HANDLER_LOG				= 0x0b;
public const uint DEDITEC_BC_PACKET_PARAM_IDENTIFY					= 0x0c;


// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
// Encryption
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------

public const string DEDITEC_ENCRYPTION_TEMPORARY_ADMIN_PW	=			"!?d3d1t3c!?4711!?";
public const string DEDITEC_ENCRYPTION_NORMAL_DEFAULT_PW	=			"normal";
public const string DEDITEC_ENCRYPTION_ADMIN_DEFAULT_PW		=			"admin";


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

// RO-Modules-REGISTER
// --------------------
// Registerbelegung:
// Register = 0x00-= 0x0f (write)- Ausgänge
// Register = 0x00-= 0x0f (read) - Ausgänge zurücklesen

// Register = 0x20-= 0x2f (read)- Eingänge
// Register = 0x40-= 0x4f (read)- Eingangs-FF's (incl. Zurücksetzen)

// Register = 0xf0 - Config Timeout (0=disabled, 1-255 sind Zeit-Werte)
// Register = 0xf1 - Config ???

// Register = 0xfff8-fffe	8Bit readback Register (zum Test)
// Register = 0xfff4-fff7		32 Bit Firmware-Version
// Register = 0xfff0-fff3		32 Bit SW-Feature Bits (Type of Software Features)
// Register = 0xffec-ffef		32 Bit HW-Feature Bits (Type of Hardware)
// Register = 0xff00-ff01		16 Bit No of DO Chan 
// Register = 0xff02-ff03		16 Bit No of DI Chan 
// Register = 0xff04-ff05		16 Bit No of DX Chan 
// Register = 0xff06-ff07		16 Bit No of AD Chan 
// Register = 0xff08-ff09		16 Bit No of DA Chan 
// Register = 0xff0a-ff0b		16 Bit No of Stepper Chan 

// = 0xffd0-df		: Gibt die Modul-Identifikations-Nummer des Moduls zurück
// Für den Direktzugriff auf die einzelnen Module

// = 0xfe00			: Modul-Nummer, auf die geschrieben werden soll
// = 0xfe02			: Register, auf das geschrieben werden soll
// = 0xfe03			: Daten, die geschrieben werden sollen 
// = 0xfe04+5			: Daten, die gelesen werden

// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------

// 8 Bytes CAN-Data Command zum Modul
// ----------------------------------
// 1.Byte = Command
// 2.Byte = Register Bit 0-7
// 3.Byte = Register Bit 8-15
// 4.Byte = JOB-ID
// 5.Byte = Data (0-7)
// 6.Byte = Data (8-15)
// 7.Byte = Data (16-23)
// 8.Byte = Data (24-31)

// Command = (= 0x10=read,= 0x20=write,= 0xf0=get info) 	OR gewünschte Datenbits (0=nix,1=Bit,2=Byte,3=Word,4=Long)

// 8 Bytes CAN-Data ANTWORT vom Modul
// ----------------------------------
// 1.Byte = 1 = OK, = 0xfd=JOB-ID nochmals vorhanden, = 0xfe Befehl ungültig
// 2.Byte = Was ist an EMPFANGENEN Daten gültig (0=nix, 1=Bit, 2=Byte, 3 Word, 4=Long)
// 3.Byte = JOB-ID
// 4.Byte = Data (0-7)
// 5.Byte = Data (8-15)
// 6.Byte = Data (16-23)
// 7.Byte = Data (24-31)


// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------

// Protokoll für Serielle Kommandos
//								Daten 	*	Seriell übertragen in hex	*	ASCII
// 1.Zeichen = <SOH>			<SOH>	*	= 0x01						*	<SOH>
// 2.Zeichen = MODUL-NR			= 0x34	*	= 0x33, = 0x34					*	(34)
// 2.Zeichen = JOB-ID			= 0x12	*	= 0x31, = 0x32					*	(12)
// 3.Zeichen = COMMAND			'W'		*	= 0x57						*	(W)
// 3.Zeichen = WIDTH			'B'		*	= 0x42						*	(B)
// 4.Zeichen = Adresse			= 0x00	*	= 0x30, = 0x30					*	(00)
// 5.Zeichen = Adresse			= 0x12	*	= 0x31, = 0x32					*	(12)
// 6.Zeichen = Daten			= 0x0f	*	= 0x30, = 0x46					*	(0F)
// 7.Zeichen = Checksumme		Summe	*	= 0x33, = 0x41					*	(3A) (Summe== 0x23a -> (nur 8 Bit -> = 0x3a)
// 8.Zeichen = <CR>				= 0x0d	*	= 0x0d						*	<CR>


// COMMAND'S : W = write, R=read
// Command=W,R greift auf Register zu

// WIDTH= B -> Byte		(-> es folgen 2 Bytes für Daten)
// WIDTH= W -> 16 Bit	(-> es folgen 4 Bytes für Daten)
// WIDTH= L -> 32 Bit	(-> es folgen 8 Bytes für Daten)
// WIDTH= X -> 64 Bit	(-> es folgen 16 Bytes für Daten)
// WIDTH= M -> 128 Bit	(-> es folgen 32 Bytes für Daten)

//public const uint SERIELL_CMD_WIDTH_8		'B'
//public const uint SERIELL_CMD_WIDTH_16	'W'
//public const uint SERIELL_CMD_WIDTH_32	'L'
//public const uint SERIELL_CMD_WIDTH_64	'X'
//public const uint SERIELL_CMD_WIDTH_128	'M'

//public const uint ERR_EXECUTE_RS232_RX_COMMAND_OK					0
//public const uint ERR_EXECUTE_RS232_RX_COMMAND_ILLEGAL_COMMAND		1
//public const uint ERR_EXECUTE_RS232_RX_COMMAND_WRONG_DATA_LENGTH	2
//public const uint ERR_EXECUTE_RS232_RX_COMMAND_WRONG_CHECKSUM		3
//public const uint ERR_EXECUTE_RS232_RX_COMMAND_NOT_FOR_ME			4

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

// RO-SPI->CPU Commands

public const uint RO_CPU_CMD_MODULE_COUNTER_LATCH						= 0x7c;		// Latch Counter, if Data = = 0x3a
public const uint RO_CPU_CMD_MODULE_COUNTER_LATCH_VALUE				= 0x3a	;	// Latch Counter, if Data = = 0x3a
public const uint RO_CPU_CMD_MODULE_COUNTER_LATCH_WITH_RESET_VALUE	= 0x3d;		// Latch Counter WITH RESET , if Data = = 0x3d

public const uint RO_CPU_CMD_MODULE_TIMEOUT_SHUT_OFF					= 0x7d;		// Deactivate Module, if Data = = 0x49
public const uint RO_CPU_CMD_MODULE_TIMEOUT_SHUT_OFF_VALUE			= 0x49;		// Deactivate Module, if Data = = 0x49

public const uint RO_CPU_CMD_WRITE_TO_BOOTLOADER						= 0x7e;		// Write data to bootloader
public const uint RO_CPU_CMD_GO_BOOTLOADER							= 0x7f;		// Jump to Bootloader if Data = = 0x83
public const uint RO_CPU_CMD_GO_BOOTLOADER_VALUE						= 0x83;		// Jump to Bootloader if Data = = 0x83

public const uint RO_CPU_CMD_FIFO_WRITE_ADDR_REG						= 0x76;		// Write to Fito-Addr reg
public const uint RO_CPU_CMD_FIFO_WRITE_FIFO_CMD						= 0x77;		// Write Fifo Command and execute this command

public const uint RO_CPU_CMD_OPTION_FILTER							= 0x78;		// 0=disable filter / 1..2..3..4 = Filter Level

public const uint RO_CPU_CMD_SET_EEPROM_DATA							= 0x79;		// Set EEPROM Data
public const uint RO_CPU_CMD_SET_EEPROM_ADRESSL						= 0x7a;		// Set EEPROM AdressLow
public const uint RO_CPU_CMD_SET_EEPROM_ADRESSH						= 0x7b;		// Set EEPROM AdressHigh + Initiere EEPROM Write
public const uint RO_CPU_CMD_GET_EEPROM_STATUS						= 0x7c;		// Set EEPROM AdressHigh + Initiere EEPROM Write

public const uint RO_CPU_CMD_FIFO_GET_ADDR_REG						= 0xf0;		// Get Fifo Status
public const uint RO_CPU_CMD_FIFO_GET_CMD_ANSW						= 0xf1;		// Get Fifo Status
public const uint RO_CPU_CMD_FIFO_GET_STATUS							= 0xf2;		// Get Fifo Status
public const uint RO_CPU_CMD_FIFO_READ_DATA							= 0xf3;		// Get Fifo Data

public const uint RO_CPU_CMD_GET_ERROR_CODE							= 0xf6	;	// Get ERROR Code
public const uint RO_CPU_CMD_GET_FIRMWARE_ID							= 0xf7;		// Get Firmware ID

public const uint RO_CPU_CMD_GET_V0									= 0xf8;		// Get Version Nr 0
public const uint RO_CPU_CMD_GET_V1									= 0xf9;		// Get Version Nr 1
public const uint RO_CPU_CMD_GET_V2									= 0xfa;		// Get Version Nr 2
public const uint RO_CPU_CMD_GET_V3									= 0xfb;		// Get Version Nr 3

public const uint RO_CPU_CMD_GET_ID									= 0xfc;		// Get Module-ID Info

public const uint RO_CPU_CMD_GET_CONFIG								= 0xfd;		// Bit 0 = Bootloader, Bit 1 = OP_X2 Bit 

public const uint RO_CPU_CMD_READ_FROM_BOOTLOADER						= 0xfe;		// Give back data from bootloader
public const uint RO_CPU_CMD_GIVE_BACK								= 0xff;		// Give the written Data back

// ----------------------------------------------------------------------------

public const uint RO_COMM_RETURN_OK 									= 0x01;
public const uint RO_COMM_RETURN_REG_INVALID							= 0xfe;
public const uint RO_COMM_RETURN_ERROR 								= 0xff;

// ----------------------------------------------------------------------------
// Bootloader-ID (8-Bit)
public const uint RO_BOOTLOAD_ID_RO_SER								= 0x00;
public const uint RO_BOOTLOAD_ID_RO_CAN								= 0x01;
public const uint RO_BOOTLOAD_ID_RO_AD16DA4							= 0x02;
public const uint RO_BOOTLOAD_ID_RO_DA2_ISO							= 0x03;
public const uint RO_BOOTLOAD_ID_RO_STEPPER2							= 0x04;
public const uint RO_BOOTLOAD_ID_RO_Ox_Rx								= 0x05;
public const uint RO_BOOTLOAD_ID_RO_PT100								= 0x06;
public const uint RO_BOOTLOAD_ID_RO_CNT8								= 0x07;

public const uint DT_BOOTLOAD_ID_USB_TTL_64							= 0x80;
public const uint RO_BOOTLOAD_ID_RO_USB2								= 0x81;

public const uint DT_BOOTLOAD_ID_STM2_ETH								= 0x82;

// ----------------------------------------------------------------------------
// RO-SPI Commands
public const uint RO_CPU_ID_DA2										= 1;
public const uint RO_CPU_ID_AD16_DA4									= 2;
public const uint RO_CPU_ID_AD16										= 3;
public const uint RO_CPU_ID_DA4										= 4;
public const uint RO_CPU_ID_AD16_ISO									= 5;
public const uint RO_CPU_ID_STEPPER2									= 6;
public const uint RO_CPU_ID_O8_R8										= 7;
public const uint RO_CPU_ID_PT100										= 8;
public const uint RO_CPU_ID_CNT_8										= 9;
public const uint RO_CPU_ID_CNT_IGR									= 10;
public const uint RO_CPU_ID_DA8										= 11;
public const uint RO_CPU_ID_MOS16										= 12;
public const uint RO_CPU_ID_MOS16_PWM									= 13;
public const uint RO_CPU_ID_R8_UM										= 14;

public const uint RO_CPU_ID_OUTPUT									= 0xf0;
public const uint RO_CPU_ID_INPUT										= 0xf1;
// ----------------------------------------------------------------------------

// Special RO-Functions
public const uint SPECIAL_COMMAND_FLASH_ERASE_0						= 1;
public const uint SPECIAL_COMMAND_FLASH_ERASE_1						= 2;
public const uint SPECIAL_COMMAND_REBOOT								= 3;
public const uint SPECIAL_COMMAND_RESCAN								= 4;

// RO-FIFO Commands
public const uint RO_FIFO_CMD_fifo_init 								= 1;
public const uint RO_FIFO_CMD_fifo_set_intervalL						= 2;
public const uint RO_FIFO_CMD_fifo_set_intervalH						= 3;
public const uint RO_FIFO_CMD_fifo_enable_chL							= 4;
public const uint RO_FIFO_CMD_fifo_enable_chH							= 5;
public const uint RO_FIFO_CMD_fifo_enable_disable						= 6;

// RO-FIFO Status
public const uint RO_FIFO_STATUS_MASK_MEASURE_ENABLED	 				= 0x80;
public const uint RO_FIFO_STATUS_MASK_TEST_DATA		 				= 0x40;
public const uint RO_FIFO_STATUS_MASK_OVERFLOW		 				= 0x20;
public const uint RO_FIFO_STATUS_MASK_UNDERRUN		 				= 0x10;
public const uint RO_FIFO_STATUS_FULL_256_BYTE		 				= 0x08;
public const uint RO_FIFO_STATUS_FULL_64_BYTE			 				= 0x04;
public const uint RO_FIFO_STATUS_FULL_16_BYTE			 				= 0x02;
public const uint RO_FIFO_STATUS_FULL_1_BYTE			 				= 0x01;

public const uint RO_FIFO_ID_START									= 0xf0;
public const uint RO_FIFO_ID_END										= 0xf1;
public const uint RO_FIFO_ID_TYPE_TEST1								= 0xfe;
public const uint RO_FIFO_ID_TYPE_AD16M0								= 0x01;

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
// EEPROM Filesystem
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------

// ------------------------------------
// Filesystem ID
public const uint DT_EE_FILESYSTEM_ID									= 0x95acd43f;

// ------------------------------------
// Filesystem Directory Types
public const uint DT_EE_FILESYSTEM_DIR_TYPE_EMPTY						= 0xff;
public const uint DT_EE_FILESYSTEM_DIR_TYPE_CONFIG					= 0x01;
public const uint DT_EE_FILESYSTEM_DIR_TYPE_MAC_ADDR					= 0x80;
public const uint DT_EE_FILESYSTEM_DIR_TYPE_ETH_CFG					= 0x81;
public const uint DT_EE_FILESYSTEM_DIR_TYPE_BOARD_NAME				= 0x82;
public const uint DT_EE_FILESYSTEM_DIR_TYPE_IO_NAMES					= 0x83;
public const uint DT_EE_FILESYSTEM_DIR_TYPE_IO_CONFIG					= 0x84;
public const uint DT_EE_FILESYSTEM_DIR_TYPE_ENCRYPTION				= 0x85;
public const uint DT_EE_FILESYSTEM_DIR_TYPE_NTP						= 0x86;
public const uint DT_EE_FILESYSTEM_DIR_TYPE_MAIL						= 0x87;
public const uint DT_EE_FILESYSTEM_DIR_TYPE_HTTP						= 0x88;

// ------------------------------------
// Filesystem Directory Sizes
public const uint DT_EE_FS_DIR_TYPE_ENCRYPTION_SIZE 					= 36;

// ------------------------------------
// Filesystem Directory Attributes
/*
public const uint DT_EE_FILESYSTEM_DIR_ATTR_READ_ONLY					(1<<6);
public const uint DT_EE_FILESYSTEM_DIR_ATTR_DATA_VALID				(1<<7);
*/
	public const uint DT_EE_FILESYSTEM_DIR_ATTR_READ_ONLY					= 6;
	public const uint DT_EE_FILESYSTEM_DIR_ATTR_DATA_VALID				= 7;

// ------------------------------------
// Filesystem Commands
public const uint DT_EE_FILESYSTEM_CMD_READ							= 0x01;
public const uint DT_EE_FILESYSTEM_CMD_WRITE							= 0x02;
public const uint DT_EE_FILESYSTEM_CMD_GET_DIR_ENTRIES				= 0x03;



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

	public const uint LED_BLINK_SHORT	= 10;		// 10 ms to blink short


}
