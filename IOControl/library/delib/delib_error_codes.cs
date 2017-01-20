//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//
//
//
//	delib_error_codes.cs
//	project: DELIB
//
//
//  (c) DEDITEC GmbH, 2016
//  web: http://www.deditec.de/
//  mail: vertrieb@deditec.de
//
//
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************

using System;
using System.Text;

namespace DeLib
{
    public class DeLibErrorCodes
    {

		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		//	= 0x0000		
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		//
		public const uint DAPI_ERR_NONE												= 0;
		//
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		//	= 0x0100		Allgemeine Error Codes
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		//
		public const uint DAPI_ERR_GEN_CLASS										= 0x0100;
		//
		public const uint DAPI_ERR_GEN_MALLOC_ERROR									= 0x0101;
		public const uint DAPI_ERR_GEN_BUFFER_TOO_SMALL_ERROR						= 0x0102;
		public const uint DAPI_ERR_GEN_ERROR_WITH_TEXT								= 0x0103;

		public const uint DAPI_ERR_GEN_SOCKET_ERROR									= 0x0110;
		public const uint DAPI_ERR_GEN_BINDING_ERROR								= 0x0111;
		public const uint DAPI_ERR_GEN_SIG_HANDLER_ERROR							= 0x0112;

		public const uint DAPI_ERR_GEN_UNKNOWN_ENCRYPTION_TYPE						= 0x0120;
		public const uint DAPI_ERR_GEN_ENCRYPTION_ERROR								= 0x0121;

		public const uint DAPI_ERR_GEN_ILLEGAL_MODULE_ID							= 0x0130;
		public const uint DAPI_ERR_GEN_ILLEGAL_MODULE_NR							= 0x0131;
		public const uint DAPI_ERR_GEN_ILLEGAL_HANDLE								= 0x0132;

		public const uint DAPI_ERR_GEN_NOT_SUPPORTED_MODE							= 0x0140;
		public const uint DAPI_ERR_GEN_NOT_SUPPORTED_IO_TYPE						= 0x0141;
		public const uint DAPI_ERR_GEN_NOT_SUPPORTED_CHANNEL						= 0x0142;
		public const uint DAPI_ERR_GEN_NOT_SUPPORTED_SW_FEATURE						= 0x0143;
		public const uint DAPI_ERR_GEN_NOT_SUPPORTED_FUNCTION						= 0x0144;

		//
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		//	= 0x0200	Communication Errors
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		//
		public const uint DAPI_ERR_COM_CLASS										= 0x0200;

