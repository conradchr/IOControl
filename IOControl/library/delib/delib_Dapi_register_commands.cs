
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//
//
//
//	delib_Dapi_register_commands.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
//using System.Net;
//using System.Net.Sockets;
using System.Text;
using System.IO;

using Android.Util; // log

//using deditec_io_control;


using System.Runtime.InteropServices;   // GCHandle


using Xamarin.Forms;    // DependencyService


namespace DELIB
{
    public partial class DeLibNET
    {
        public static void CatchException(Exception e)
        {
            
        }

		public static string toast;

        /*
        private static uint job_id = 0;
        private static uint amount_bytes_received;
        private static Byte[] buffer = new Byte[4000];
        private static uint dapi_last_error = 0;
        private static String dapi_last_error_text;

		public const uint TCP_RO_BIG_RX_HEADER_LENGTH = 9;		// id(2) + answer(1) + ok(1) + job_id(1) + length(4)
		public const uint TCP_RO_BIG_TX_HEADER_LENGTH = 8;		// id(2) + ro-cmd(1) + job_id(1) + length(4)

		public const uint TCP_RO_1_RX_HEADER_LENGTH = 7;		// id(2) + answer(1) + ok(1) + job_id(1) + length(2)

		public const uint DEDITEC_TCP_PACKET_ID_2_CMD_RO_BIG = 0x02;

        

        static TcpClient client;
        static public NetworkStream stream;
        */

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // Management

