using System;
using System.Runtime.InteropServices;	// GCHandle

using Xamarin.Forms;    // DependencyService

//const int MAX_BUFFER_LENGTH = 2048;

public class DT_TCPUtils
{
    const int BUFF_TCP_RECV_SIZE 			= 16384;  	// better don't touch, used by read() and recv()
	const int TX_BUFFER_CP_LENGTH 			= 4096;

	const int TCP_RO_TX_ENCRYPTION_START	= 4;		// id(2) + ro-cmd(1) + job_id(1)
	const int TCP_RO_RX_ENCRYPTION_START	= 5;		// id(2) + answer(1) + ok(1) + job_id(1)
	const int TCP_RO_1_TX_HEADER_LENGTH 	= 6;		// id(2) + ro-cmd(1) + job_id(1) + length(2)
	const int TCP_RO_1_RX_HEADER_LENGTH 	= 7;		// id(2) + answer(1) + ok(1) + job_id(1) + length(2)
	const int TCP_RO_BIG_TX_HEADER_LENGTH 	= 8;		// id(2) + ro-cmd(1) + job_id(1) + length(4)
	const int TCP_RO_BIG_RX_HEADER_LENGTH 	= 9;		// id(2) + answer(1) + ok(1) + job_id(1) + length(4)




	static byte[] tx_buff = new byte[4096];
	static byte[] rx_buff = new byte[4096];

