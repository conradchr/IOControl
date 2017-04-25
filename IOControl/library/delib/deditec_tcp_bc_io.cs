

#define USE_UDP_SERVICE

using System;

using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using System.Threading;

using IOControl;

#if (USE_UDP_SERVICE)
using Xamarin.Forms;    // DependencyService
#else
    using Java.Net;
#endif




public class DT_Mobile_BCFunctions
{
	
	public const uint BYTES_DEV_ANSWER_GC								= 6;
	public const uint BYTES_DEV_ANSWER_GC_DEV_CFG						= 256;

	public const int DEDITEC_BC_PORT 									= 9912;
	public const String DEDITEC_BC_ADDR 								= "255.255.255.255";

	public const uint DEDITEC_BC_PACKET_TX_ID_0 						= 0x12;
	public const uint DEDITEC_BC_PACKET_TX_ID_1 						= 0x47;
	public const uint DEDITEC_BC_PACKET_TX_ID_2 						= 0x22;
	public const uint DEDITEC_BC_PACKET_TX_ID_3 						= 0x01;

	public const uint DEDITEC_BC_PACKET_RX_ID_0 						= 0x13;
	public const uint DEDITEC_BC_PACKET_RX_ID_1 						= 0x47;
	public const uint DEDITEC_BC_PACKET_RX_ID_2 						= 0x22;
	public const uint DEDITEC_BC_PACKET_RX_ID_3 						= 0x01;

	public const uint DEDITEC_BC_PACKET_CMD_GLOBAL_CALL					= 0x01;
	public const uint DEDITEC_BC_PACKET_CMD_PARAM_GET					= 0x02;
	public const uint DEDITEC_BC_PACKET_CMD_PARAM_SET					= 0x03;
	public const uint DEDITEC_BC_PACKET_CMD_ETH0_CONFIGURE				= 0x04;
	public const uint DEDITEC_BC_PACKET_CMD_GLOBAL_CALL_WITH_DEV_CFG	= 0x05;

	public const uint DEDITEC_BC_PACKET_PARAM_BOARD_NAME				= 0x01;
	public const uint DEDITEC_BC_PACKET_PARAM_IP_ADDR					= 0x02;
	public const uint DEDITEC_BC_PACKET_PARAM_NETMASK					= 0x03;
	public const uint DEDITEC_BC_PACKET_PARAM_STDGATEWAY				= 0x04;
	public const uint DEDITEC_BC_PACKET_PARAM_DNS1						= 0x05;
	public const uint DEDITEC_BC_PACKET_PARAM_DHCP						= 0x06;
	public const uint DEDITEC_BC_PACKET_PARAM_DELIB_MODULE_ID			= 0x07;
	public const uint DEDITEC_BC_PACKET_PARAM_MAC_ADDR					= 0x08;
	public const uint DEDITEC_BC_PACKET_PARAM_INFO_STRING				= 0x09;
	public const uint DEDITEC_BC_PACKET_PARAM_TCP_CONFIG_USED			= 0x0a;
	public const uint DEDITEC_BC_PACKET_PARAM_KEY_HANDLER_LOG			= 0x0b;
	public const uint DEDITEC_BC_PACKET_PARAM_IDENTIFY					= 0x0c;

	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------

	public static uint ArrayCompare(Byte[] array_a, uint index_a, Byte[] array_b, uint index_b, uint length)
	{
		uint i;
		uint cnt_not_equal = 0;

		for (i=0; i!=length; i++)
		{
			if (array_a [index_a + i] != array_b [index_b + i]) 
			{
				cnt_not_equal++;
			}
		}

		return cnt_not_equal;
	}

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