        public static uint DapiOpenModuleEx(String ipAdresseDeditec, uint portNoDeditec, uint timeout_ms, String enc_pw)
        {
            /*
			DapiHandle dapiHandle = new DapiHandle();
            dapiHandle.ip_addr = ipAdresseDeditec;
            dapiHandle.port = (int) portNoDeditec;
            dapiHandle.timeout = (int) timeout_ms;

            dapiHandle.encryption_type = DT.DAPI_OPEN_MODULE_ENCRYPTION_TYPE_ADMIN;
            dapiHandle.encryption_password = enc_pw;

            try
            {
                dapiHandle.TCP_IO.client = new TcpClient(dapiHandle.ip_addr, dapiHandle.port);
                dapiHandle.TCP_IO.stream = dapiHandle.TCP_IO.client.GetStream();
                dapiHandle.TCP_IO.client.ReceiveTimeout = dapiHandle.timeout;
                dapiHandle.TCP_IO.client.SendTimeout = dapiHandle.timeout;
            }
            catch (Exception e) 
			{ 
                DT.DBG.Error("DapiOpenModuleEx Error!\n" + e.ToString());
                return 0;
			}

            return (uint)((IntPtr)GCHandle.Alloc(dapiHandle));
            */

            var delibService = DependencyService.Get<IOControl.IDelibUtils>();
            return delibService.DapiOpenModuleEx(ipAdresseDeditec, portNoDeditec, timeout_ms, enc_pw);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiCloseModule(uint handle)
        {
            /*
            try
            {
                DapiHandle dapiHandle = (DapiHandle) ((GCHandle)((IntPtr) handle)).Target;
                dapiHandle.TCP_IO.stream.Close();
                dapiHandle.TCP_IO.client.Close();
                dapiHandle = null;
            }
            catch (Exception e)
            {
                DT.DBG.Error("DapiCloseModule Error!\n" + e.ToString());
            }
            */
            var delibService = DependencyService.Get<IOControl.IDelibUtils>();
            delibService.DapiCloseModule(handle);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // Register Access

        public static void DapiWriteByte(uint handle, uint address, uint value)
        {
            DT.TCP.DapiTCPWriteByte(handle, address, value);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiWriteWord(uint handle, uint address, uint value)
        {
            DT.TCP.DapiTCPWriteWord(handle, address, value);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static void DapiWriteLong (uint handle, uint address, uint value)
        {
            DT.TCP.DapiTCPWriteLong(handle, address, value);
        }

		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------

		public static void DapiWriteLongLong (uint handle, uint address, ulong value)
		{
            DT.TCP.DapiTCPWriteLongLong(handle, address, value);
		}

		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------

        public static uint DapiReadByte(uint handle, uint address)
        {
            return DT.TCP.DapiTCPReadByte(handle, address);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static uint DapiReadWord(uint handle, uint address)
        {
            return DT.TCP.DapiTCPReadWord(handle, address);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static uint DapiReadLong (uint handle, uint address)
        {
            return DT.TCP.DapiTCPReadLong(handle, address);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

		public static ulong DapiReadLongLong (uint handle, uint address)
		{
            return DT.TCP.DapiTCPReadLongLong(handle, address);
		}

		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------

        public static uint DapiReadMultipleBytes(uint handle, uint address, uint address_depth, uint repeat, Byte[] buff, uint buff_len)
        {
            return DT.TCP.DapiTCPReadMultipleBytes(handle, address, address_depth, repeat, buff, buff_len);
        }

		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------

		public static uint DapiWriteMultipleBytes(uint handle, uint address, uint address_depth, uint repeat, Byte[] buff, uint buff_len)
		{
            return 0;

            /*
			Byte[] tx_buffer;

			uint tx_cnt = 0;
			uint buffer_length;

			uint i;
			uint j;
			uint pos = 0;

			
			// speicherallokierung für tx_buffer + check 
			buffer_length = ((address_depth + 3) * repeat);	// header(8) + wiederholung * (adresstiefe + start_id(1) + data_id(2))
			tx_buffer = new Byte[buffer_length + TCP_RO_BIG_TX_HEADER_LENGTH];


			if (buff.Length < buffer_length)
			{				
				return DT.RETURN_ERROR;			// Error
			}	


			// tx_buffer ok - daten füllen
			tx_buffer[tx_cnt++] = (byte)0x63;									// Packet_ID_o
			tx_buffer[tx_cnt++] = (byte)0x9a;									// Packet_ID_1

			tx_buffer[tx_cnt++] = (byte)DEDITEC_TCP_PACKET_ID_2_CMD_RO_BIG;		// DEDITEC_TCP_PACKET_ID_2_CMD_RO_BIG
			tx_buffer[tx_cnt++] = (byte)job_id++;								// job_id

			tx_buffer[tx_cnt++] = (byte)0;										// Placeholder for LENGTH Bit24-31
			tx_buffer[tx_cnt++] = (byte)0;										// Placeholder for LENGTH Bit16-23
			tx_buffer[tx_cnt++] = (byte)0;										// Placeholder for LENGTH Bit8-15
			tx_buffer[tx_cnt++] = (byte)0;										// Placeholder for LENGTH Bit0-7

			tx_buffer[tx_cnt++]=(byte)'W';										// COMMAND //Write
			tx_buffer[tx_cnt++]=(byte)'M';										// WIDTH //Multiple

			// address (2byte)
			tx_buffer[tx_cnt++]=(byte)((address >> 8 ) & 0xff);					// ADDRESS_BIT_8_15
			tx_buffer[tx_cnt++]=(byte)((address >> 0 ) & 0xff);					// ADDRESS_BIT_0_7
			
			// address_depth (2byte)
			tx_buffer[tx_cnt++]=(byte)((address_depth >> 8 ) & 0xff);			// ADDRESS_DEPTH_BIT_8_15
			tx_buffer[tx_cnt++]=(byte)((address_depth >> 0 ) & 0xff);			// ADDRESS_DEPTH_BIT_0_7
			
			// repeat (2byte)
			tx_buffer[tx_cnt++]=(byte)((repeat >> 8 ) & 0xff);					// repeat_BIT_8_15
			tx_buffer[tx_cnt++]=(byte)((repeat >> 0 ) & 0xff);					// repeat_BIT_0_7
			
			// data start
			for (i=0; i<repeat; i++)
			{
				// START_ID_MULTIPLE_BYTE (1byte)
				tx_buffer[tx_cnt++]= (byte)DEDITEC_TCP_START_ID_FOR_MULTIPLE_BYTE_DATA;		// START_ID_MULTIPLE_BYTE
				
				// DATA_ID (2byte)
				tx_buffer[tx_cnt++]=(byte)((i >> 8 ) & 0xff);					// DATA_ID_BIT_8_15
				tx_buffer[tx_cnt++]=(byte)((i >> 0 ) & 0xff);					// DATA_ID_BIT_0_7
				
				// data
				for (j=0; j<address_depth; j++)
				{
					tx_buffer[tx_cnt++]= buff[pos++];
				}
			}


			tx_buffer[4] = (byte)((tx_cnt >> 24) & 0xff);						//LENGTH_BIT_24_31
			tx_buffer[5] = (byte)((tx_cnt >> 16) & 0xff);						//LENGTH_BIT_16_23
			tx_buffer[6] = (byte)((tx_cnt >> 8) & 0xff);						//LENGTH_BIT_8_15
			tx_buffer[7] = (byte)((tx_cnt >> 0) & 0xff);						//LENGTH_BIT_0_7

			if (job_id > 255) 
			{
				job_id = 0;
			}

			try 
			{
				// send
				stream.Write(tx_buffer, 0, tx_buffer.Length);
				stream.Flush();			

				// recv
				amount_bytes_received = (uint)stream.Read(buffer, 0, buffer.Length);
				CheckIsValidDeditecAnswer();
			}
			catch (IOException ioe)
			{
				SetError(DAPI_ERR_DEVICE_NOT_FOUND);
				return 1;
			}

			return DT.RETURN_OK;
   */         

		}

		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------

		public static void CheckIsValidDeditecAnswer()
		{
            /*
			int recv_pos = 0;

			// PACKET_ID_0 checken
			if((buffer[recv_pos++] & 0xFFL) != DEDITEC_PACKET_ID_0)
			{
				SetError(DAPI_ERR_COMMUNICATION_ERROR);
				return;
			}

			// PACKET_ID_1 checken
			if((buffer[recv_pos++] & 0xFFL) != DEDITEC_PACKET_ID_1)
			{
				SetError(DAPI_ERR_COMMUNICATION_ERROR);
				return;
			}

			// TCP_ANSWER_RO_1 checken
			if((buffer[recv_pos++] & 0xFFL) != DEDITEC_TCP_ANSWER_RO_1)
			{
				SetError(DAPI_ERR_COMMUNICATION_ERROR);
				return;
			}

			// TCP_ANSWER_OK checken
			if((buffer[recv_pos++] & 0xFFL) != DEDITEC_TCP_ANSWER_OK)
			{
				SetError(DAPI_ERR_COMMUNICATION_ERROR);
				return;
			}
   */         
		}

		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------

		public static void SetError(uint errorCode)
		{
            /*
			if (errorCode == DAPI_ERR_DEVICE_NOT_FOUND)
			{
				dapi_last_error_text = "Device not found!";
			}
   */         
		}

		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------

		public static bool IsError()
		{
            /*
			if (dapi_last_error != DAPI_ERR_NONE)
			{
				//System.out.printf("Error - Error Code = 0x%x\nMessage = %s\n", dapi_last_error,dapi_last_error_text);
				string toast = string.Format ("Error - Error Code = {0}", dapi_last_error);
				// clear last error
				dapi_last_error = DAPI_ERR_NONE;

				return true;
			}
*/
			return false;
		}

		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------

    }
}