    public static uint SendRecData(uint handle, uint cmd, byte[] tx_buffer, uint tx_buffer_size, byte[] rx_buffer, uint rx_buffer_size, ref int rx_buffer_length)
	{
		byte[] tx_buffer_cp = new byte[TX_BUFFER_CP_LENGTH];
		byte[] recv_buffer;
		uint recv_buffer_size;
		uint recv_error;
		uint recv_pos;
		int amount_bytes_received;
		uint tx_buffer_cnt;
		uint err = 0;
		uint dt_packet_id_1;
		uint remainder;
		bool encryption_flag;
		uint encryption_length;
		string encryption_pw = "";
		
		uint tx_data_pos;
		uint rx_data_pos;


        //DapiHandle dapiHandle = (DapiHandle) ((GCHandle)((IntPtr) handle)).Target;
        var delibService = DependencyService.Get<IOControl.IDelibUtils>();
        DapiHandle dapiHandle = delibService.GetDapiHandle(handle);

        if (dapiHandle == null)
		{
			return DT.Error.DAPI_ERR_COM_HANDLE_INVALID;
		}

		if (dapiHandle.TCP_IO == null)
		{
			return DT.Error.DAPI_ERR_GEN_SOCKET_ERROR;
		}

		encryption_flag = false;

		switch (dapiHandle.encryption_type)
		{
			// ------------------------------------
			case DT.DAPI_OPEN_MODULE_ENCRYPTION_TYPE_NONE:
				dt_packet_id_1 = DT.DEDITEC_TCP_PACKET_ID_1_NORMAL;
				break;
			// ------------------------------------
			case DT.DAPI_OPEN_MODULE_ENCRYPTION_TYPE_NORMAL:
				dt_packet_id_1 = DT.DEDITEC_TCP_PACKET_ID_1_ENCRYPT_NORMAL;
				encryption_pw = dapiHandle.encryption_password;
				encryption_flag = true;
				break;		
			// ------------------------------------
			case DT.DAPI_OPEN_MODULE_ENCRYPTION_TYPE_ADMIN:
				dt_packet_id_1 = DT.DEDITEC_TCP_PACKET_ID_1_ENCRYPT_ADMIN;
				encryption_pw = dapiHandle.encryption_password;
				encryption_flag = true;
				break;
			// ------------------------------------
			case DT.DAPI_OPEN_MODULE_ENCRYPTION_TYPE_ADMIN_TEMP:
				dt_packet_id_1 = DT.DEDITEC_TCP_PACKET_ID_1_ENCRYPT_ADMIN_TEMP;
				encryption_pw = DT.DEDITEC_ENCRYPTION_TEMPORARY_ADMIN_PW;
				encryption_flag = true;
				break;
			// ------------------------------------
			default:
                DT.Log("SendRecData: Unknown encryption_type");
                return DT.Error.DAPI_ERR_GEN_UNKNOWN_ENCRYPTION_TYPE;
			// ------------------------------------
		}
		
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// SENDING MESSAGE
		
		// ---- creating message header (3 bytes for ID's, 1 byte for job_id, 2 bytes for message length-> 6 bytes total)
		tx_buffer_cnt = 0;

		// now 3 bytes for DEDITEC ID and the command-id
		tx_buffer_cp[tx_buffer_cnt++] = (byte) DT.DEDITEC_TCP_PACKET_ID_0;
		tx_buffer_cp[tx_buffer_cnt++] = (byte) dt_packet_id_1;
		tx_buffer_cp[tx_buffer_cnt++] = (byte) cmd;

		// now first byte for job_id
		tx_buffer_cp[tx_buffer_cnt++] = (byte) dapiHandle.job_id;
		
		tx_data_pos = TCP_RO_1_TX_HEADER_LENGTH;
		if (encryption_flag)
		{
			try
			{
				Array.Copy(tx_buffer_cp, 0, tx_buffer_cp, TCP_RO_TX_ENCRYPTION_START, TCP_RO_TX_ENCRYPTION_START);
			}
			catch (Exception)
			{
				return DT.Error.DAPI_ERR_GEN_BUFFER_TOO_SMALL_ERROR;
			}
			
			tx_buffer_cnt += TCP_RO_TX_ENCRYPTION_START;
			tx_data_pos += TCP_RO_TX_ENCRYPTION_START;
		}
		
		// now 2 bytes for length of receive buffer
        tx_buffer_cp[tx_buffer_cnt++] = (byte) (((tx_buffer_size + tx_data_pos) >> 8) & 255); // rotate by 8 bits and mask upper 8 bits
        tx_buffer_cp[tx_buffer_cnt++] = (byte) ((tx_buffer_size + tx_data_pos) & 255);       // mask lower 8 bits

		try
		{
			Array.Copy(tx_buffer, 0, tx_buffer_cp, (int)tx_data_pos, (int)tx_buffer_size);
		}
		catch (Exception)
		{
			return DT.Error.DAPI_ERR_GEN_BUFFER_TOO_SMALL_ERROR;
		}

		tx_buffer_cnt += tx_buffer_size;
		
		if (encryption_flag)
		{
			// first 4bytes (0=id0, 1=id1, 2=cmd, 3=jobid) won't be encrypted -> (tx_buffer_cnt-4) ... decryption starts at tx_buffer[4]
			encryption_length = tx_buffer_cnt - TCP_RO_TX_ENCRYPTION_START;
		
			// length of encryption buffer must be a multiple of 8!
			if ((remainder = (encryption_length % 8)) != 0)
			{
				// not a multiple of 8 -> fix encryption_length and set new tx_buffer_cnt
				encryption_length += (8 - remainder);
				tx_buffer_cnt = encryption_length + TCP_RO_TX_ENCRYPTION_START;
			}

            if (DT.ENC.Encrypt(DT.Conv.ConvertStringToByteArray(encryption_pw), tx_buffer_cp, (int) TCP_RO_TX_ENCRYPTION_START, (int) encryption_length) != DT.RETURN_OK)
			{
                DT.Log("SendRecData: Error while encrypting tx_buffer");
				return DT.Error.DAPI_ERR_GEN_ENCRYPTION_ERROR;
			}
		}
		
		try
		{
            /*
			dapiHandle.TCP_IO.stream.Write(tx_buffer_cp, 0, (int) tx_buffer_cnt);
			dapiHandle.TCP_IO.stream.Flush();
            */
            delibService.TCPSend(dapiHandle, tx_buffer_cp, (int)tx_buffer_cnt);
        }
		catch (Exception)
		{
            DT.Log("SendRecData: Error while SEND");
			return DT.Error.DAPI_ERR_COM_CONN_COULD_NOT_BE_ESTABLISHED;			
		}
		
		// ----------------------------------------------------------------------------
		// ----------------------------------------------------------------------------
		// RECEIVING MESSAGE
		
		rx_data_pos = TCP_RO_1_RX_HEADER_LENGTH;
        recv_buffer_size = TCP_RO_1_RX_HEADER_LENGTH + rx_buffer_size;
		if (encryption_flag)
		{
			recv_buffer_size += TCP_RO_RX_ENCRYPTION_START;
			rx_data_pos += TCP_RO_RX_ENCRYPTION_START;
			
			// first 5bytes (0=id0, 1=id1, 2=cmd, 3=ok, 4=jobid) won't be decrypted -> (recv_buffer_size-5) ... decryption starts at recv_buffer[5]
			if ((remainder = (recv_buffer_size - TCP_RO_RX_ENCRYPTION_START) % 8) != 0)
			{
				// not a multiple of 8 -> set new recv_buffer_size
				recv_buffer_size += (8 - remainder);
			}
		}

		recv_buffer = new byte[recv_buffer_size];

		try
		{
            //amount_bytes_received = dapiHandle.TCP_IO.stream.Read(recv_buffer, 0, (int) recv_buffer_size);
            amount_bytes_received = delibService.TCPReceive(dapiHandle, ref recv_buffer);
        }
		catch (Exception)
		{
            DT.Log("SendRecData: Error while RCV");
			return DT.Error.DAPI_ERR_COM_DEVICE_DID_NOT_ANSWER;
		}
		
		if (encryption_flag)
		{
			// first 5bytes (0=id0, 1=id1, 2=cmd, 3=ok, 4=jobid) won't be decrypted -> (amount_bytes_received-5) ... decryption starts at recv_buffer[5]
			encryption_length = (uint) amount_bytes_received - TCP_RO_RX_ENCRYPTION_START;
			
			if ((remainder = (encryption_length % 8)) != 0)
			{
				// not a multiple of 8 -> set new recv_buffer_size
				encryption_length += (8 - remainder);
			}

            if (DT.ENC.Decrypt(DT.Conv.ConvertStringToByteArray(encryption_pw), recv_buffer, (int) TCP_RO_RX_ENCRYPTION_START, (int) encryption_length) != DT.RETURN_OK)
			{
                DT.Log("SendRecData: Error while decrypting rx_buffer");
				return DT.Error.DAPI_ERR_GEN_ENCRYPTION_ERROR;
			}

            if (DT.Conv.ArrayCompare(recv_buffer, 0, recv_buffer, TCP_RO_RX_ENCRYPTION_START, TCP_RO_RX_ENCRYPTION_START-1) > 0)
			//if (DT.Conv.ArrayCompare(recv_buffer, 0, recv_buffer, TCP_RO_RX_ENCRYPTION_START, TCP_RO_RX_ENCRYPTION_START) > 0)
			{
                DT.Log("SendRecData: Incorrect password \""+dapiHandle.encryption_password+"\" (len="+dapiHandle.encryption_password.Length+")");
				return DT.Error.DAPI_ERR_DEV_ENCRYPTED_HEADER_NOT_OK;
			}  
		}	
		
		// antwort überprüfen
		recv_error = 0;
		recv_pos = 0;

		if (recv_buffer[recv_pos++] != DT.DEDITEC_TCP_PACKET_ID_0)				++recv_error;
		if (recv_buffer[recv_pos++] != dt_packet_id_1)							++recv_error;
		if (recv_buffer[recv_pos++] != (byte) (0x80 | cmd))					    ++recv_error;
		recv_pos++;
		if (recv_buffer[recv_pos++] != (byte) (dapiHandle.job_id & 0xff))		++recv_error;

		if (recv_error != 0)	// header not ok = error
		{
            DT.Log("SendRecData: Header of rx_msg not ok!");
			return DT.Error.DAPI_ERR_DEV_PACKET_HEADER_NOT_OK;
		}

		// device error code
		if (recv_buffer[3] != 0x0)
		{
            DT.Log("SendRecData: recv_buffer[3] != 0x0");
			return (err | DT.Error.DAPI_ERR_DEV_CLASS);
		}

		if (amount_bytes_received > 0)  // seems possible to receive an empty tcp-packet
		{
			try
			{
				//Array.Copy(recv_buffer, rx_data_pos, rx_buffer, 0, rx_buffer.Length);
                //Array.Copy(recv_buffer, rx_data_pos, rx_buffer, 0, recv_buffer.Length - rx_data_pos);
                if (rx_buffer != null)
                {
                    Array.Copy(recv_buffer, (int) rx_data_pos, rx_buffer, 0, amount_bytes_received - (int) rx_data_pos);
                }
			}
			catch (Exception)
			{
                DT.Log(String.Format("Array.Copy(recv_buffer[{0}], {1}, rx_buffer[{2}], 0, {3}-{1}={4})",
                    recv_buffer.Length, rx_data_pos, rx_buffer.Length, amount_bytes_received, amount_bytes_received-rx_data_pos)
                );

                //DT.DBG.Print("rx_data pos = " + rx_data_pos.ToString() + " recvbuffer_len = " + recv_buffer.Length.ToString() + " rx_buff_len = " + rx_buffer.Length.ToString());

                //DT.DBG.Print("rx_data pos = " + rx_data_pos.ToString());
                //DT.DBG.Print("recvbuffer_len = " + recv_buffer.Length.ToString());
                //DT.DBG.Print("rx_buff_len = " + rx_buffer.Length.ToString());


                DT.Log("SendRecData: rx_buffer too small");
				return DT.Error.DAPI_ERR_GEN_BUFFER_TOO_SMALL_ERROR;
			}
		}
		
        rx_buffer_length = (int) amount_bytes_received - (int) rx_data_pos;

		return DT.Error.DAPI_ERR_NONE;
	}

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