		public const uint DAPI_ERR_COM_CONN_COULD_NOT_BE_ESTABLISHED				= 0x0201;
		public const uint DAPI_ERR_COM_DEVICE_DID_NOT_ANSWER						= 0x0202;
		//
		public const uint DAPI_ERR_COM_HANDLE_INVALID								= 0x0210;
		public const uint DAPI_ERR_COM_DELIB_ID_INVALID								= 0x0211;
		public const uint DAPI_ERR_COM_FT_HANDLE_INVALID							= 0x0212;
        public const uint DAPI_ERR_COM_FT_ERROR										= 0x0213;
		//
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		//	= 0x0300	Device Errors
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		//
		public const uint DAPI_ERR_DEV_CLASS										= 0x0300;
		//
		public const uint DAPI_ERR_DEV_PKT_CMD_ILLEGAL								= 0x0301;		//ehemals ERR_EXECUTE_RS232_RX_COMMAND_ILLEGAL_COMMAND     1
		public const uint DAPI_ERR_DEV_PKT_DATA_LENGTH_ERROR						= 0x0302;		//ehemals ERR_EXECUTE_RS232_RX_COMMAND_WRONG_DATA_LENGTH   2
		public const uint DAPI_ERR_DEV_PKT_DATA_CHECKSUM_ERROR						= 0x0303;		//ehemals ERR_EXECUTE_RS232_RX_COMMAND_WRONG_CHECKSUM      3
		public const uint DAPI_ERR_DEV_PKT_ADDR_NOT_FOR_ME							= 0x0304;		//ehemals ERR_EXECUTE_RS232_RX_COMMAND_NOT_FOR_ME			 4
		public const uint DAPI_ERR_DEV_PKT_OTHER_ERROR								= 0x0305;		//ehemals ERR_EXECUTE_RS232_RX_COMMAND_NOT_FOR_ME			 4
		//
		public const uint DAPI_ERR_DEV_PACKET_CMD_NOT_SUPPORTED						= 0x0310;		// BC-CMD or TCP-CMD
		public const uint DAPI_ERR_DEV_PACKET_ID_1_NOT_ALLOWED						= 0x0311;		// TCP
		public const uint DAPI_ERR_DEV_PACKET_ID_1_NOT_SUPPORTED					= 0x0312;		// TCP
		public const uint DAPI_ERR_DEV_PACKET_HEADER_NOT_OK							= 0x0313;		// TCP
        public const uint DAPI_ERR_DEV_PACKET_SUBCMD_NOT_SUPPORTED                  = 0x0314;		// TCP
		//
		public const uint DAPI_ERR_DEV_IO_ADDR_ILLEGAL								= 0x0331;		// dev_io (1. von 7 möglichen Errors)
		public const uint DAPI_ERR_DEV_IO_RD_WR_ERROR								= 0x0332;		// dev_io (2. von 6 möglichen Errors)
		public const uint DAPI_ERR_DEV_IO_SUB_ADDR_ILLEGAL							= 0x0333;		// dev_io (3. von 7 möglichen Errors)
        public const uint DAPI_ERR_DEV_IO_OTHER_ERROR								= 0x0337;		// dev_io (7. von 7 möglichen Errors)
        public const uint DAPI_ERR_DEV_IO_HTML_ACCESS_DENIED						= 0x0338;
        //
        public const uint DAPI_ERR_DEV_FS_INTERNAL_TEXT_BUFFER_TOO_SMALL			= 0x0345;
        public const uint DAPI_ERR_DEV_FS_RW_OFFSET_OUT_OF_SCOPE					= 0x0346;
        public const uint DAPI_ERR_DEV_FS_RW_LENGTH_OUT_OF_SCOPE					= 0x0347;
        public const uint DAPI_ERR_DEV_FS_BUFFER_TOO_SMALL							= 0x0349;
        //
		public const uint DAPI_ERR_DEV_FILE_WRITE_ERROR								= 0x0370;
		//
		public const uint DAPI_ERR_DEV_CONFIG_WRITE_PROTECTED						= 0x0380;
		public const uint DAPI_ERR_DEV_CONFIG_KEY_NOT_PRESSED						= 0x0381;
		public const uint DAPI_ERR_DEV_CONFIG_READ_ERROR							= 0x0382;
		public const uint DAPI_ERR_DEV_CONFIG_WRITE_ERROR							= 0x0383;
		public const uint DAPI_ERR_DEV_CONFIG_UPDATE_ERROR							= 0x0384;		// Broadcast
		public const uint DAPI_ERR_DEV_CONFIG_PARAM_ONLY_ONE_TIME_WRITEABLE			= 0x0385;		// ETH
        public const uint DAPI_ERR_DEV_CONFIG_PARAM_IS_READ_ONLY					= 0x0386;
        public const uint DAPI_ERR_DEV_CONFIG_PARAM_IS_WRITE_ONLY					= 0x0387;
        public const uint DAPI_ERR_DEV_CONFIG_WRITE_PROTECTED_SW					= 0x0388;
        public const uint DAPI_ERR_DEV_CONFIG_FS_INVALID							= 0x0389;
        public const uint DAPI_ERR_DEV_CONFIG_FS_DIR_NOT_FOUND						= 0x038a;
        public const uint DAPI_ERR_DEV_CONFIG_FS_DIR_ALREADY_EXIST					= 0x038b;
        public const uint DAPI_ERR_DEV_CONFIG_FS_IS_FULL							= 0x038c;
        public const uint DAPI_ERR_DEV_CONFIG_FS_DIR_DATA_INVALID                   = 0x038d;
		//
		public const uint DAPI_ERR_DEV_ENCRYPTED_HEADER_NOT_OK						= 0x0391;
		public const uint DAPI_ERR_DEV_ENCRYPTION_ERROR								= 0x0392;		// Encryption
		public const uint DAPI_ERR_DEV_ENCRYPTION_TEMP_ADMIN_MODE_NOT_ALLOWED		= 0x0393;
		public const uint DAPI_ERR_DEV_ENCRYPTION_NO_ADMIN_RIGHTS					= 0x0394;
		//
		public const uint DAPI_ERR_DEV_EXECUTE_CMD_ERROR							= 0x03FF;
		//
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------

	}
}