	public static uint BroadcastTxRx(uint cmd, uint parameter, Byte[] mac, Byte[] tx_buffer, ref Byte[] rx_buffer)
	{
		uint pos;
		Byte[] buff_tx = new Byte[1024];
        Byte[] buff_rx = new Byte[1024];

        bool data_present;

		uint bytes_dev_answer;
		uint bytes_to_compare;

		uint i;
		bool exist;
		uint rx_buffer_multipos = 0;
		uint bytes_header;
		uint exist_cnt = 0;
		uint ret = 0;

        int bytes_received;

#if (USE_UDP_SERVICE)
        var udpService = DependencyService.Get<IOControl.IUDPUtils>();
#else
        DatagramPacket packet;
		DatagramSocket socket;
#endif



        // --------------------------------
        // open socket

#if (USE_UDP_SERVICE)
        udpService.CreateSocket(DEDITEC_BC_PORT, 1000);
#else
        socket = new DatagramSocket (DEDITEC_BC_PORT);
		socket.Broadcast = true;
		socket.SoTimeout = 1000;
#endif

		pos = 0;
		buff_tx[pos++] = (byte) DEDITEC_BC_PACKET_TX_ID_0;
		buff_tx[pos++] = (byte) DEDITEC_BC_PACKET_TX_ID_1;
		buff_tx[pos++] = (byte) DEDITEC_BC_PACKET_TX_ID_2;
		buff_tx[pos++] = (byte) DEDITEC_BC_PACKET_TX_ID_3;
		buff_tx[pos++] = (byte) cmd;
		buff_tx[pos++] = (byte) parameter;

		if ((cmd != DEDITEC_BC_PACKET_CMD_GLOBAL_CALL) && (cmd != DEDITEC_BC_PACKET_CMD_GLOBAL_CALL_WITH_DEV_CFG)) 
		{
			// Es ist kein Global Call -> Mac-Addr mit anhängen
			Array.Copy (mac, 0, buff_tx, (int)pos, mac.Length);
			pos+=6;
		}

		try
		{
		    if (tx_buffer != null) 
		    {
			    buff_tx [pos++] = (byte)tx_buffer.Length;
			    Array.Copy (tx_buffer, 0, buff_tx, (int)pos, tx_buffer.Length);
			    pos += (uint)tx_buffer.Length;
		    } 
		    else 
		    {
			    buff_tx [pos++] = 0;
		    }
		} catch (Exception/* e*/) {
			//Sess.Log(e.ToString());
		}



        // --------------------------------
        // send

        // 255.255.255.255
        Sess.Log("Sending BC to " + DEDITEC_BC_ADDR);
#if (USE_UDP_SERVICE)
        udpService.Send(DEDITEC_BC_ADDR, DEDITEC_BC_PORT, buff_tx, (int)pos);
#else
        packet = new DatagramPacket (buff_tx, (int) pos, InetAddress.GetByName(DEDITEC_BC_ADDR), DEDITEC_BC_PORT);
        socket.Send (packet);
#endif

        // xxx.xxx.xxx.255

        //String nw_bc_addr = String.Format("{0}.{1}.{2}.255", (Sess.deviceIP & 0xff), (Sess.deviceIP >> 8 & 0xff), (Sess.deviceIP >> 16 & 0xff));
        String nw_bc_addr = "192.168.1.255";
        Sess.Log("Sending BC to " + nw_bc_addr);
#if (USE_UDP_SERVICE)
        udpService.Send(nw_bc_addr, DEDITEC_BC_PORT, buff_tx, (int)pos);
#else
        packet = new DatagramPacket (buff_tx, (int) pos, InetAddress.GetByName(nw_bc_addr), DEDITEC_BC_PORT);   
		socket.Send (packet);
#endif


        // --------------------------------
        // receive

        int durchlauf = 0;

		do 
		{

#if (USE_UDP_SERVICE)
            if ((data_present = udpService.DataPresent()) == true)
            {

                bytes_received = udpService.Receive(ref buff_rx);
#else
            packet = new DatagramPacket (buff_rx, buff_rx.Length);
			try 
			{
				data_present = true;
				socket.Receive(packet);
                Sess.Log("#" + durchlauf.ToString() + " UDP: " + packet.Length);
				//Log.Info ("DBG", "#" + durchlauf.ToString() + " UDP: " + packet.Length);

            }
			catch (Exception) 
			{
				// wenn wir hier landen, ist nichts mehr da ...
				//data_present = true;
				data_present = false;
                Sess.Log("#" + durchlauf.ToString() + " UDP: nichts mehr da!");
            }

			if (data_present == true)
			{
                buff_rx = packet.GetData();
                bytes_received = packet.Length;
#endif

                if ((buff_rx[0] == DEDITEC_BC_PACKET_RX_ID_0) &&
					(buff_rx[1] == DEDITEC_BC_PACKET_RX_ID_1) &&
					(buff_rx[2] == DEDITEC_BC_PACKET_RX_ID_2) &&
					(buff_rx[3] == DEDITEC_BC_PACKET_RX_ID_3) &&
					(buff_rx[4] == cmd))
				{
					// antwortpaket auf unsere anfrage

					if ((cmd == DEDITEC_BC_PACKET_CMD_GLOBAL_CALL) || (cmd == DEDITEC_BC_PACKET_CMD_GLOBAL_CALL_WITH_DEV_CFG)) 
					{
						// global call

						bytes_header = 5;

						if (cmd == DEDITEC_BC_PACKET_CMD_GLOBAL_CALL)
						{
							// DEDITEC_BC_PACKET_CMD_GLOBAL_CALL
							bytes_dev_answer = BYTES_DEV_ANSWER_GC;
							bytes_to_compare = BYTES_DEV_ANSWER_GC;
						}
						else
						{
							// DEDITEC_BC_PACKET_CMD_GLOBAL_CALL_WITH_DEV_CFG
							bytes_dev_answer = BYTES_DEV_ANSWER_GC_DEV_CFG - 5;
							bytes_to_compare = BYTES_DEV_ANSWER_GC + 5;	// network(1) + ip(4)
						}

						if ((bytes_received-bytes_header) == bytes_dev_answer)
						{
							// Neue MAC-Adresse empfangen !

							exist=false;
							if(rx_buffer_multipos != 0)
							{
								// Es wurden schon MAC-Adressen empfangen...checke, ob schon die empfangene eingetragen wurde
								for(i=0;i!=(rx_buffer_multipos/bytes_dev_answer);++i)
								{
									exist_cnt=0;
									for(pos=0;pos!=bytes_to_compare;++pos)			
									{
										if (rx_buffer[i*bytes_dev_answer+pos] == buff_rx[bytes_header+pos])
										{
											++exist_cnt;
										}
									}
									if(exist_cnt == bytes_to_compare)
									{
										// Alle 6 Bytes der MAC-Adresse sind identisch
										exist=true;
									}
								}
							}
							// Check ist zu ende, ob schon existiert !

							if (!exist)
							{
								for(pos=0;pos!=bytes_dev_answer;++pos)			
								{
									if((rx_buffer_multipos < rx_buffer.Length) && ((bytes_header+pos) < buff_rx.Length))
									{
										rx_buffer[rx_buffer_multipos] = buff_rx[bytes_header+pos];
									}
									++rx_buffer_multipos;
								}
								if(rx_buffer_multipos < rx_buffer.Length)
								{
									rx_buffer[rx_buffer_multipos] = 0x00;	// Terminate
								}

								ret = rx_buffer_multipos;
							}
						} // if ((packet.Length-bytes_header) == bytes_dev_answer)
					}
					else	// if ((broadcast_command == DEDITEC_BC_PACKET_CMD_GLOBAL_CALL) || (broadcast_command == DEDITEC_BC_PACKET_CMD_GLOBAL_CALL_WITH_DEV_CFG))
					{
						// other Commands (MAC-Adresse checken)
						if (ArrayCompare(buff_rx, 5, mac, 0, 6) == 0)
						{
							bytes_header = 11;			// // 5 Bytes Header + 6 Byte MAC-Adresse abziehen

							for(pos=0;pos!=(bytes_received - bytes_header);++pos)		
							{
								if((pos < rx_buffer.Length) && ((bytes_header+pos) < buff_rx.Length))
								{
									rx_buffer[pos] = buff_rx[bytes_header+pos];
								}
							}

							if(pos < rx_buffer.Length)
							{
								rx_buffer[pos] = 0x00;	// Terminate
							}
							ret = pos;		// Rückgabelänge
						}
						else
						{
							bytes_header = 5;			// 6 Bytes Header abziehen

							for(pos=0; pos!=6; pos++)		
							{
								rx_buffer[pos] = buff_rx[bytes_header+pos];
							}
							ret = 0;
						}
					}	// if ((broadcast_command == DEDITEC_BC_PACKET_CMD_GLOBAL_CALL) || (broadcast_command == DEDITEC_BC_PACKET_CMD_GLOBAL_CALL_WITH_DEV_CFG))
				} // "if IDs stimmen"
			} // if (data_present == true)
            
			durchlauf++;
		
		} while (data_present == true);

#if (USE_UDP_SERVICE)
        udpService.CloseSocket();
#else
        socket.Close ();
#endif


        return ret;
	}

	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------

	public static uint GetParameter(Byte[] mac_addr_orig, uint parameter_nr, Byte[] rx_buffer)
	{
		uint ret;
		uint error_code;
		Byte[] buff_rx = new byte[1000];
		int i;
		Byte[] mac_addr = new Byte[6];
		
		error_code = DT.Error.DAPI_ERR_NONE;

		String xx = DT.Conv.ConvertByteArrayToString(mac_addr_orig, 12);

		for(i=0;i!=6;++i) 
		{
			mac_addr[i] = (Byte) Convert.ToUInt32(xx.Substring(i*2, 2), 16);
		}

		ret = BroadcastTxRx(DEDITEC_BC_PACKET_CMD_PARAM_GET, parameter_nr, mac_addr, null, ref buff_rx);

		if(ret==0)
		{
			// Nichts empfangen
			error_code = DT.Error.DAPI_ERR_COM_DEVICE_DID_NOT_ANSWER;
			//rx_buffer_length = 0;
		}
		else
		{
			if((ret > buff_rx.Length) || (ret > rx_buffer.Length))
			{
				error_code = DT.Error.DAPI_ERR_GEN_BUFFER_TOO_SMALL_ERROR;
				//rx_buffer_length = 0;
			}
			else
			{
				for(i=2;i<ret;++i)
				{
					rx_buffer[i-2] = buff_rx[i];
				}
				rx_buffer[i-2] = 0;				//Terminate string

				error_code = buff_rx[0];			// Error Code
				if(error_code != 0)
				{
					error_code = DT.Error.DAPI_ERR_DEV_CLASS | (error_code&0xff); // BC Protocol does only support 8 Bit codes
				}

				/*
				if(rx_buffer_length != 0)
				{
					rx_buffer_length=buff_rx[1];	// Anzahl der empfangenen Zeichen 		
				}
				*/
			}
		}

		return error_code;
	}

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint SetParameter(Byte[] mac_addr_orig, uint parameter_nr, Byte[] tx_buffer)
    {
        uint ret;
        uint error_code;
        Byte[] buff_rx = new byte[1000];
        int i;
        Byte[] mac_addr = new Byte[6];

        String xx = DT.Conv.ConvertByteArrayToString(mac_addr_orig, 12);

        for (i = 0; i != 6; ++i)
        {
            mac_addr[i] = (Byte)Convert.ToUInt32(xx.Substring(i * 2, 2), 16);
        }

        ret = BroadcastTxRx(DEDITEC_BC_PACKET_CMD_PARAM_SET, parameter_nr, mac_addr, tx_buffer, ref buff_rx);

        if (ret != 1)
        {
            // Nichts empfangen
            error_code = DT.Error.DAPI_ERR_COM_DEVICE_DID_NOT_ANSWER;
        }
        // Es muss ein Zeichen empfangen werden -> der Return Code vom Parameter Set !
        
        error_code = buff_rx[0];			// Error Code
	    if(error_code != 0)
	    {
		    error_code = DT.Error.DAPI_ERR_DEV_CLASS | (error_code&0xff); // BC Protocol does only support 8 Bit codes
	    }

	    return error_code;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

	public static uint GetMacList(ref Byte[] rx_buffer)
	{
		uint ret;
		uint no_of_mac_adresses;
        uint i;
		Byte[] buff_rx = new Byte[1000];

		ret = BroadcastTxRx(DEDITEC_BC_PACKET_CMD_GLOBAL_CALL, 0x0, null, null, ref buff_rx);

		no_of_mac_adresses = ret/6;

		for(i=0; i!= no_of_mac_adresses; ++i)
		{
			String xx = DT.Conv.ByteArrayToHexString((Byte[]) DT.Conv.SplitArray(buff_rx, i*6, 6));
			Array.Copy (DT.Conv.ConvertStringToByteArray (xx), 0, rx_buffer, (int)i * 12, 12);
		}

		return no_of_mac_adresses;
	}

	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------

	public static uint GetMacListDevCfg(ref Byte[] rx_buffer)
	{
		uint ret;
		uint no_of_mac_adresses;
        
        ret = BroadcastTxRx(DEDITEC_BC_PACKET_CMD_GLOBAL_CALL_WITH_DEV_CFG, 0x0, null, null, ref rx_buffer);

        no_of_mac_adresses = ret/251;

		return no_of_mac_adresses;
	}

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint Eth0Config(Byte[] mac_addr_orig)
    {
        uint ret;
        uint error_code;
        Byte[] buff_rx = new byte[1000];
        int i;
        Byte[] mac_addr = new Byte[6];

        String xx = DT.Conv.ConvertByteArrayToString(mac_addr_orig, 12);

        for (i = 0; i != 6; ++i)
        {
            mac_addr[i] = (Byte)Convert.ToUInt32(xx.Substring(i * 2, 2), 16);
        }

        ret = BroadcastTxRx(DEDITEC_BC_PACKET_CMD_ETH0_CONFIGURE, 0, mac_addr, null, ref buff_rx);

        if (ret != 1)
        {
            // Nichts empfangen
            error_code = DT.Error.DAPI_ERR_COM_DEVICE_DID_NOT_ANSWER;
        }
        // Es muss ein Zeichen empfangen werden -> der Return Code vom Parameter Set !

        error_code = buff_rx[0];            // Error Code
        if (error_code != 0)
        {
            error_code = DT.Error.DAPI_ERR_DEV_CLASS | (error_code & 0xff); // BC Protocol does only support 8 Bit codes
        }

        return error_code;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
}