	public static uint DapiTCPSendCommand(uint handle, byte[] tx, int tx_length, byte[] rx, int rx_length)
	{
		int dummy = 0;

        //DapiHandle dapiHandle = (DapiHandle) ((GCHandle)((IntPtr) handle)).Target;
        var delibService = DependencyService.Get<IOControl.IDelibUtils>();
        DapiHandle dapiHandle = delibService.GetDapiHandle(handle);

        if (dapiHandle == null)
		{
			return DT.Error.DAPI_ERR_COM_HANDLE_INVALID;
		}

        dapiHandle.job_id++;

        return SendRecData(handle, DT.DEDITEC_TCP_PACKET_ID_2_CMD_RO_1, tx, (uint) tx_length, rx, (uint) rx_length, ref dummy);
	}

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint DapiTCPSpecialCmd(uint handle, uint cmd, byte[] tx, int tx_length, byte[] rx, int rx_anzahl, ref int rx_cnt)
    {
        //byte[] tx_buffer = new byte[2048];
        uint tx_cnt=0;
        int i;
        uint delib_error_code;

        tx_buff[tx_cnt++] = (byte) 'Z';        // SpecialCmd
        tx_buff[tx_cnt++] = (byte) 'C';        // SpecialCmd

        tx_buff[tx_cnt++]=(byte) ((cmd >> 8)  & 0xff);
        tx_buff[tx_cnt++]=(byte) ((cmd >> 0 ) & 0xff);

        for (i=0; i!=tx_length; i++)
        {
            tx_buff[tx_cnt++] = tx[i];
        }

        if ((delib_error_code = DapiTCPSendSpecialCommand(handle, tx_buff, (int) tx_cnt, rx, rx_anzahl, ref rx_cnt)) != DT.Error.DAPI_ERR_NONE)
        {
            rx_cnt = 0;
        }

        return delib_error_code;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint DapiTCPSendSpecialCommand(uint handle, byte[] tx, int tx_length, byte[] rx, int rx_anzahl, ref int rx_cnt)
    {
        uint delib_error_code;

        //DapiHandle dapiHandle = (DapiHandle) ((GCHandle)((IntPtr) handle)).Target;
        var delibService = DependencyService.Get<IOControl.IDelibUtils>();
        DapiHandle dapiHandle = delibService.GetDapiHandle(handle);

        if (dapiHandle == null)
        {
            return DT.Error.DAPI_ERR_COM_HANDLE_INVALID;
        }

        dapiHandle.job_id++;

        if ((delib_error_code = SendRecData(handle, DT.DEDITEC_TCP_PACKET_ID_2_CMD_SPECIAL, tx, (uint) tx_length, rx, (uint) rx_anzahl, ref rx_cnt)) != DT.Error.DAPI_ERR_NONE)
        {
            rx_cnt = 0;
        }

        return delib_error_code;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

	public static void DapiTCPWriteByte(uint handle, uint address, uint value)
	{
		int tx_pos = 0;
		uint delib_error_code;

		tx_buff[tx_pos++] = (byte)'W';							// COMMAND
		tx_buff[tx_pos++] = (byte)'B';							// WIDTH

		tx_buff[tx_pos++] = (byte)((address >> 8) & 0xff);		// ADDRESS_BIT_8_15
		tx_buff[tx_pos++] = (byte)((address >> 0) & 0xff);		// ADDRESS_BIT_0_7

		tx_buff[tx_pos++] = (byte)(value & 0xff);				// DATA_BIT_0_7

		if ((delib_error_code = DT.TCP.DapiTCPSendCommand(handle, tx_buff, tx_pos, null, 0)) != DT.Error.DAPI_ERR_NONE)
		{
			
		}
	}

	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------

	public static void DapiTCPWriteWord(uint handle, uint address, uint value)
	{
		int tx_pos = 0;
		uint delib_error_code;

		tx_buff[tx_pos++] = (byte)'W';							// COMMAND
		tx_buff[tx_pos++] = (byte)'W';							// WIDTH

		tx_buff[tx_pos++] = (byte)((address >> 8) & 0xff);		// ADDRESS_BIT_8_15
		tx_buff[tx_pos++] = (byte)((address >> 0) & 0xff);		// ADDRESS_BIT_0_7

		tx_buff[tx_pos++] = (byte)((value >> 8) & 0xff);		// DATA_BIT_8_15
		tx_buff[tx_pos++] = (byte)((value >> 0) & 0xff);		// DATA_BIT_0_7

		if ((delib_error_code = DT.TCP.DapiTCPSendCommand(handle, tx_buff, tx_pos, null, 0)) != DT.Error.DAPI_ERR_NONE)
		{

		}
	}

	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------

	public static void DapiTCPWriteLong (uint handle, uint address, uint value)
	{
		int tx_pos = 0;
		uint delib_error_code;

		tx_buff[tx_pos++] = (byte)'W';							// COMMAND
		tx_buff[tx_pos++] = (byte)'L';							// WIDTH

		tx_buff[tx_pos++] = (byte)((address >> 8) & 0xff);		// ADDRESS_BIT_8_15
		tx_buff[tx_pos++] = (byte)((address >> 0) & 0xff);		// ADDRESS_BIT_0_7

		tx_buff[tx_pos++] = (byte)((value >> 24) & 0xff);		// DATA_BIT_24_31
		tx_buff[tx_pos++] = (byte)((value >> 16) & 0xff);		// DATA_BIT_16_23
		tx_buff[tx_pos++] = (byte)((value >> 8) & 0xff);		// DATA_BIT_8_15
		tx_buff[tx_pos++] = (byte)((value >> 0) & 0xff);		// DATA_BIT_0_7

		if ((delib_error_code = DT.TCP.DapiTCPSendCommand(handle, tx_buff, tx_pos, null, 0)) != DT.Error.DAPI_ERR_NONE)
		{

		}
	}

	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------

	public static void DapiTCPWriteLongLong (uint handle, uint address, ulong value)
	{
		int tx_pos = 0;
		uint delib_error_code;

		tx_buff[tx_pos++] = (byte)'W';							// COMMAND
		tx_buff[tx_pos++] = (byte)'X';							// WIDTH

		tx_buff[tx_pos++] = (byte)((address >> 8) & 0xff);		// ADDRESS_BIT_8_15
		tx_buff[tx_pos++] = (byte)((address >> 0) & 0xff);		// ADDRESS_BIT_0_7

		tx_buff[tx_pos++] = (byte)((value >> 56) & 0xff);		// DATA_BIT_56_63
		tx_buff[tx_pos++] = (byte)((value >> 48) & 0xff);		// DATA_BIT_48_55
		tx_buff[tx_pos++] = (byte)((value >> 40) & 0xff);		// DATA_BIT_40_47
		tx_buff[tx_pos++] = (byte)((value >> 32) & 0xff);		// DATA_BIT_32_39
		tx_buff[tx_pos++] = (byte)((value >> 24) & 0xff);		// DATA_BIT_24_31
		tx_buff[tx_pos++] = (byte)((value >> 16) & 0xff);		// DATA_BIT_16_23
		tx_buff[tx_pos++] = (byte)((value >> 8) & 0xff);		    // DATA_BIT_8_15
		tx_buff[tx_pos++] = (byte)((value >> 0) & 0xff);		    // DATA_BIT_0_7

		if ((delib_error_code = DT.TCP.DapiTCPSendCommand(handle, tx_buff, tx_pos, null, 0)) != DT.Error.DAPI_ERR_NONE)
		{

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

    public static uint DapiTCPReadByte(uint handle, uint address)
	{
		uint delib_error_code;
		int tx_pos = 0;
		int rx_pos = 0;
		uint data = 0;

		tx_buff[tx_pos++] = (byte)'R';							// COMMAND
		tx_buff[tx_pos++] = (byte)'B';							// WIDTH

		tx_buff[tx_pos++] = (byte)((address >> 8) & 0xff);		// ADDRESS_BIT_8_15
		tx_buff[tx_pos++] = (byte)((address >> 0) & 0xff);		// ADDRESS_BIT_0_7

		if ((delib_error_code = DT.TCP.DapiTCPSendCommand(handle, tx_buff, tx_pos, rx_buff, 1)) != DT.Error.DAPI_ERR_NONE)
		{
			
		}
		else
		{
			data = (uint)((rx_buff[rx_pos++] << 0) & 0xff);
		}

		return data;
	}

	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------

    public static uint DapiTCPReadWord(uint handle, uint address)
	{
		uint delib_error_code;
		int tx_pos = 0;
		int rx_pos = 0;
		uint data = 0;

		tx_buff[tx_pos++] = (byte)'R';							// COMMAND
		tx_buff[tx_pos++] = (byte)'W';							// WIDTH

		tx_buff[tx_pos++] = (byte)((address >> 8) & 0xff);		// ADDRESS_BIT_8_15
		tx_buff[tx_pos++] = (byte)((address >> 0) & 0xff);		// ADDRESS_BIT_0_7

		if ((delib_error_code = DT.TCP.DapiTCPSendCommand(handle, tx_buff, tx_pos, rx_buff, 2)) != DT.Error.DAPI_ERR_NONE)
		{

		}
		else
		{
			data  = (uint)((rx_buff[rx_pos++] << 0) & 0xff);
			data |= (uint)((rx_buff[rx_pos++] << 8) & 0xff00);
		}

		return data;
	}

	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------

    public static uint DapiTCPReadLong (uint handle, uint address)
	{
		uint delib_error_code;
		int tx_pos = 0;
		int rx_pos = 0;
		uint data = 0;

		tx_buff[tx_pos++] = (byte)'R';							// COMMAND
		tx_buff[tx_pos++] = (byte)'L';							// WIDTH

		tx_buff[tx_pos++] = (byte)((address >> 8) & 0xff);		// ADDRESS_BIT_8_15
		tx_buff[tx_pos++] = (byte)((address >> 0) & 0xff);		// ADDRESS_BIT_0_7

		if ((delib_error_code = DT.TCP.DapiTCPSendCommand(handle, tx_buff, tx_pos, rx_buff, 4)) != DT.Error.DAPI_ERR_NONE)
		{

		}
		else
		{
			data  = (uint)((rx_buff[rx_pos++] << 0) & 0xff);
			data |= (uint)((rx_buff[rx_pos++] << 8) & 0xff00);
			data |= (uint)((rx_buff[rx_pos++] << 16) & 0xff0000L);
			data |= (uint)((rx_buff[rx_pos++] << 24) & 0xff000000L);
		}

		return data;
	}

	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------

    public static ulong DapiTCPReadLongLong (uint handle, uint address)
	{
		uint delib_error_code;
		int tx_pos = 0;
		int rx_pos = 0;
		ulong data = 0;

		tx_buff[tx_pos++] = (byte)'R';							// COMMAND
		tx_buff[tx_pos++] = (byte)'X';							// WIDTH

		tx_buff[tx_pos++] = (byte)((address >> 8) & 0xff);		// ADDRESS_BIT_8_15
		tx_buff[tx_pos++] = (byte)((address >> 0) & 0xff);		// ADDRESS_BIT_0_7

		if ((delib_error_code = DT.TCP.DapiTCPSendCommand(handle, tx_buff, tx_pos, rx_buff, 8)) != DT.Error.DAPI_ERR_NONE)
		{

		}
		else
		{
			data =  ((ulong) (rx_buff[rx_pos++] << 0) & 0xffUL);
			data |= ((ulong) (rx_buff[rx_pos++] << 8) & 0xff00UL);
			data |= ((ulong) (rx_buff[rx_pos++] << 16) & 0xff0000UL);
			data |= ((ulong) (rx_buff[rx_pos++] << 24) & 0xff000000UL);
			data |= ((ulong) (rx_buff[rx_pos++] << 32) & 0xff00000000UL);
			data |= ((ulong) (rx_buff[rx_pos++] << 40) & 0xff0000000000UL);
			data |= ((ulong) (rx_buff[rx_pos++] << 48) & 0xff0000000000UL);
			data |= ((ulong) (rx_buff[rx_pos++] << 56) & 0xff000000000000UL);
		}

		return data;
	}

	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------


    public static uint DapiTCPReadMultipleBytes(uint handle, uint address, uint address_depth, uint repeat, Byte[] buff, uint buff_len)
    {
        byte[] buffer;
        uint buffer_length;

        uint i;
        uint j;

        uint pos = 0;
        uint tx_cnt = 0;

        uint m_start_id;
        uint m_data_id;
        uint m_data_pos;

        uint delib_error_code;

        //DapiHandle dapiHandle = (DapiHandle) ((GCHandle)((IntPtr) handle)).Target;
        var delibService = DependencyService.Get<IOControl.IDelibUtils>();
        DapiHandle dapiHandle = delibService.GetDapiHandle(handle);

        //if ((dapiHandle = (DapiHandle)((GCHandle)((IntPtr)handle)).Target) == null)
        if (dapiHandle == null)
        {
            DT.Log("invalid handle");
            return DT.Error.DAPI_ERR_COM_HANDLE_INVALID;
        }
        if (dapiHandle.TCP_IO == null)
        {
            DT.Log("invalid socket");
            return DT.Error.DAPI_ERR_GEN_SOCKET_ERROR;
        }




        // speicherallokierung für buffer + check 
        buffer_length = (((address_depth + 3) * repeat) + 1) + 100;                     //sizeof(buffer);
        buffer = new Byte[buffer_length];                               // +1 weil "sizeof" nicht bei 0 sondern bei 1 anfängt



        tx_buff[tx_cnt++]=(byte)'R';                                      // COMMAND //Read
        tx_buff[tx_cnt++]=(byte)'M';                                      // WIDTH //Multiple

        // address (2byte)
        tx_buff[tx_cnt++]=(byte)((address >> 8 ) & 0xff);                 // ADDRESS_BIT_8_15
        tx_buff[tx_cnt++]=(byte)((address >> 0 ) & 0xff);                 // ADDRESS_BIT_0_7

        // address_depth (2byte)
        tx_buff[tx_cnt++]=(byte)((address_depth >> 8 ) & 0xff);           // ADDRESS_DEPTH_BIT_8_15
        tx_buff[tx_cnt++]=(byte)((address_depth >> 0 ) & 0xff);           // ADDRESS_DEPTH_BIT_0_7

        // repeat (2byte)
        tx_buff[tx_cnt++]=(byte)((repeat >> 8 ) & 0xff);                  // repeat_BIT_8_15
        tx_buff[tx_cnt++]=(byte)((repeat >> 0 ) & 0xff);                  // repeat_BIT_0_7


        delib_error_code = DT.TCP.DapiTCPSendCommand(handle, tx_buff, (int) tx_cnt, buffer, (int) buffer_length);

        //DT.DBG.ByteDump(buffer, 0, buffer_length);



        // buffer überprüfen
        // datenblock = start_id_multiple_byte(1) +  data_id(2) + addr_depth    

        for (i=0; i<repeat; i++)
        {
            pos = 0;
            m_data_pos = i * (address_depth + 3); //+ TCP_RO_BIG_RX_HEADER_LENGTH;

            // start_id + data_id aus dem buffer filtern
            m_start_id = Convert.ToUInt32(buffer [m_data_pos + (pos++)]);
            m_data_id = Convert.ToUInt32(buffer[m_data_pos + (pos++)]);
            m_data_id = Convert.ToUInt32((m_data_id << 8) | (buffer[m_data_pos + (pos++)]));


            // start_id checken
            if (m_start_id != DT.DEDITEC_TCP_START_ID_FOR_MULTIPLE_BYTE_DATA)
            {
                DT.Log("m_start_id != DEDITEC_TCP_START_ID_FOR_MULTIPLE_BYTE_DATA");
                return DT.RETURN_ERROR;         // Error
            }

            // data_id checken
            if (m_data_id != i)
            {
                DT.Log("m_data_id != i");
                return DT.RETURN_ERROR;         // Error
            }
        }

        // daten sind ok - daten in "richtigen rx_buffer" schreiben
        tx_cnt = 0;

        for (i=0; i< repeat; i++)
        {
            for (j=0; j<address_depth; j++)
            {
                buff[tx_cnt++] = buffer[3 + j + i*(address_depth+3)];
            }
        }

        // zum abschluss geschrieben zeichen in "richtigen buffer" mit der errechneten (und benötigten größe vergleichen)
        if (tx_cnt != (repeat * address_depth))
        {
            DT.Log("tx_cnt != (repeat * address_depth)");
            return DT.RETURN_ERROR;         // Error
        }

        // ok
        return DT.RETURN_OK;
    }

}
