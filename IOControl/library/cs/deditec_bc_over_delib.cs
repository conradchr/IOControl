using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;

#if (!__MOBILE__)
using System.Data;
using System.Windows.Forms;
#endif

public class DT_BCFunctions
{    
    public class Parameter
    {
        public const uint DEDITEC_BC_PACKET_PARAM_BOARD_NAME        = 0x01;
        public const uint DEDITEC_BC_PACKET_PARAM_IP_ADDR           = 0x02;
        public const uint DEDITEC_BC_PACKET_PARAM_NETMASK           = 0x03;
        public const uint DEDITEC_BC_PACKET_PARAM_STDGATEWAY        = 0x04;
        public const uint DEDITEC_BC_PACKET_PARAM_DNS1              = 0x05;
        public const uint DEDITEC_BC_PACKET_PARAM_DHCP              = 0x06;
        public const uint DEDITEC_BC_PACKET_PARAM_DELIB_MODULE_ID   = 0x07;
        public const uint DEDITEC_BC_PACKET_PARAM_MAC_ADDR          = 0x08;
        public const uint DEDITEC_BC_PACKET_PARAM_INFO_STRING       = 0x09;
        public const uint DEDITEC_BC_PACKET_PARAM_TCP_CONFIG_USED   = 0x0a;
        public const uint DEDITEC_BC_PACKET_PARAM_KEY_HANDLER_LOG   = 0x0b;
        public const uint DEDITEC_BC_PACKET_PARAM_IDENTIFY          = 0x0c;
    }

    public const uint PACKET_NOT_VALID = 0x00;
    public const uint PACKET_VALID = 0x01;

    public const uint IS_BL = 0x01;
    public const uint IS_FW = 0x02;

    public const uint MAX_BYTES_DEV_ANSWER_GC                           = 12;           // mac(12)
    public const uint MAX_BYTES_DEV_ANSWER_GC_DEV_CFG                   = 251;	        // mac(6) + max. dev_cfg(245)
    
    public const String ETH_DEV_DEDITEC_MAC_START = "00C0D5";
    public const String ETH_DEV_DEFAULT_MAC = ETH_DEV_DEDITEC_MAC_START + "FFFFFF";

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint gc_dev_cfg(List<ETHDeviceConfig> eth_devices)
    {
        Byte[] mac_and_dev_cfg_buffer = new Byte[MAX_BYTES_DEV_ANSWER_GC_DEV_CFG * 30]; // maximal 30 module
        uint no_of_modules;
        uint i;
        ETHDeviceConfig eth_dev;
        #if (!__MOBILE__)
            uint dummy_uint = 0;
        	no_of_modules = DT.Ext.DapiInternCommand(DT.Ext.DAPI_INTERN_CMD_BC_GET_MAC_LIST_WITH_DEV_CFG, "", "", mac_and_dev_cfg_buffer, (uint)mac_and_dev_cfg_buffer.Length, ref dummy_uint);
        #else
            no_of_modules = DT.Mobile.BC.GetMacListDevCfg(ref mac_and_dev_cfg_buffer);
		#endif
        if (no_of_modules > 0)
        {
            for (i = 0; i != no_of_modules; i++)
            {
                eth_dev = new ETHDeviceConfig(
                    (Byte[])DT.Conv.SplitArray(mac_and_dev_cfg_buffer, i * MAX_BYTES_DEV_ANSWER_GC_DEV_CFG, 6),        // 6 Bytes MAC-Adresse
                    (Byte[])DT.Conv.SplitArray(mac_and_dev_cfg_buffer, i * MAX_BYTES_DEV_ANSWER_GC_DEV_CFG + 6, MAX_BYTES_DEV_ANSWER_GC_DEV_CFG - 6)  // Restlichen Bytes AB pos. MAC-Adresse
                );

                eth_devices.Add(eth_dev);
            }
        }

        return no_of_modules;
    }


    public static uint gc(List<ETHDeviceConfig> eth_devices)
    {
        Byte[] mac_buffer = new Byte[MAX_BYTES_DEV_ANSWER_GC * 30]; // maximal 30 module
        uint no_of_modules;
        uint i;
        int j;
        ETHDeviceConfig eth_dev;
        bool already_exist;
        uint new_modules = 0;
        int modules_found_by_gc_dev_cfg = eth_devices.Count;
        uint delib_error_code;

        uint debug_modules_filtered = 0;
		#if (!__MOBILE__)
            uint dummy_uint = 0;
        	no_of_modules = DT.Ext.DapiInternCommand(DT.Ext.DAPI_INTERN_CMD_BC_GET_MAC_LIST, "", "", mac_buffer, (uint)mac_buffer.Length, ref dummy_uint);
		#else
			no_of_modules = DT.Mobile.BC.GetMacList(ref mac_buffer);
		#endif
        if (no_of_modules > 0)
        {
            for (i = 0; i != no_of_modules; i++)
            {
                eth_dev = new ETHDeviceConfig(
                    	(Byte[])DT.Conv.SplitArray(mac_buffer, i * MAX_BYTES_DEV_ANSWER_GC, MAX_BYTES_DEV_ANSWER_GC)
                );

                // check ob mac schon drin ist
                already_exist = false;
                for (j = 0; j != modules_found_by_gc_dev_cfg; j++)
                {
                    if (eth_devices[j].Network.mac != ETH_DEV_DEFAULT_MAC)
                    {
                        if (eth_devices[j].Network.mac == eth_dev.Network.mac)
                        {
                            already_exist = true;
                            debug_modules_filtered++;
                        }
                    }
                }

                if (already_exist == false)
                {
                    // mac ist noch nicht vorhanden oder ist "FFFFFF"

                    // versuchen dev_cfg über BC als EIN buffer zu holen
                    if (DT.Bc.deditec_bc_get_byte_parameter(eth_dev.Network.mac, DT.Bc.Parameter.DEDITEC_BC_PACKET_PARAM_TCP_CONFIG_USED, ref eth_dev.dev_cfg_buffer) == DT.Error.DAPI_ERR_NONE)
                    {
                        // dev_cfg buffer wird vom modul unterstützt
                        eth_dev.suppport_bc_dev_cfg = true;

                        // buffer in struktur
                        eth_dev.SetDeviceConfig(eth_dev.dev_cfg_buffer);

                        // bei RO-ETH-FW 1.75 ist wohl ein fehler beim buffer bauen drin - hier kommt immer das gateway "0.0.0.0"
                        // daher diese abrfrage - falls das modul die fehlerhafte FW hat, wird hier das gateway erneut geladen
                        if ((eth_dev.dev_cfg_buffer[8] == 0) &&
                            (eth_dev.dev_cfg_buffer[9] == 0) &&
                            (eth_dev.dev_cfg_buffer[10] == 0) &&
                            (eth_dev.dev_cfg_buffer[11] == 0))
                        {
                            DT.Bc.deditec_bc_get_string_parameter(eth_dev.Network.mac, DT.Bc.Parameter.DEDITEC_BC_PACKET_PARAM_STDGATEWAY, ref eth_dev.Network.gateway);
                        }
                    }
                    else
                    {
                        // dev_cfg buffer wird vom modul NICHT unterstützt
                        // -> Paramter einzeln holen
                        if ((delib_error_code = DT.Bc.deditec_bc_get_string_parameter(eth_dev.Network.mac, DT.Bc.Parameter.DEDITEC_BC_PACKET_PARAM_IP_ADDR, ref eth_dev.Network.ip)) != DT.Error.DAPI_ERR_NONE)
                        {
                            //ErrorMessageHandler(delib_error_code, prog_action, config, "IP-Addresse");
                            //return DT.RETURN_ERROR;
                        }

                        if ((delib_error_code = DT.Bc.deditec_bc_get_string_parameter(eth_dev.Network.mac, DT.Bc.Parameter.DEDITEC_BC_PACKET_PARAM_NETMASK, ref eth_dev.Network.netmask)) != DT.Error.DAPI_ERR_NONE)
                        {
                            //ErrorMessageHandler(delib_error_code, prog_action, config, "Netmask");
                            //return DT.RETURN_ERROR;
                        }

                        if ((delib_error_code = DT.Bc.deditec_bc_get_string_parameter(eth_dev.Network.mac, DT.Bc.Parameter.DEDITEC_BC_PACKET_PARAM_STDGATEWAY, ref eth_dev.Network.gateway)) != DT.Error.DAPI_ERR_NONE)
                        {
                            //ErrorMessageHandler(delib_error_code, prog_action, config, "Gateway");
                            //return DT.RETURN_ERROR;
                        }
                    }
                    if ((delib_error_code = DT.Bc.deditec_bc_get_string_parameter(eth_dev.Network.mac, DT.Bc.Parameter.DEDITEC_BC_PACKET_PARAM_BOARD_NAME, ref eth_dev.BoardName.boardname)) != DT.Error.DAPI_ERR_NONE)
                    {
                        //ErrorMessageHandler(delib_error_code, prog_action, config, "Board-Name");
                        //return DT.RETURN_ERROR;
                    }
                    if ((delib_error_code = DT.Bc.deditec_bc_get_numeric_parameter(eth_dev.Network.mac, DT.Bc.Parameter.DEDITEC_BC_PACKET_PARAM_DELIB_MODULE_ID, ref eth_dev.BLFWInfo.delib_module_id)) != DT.Error.DAPI_ERR_NONE)
                    {
                        //ErrorMessageHandler(delib_error_code, prog_action, config, "Board-Name");
                        //return DT.RETURN_ERROR;
                    }

                    // jetzt noch die module mit der "FFFFFF" mac filtern
                    for (j = 0; j != modules_found_by_gc_dev_cfg; j++)
                    {
                        if (eth_devices[j].Network.mac == ETH_DEV_DEFAULT_MAC)
                        {
                            if (eth_devices[j].Network.ip == eth_dev.Network.ip)
                            {
                                // ein modul mit gleicher MAC und IP ist bereits vorhanden!
                                already_exist = true;
                                debug_modules_filtered++;
                            }
                        }
                    }

                    if (already_exist == false)
                    {
                        eth_devices.Add(eth_dev);
                        new_modules++;
                    }
                }

            }
        }

//MessageBox.Show("gc: " + debug_modules_filtered.ToString() +  " modul(e) gefiltert");

        return new_modules;
    }


    public static uint GetEthernetDevicesByBC(List<ETHDeviceConfig> eth_devices)
    {
        uint bc_dev_found_with_dev_cfg;
        uint bc_dev_found;
        uint tcp_dev_found = 0; // schonmal für später

        eth_devices.Clear();

        // neue methode - bc mit device config
        bc_dev_found_with_dev_cfg = gc_dev_cfg(eth_devices);
//MessageBox.Show("GetEthernetDevicesByBC: neue module = " + bc_dev_found_with_dev_cfg.ToString());

        // alt - bc ohne device config
        bc_dev_found = gc(eth_devices);
		//bc_dev_found = 0;
//MessageBox.Show("GetEthernetDevicesByBC: alte module = " + bc_dev_found.ToString());

        //return (0 + bc_dev_found + tcp_dev_found);
        return (bc_dev_found_with_dev_cfg + bc_dev_found + tcp_dev_found);
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
	/*
    public static uint deditec_bc_get_mac_list(out String[] mac_list, ref uint anz_addr)
    {
        Byte[] mac_buffer = new Byte[256];
		String dummy_str = "";
		uint dummy_uint = 0;
		uint i;

        int pos;
        int mac_length = 12;

		#if (!__MOBILE__)
			anz_addr = DT.Ext.DapiInternCommand(DT.Ext.DAPI_INTERN_CMD_BC_GET_MAC_LIST, dummy_str, dummy_str, mac_buffer, (uint)mac_buffer.Length, ref dummy_uint);
		#else
			anz_addr = DT.Mobile.BC.GetMacList(ref mac_buffer);
		#endif

        
        if (anz_addr > 0)
        {
            dummy_str = DT.Conv.ConvertByteArrayToString(mac_buffer, (uint)mac_buffer.Length);
            mac_list = new String[anz_addr];
            pos = 0;

            for (i = 0; i != anz_addr; i++)
            {
                mac_list[i] = dummy_str.Substring(pos, mac_length);
                pos += mac_length;
            }

            return DT.RETURN_OK;
        }
        else
        {
            // dummy init -> out !
            mac_list = new String[1];

            return DT.RETURN_ERROR;
        }
	}
    */
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint deditec_bc_get_byte_parameter(String mac_addr, uint parameter, ref Byte[] value)
    {
#if (!__MOBILE__)
        return DT.Ext.DapiInternCommand(DT.Ext.DAPI_INTERN_CMD_BC_GET_PARAMETER, mac_addr, "", value, (uint)value.Length, ref parameter);
#else
        return DT.Mobile.BC.GetParameter(DT.Conv.ConvertStringToByteArray(mac_addr), parameter, value);
#endif
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
	
    public static uint deditec_bc_set_byte_parameter(String mac_addr, uint parameter, Byte[] value)
    {
#if (!__MOBILE__)
        return DT.Ext.DapiInternCommand(DT.Ext.DAPI_INTERN_CMD_BC_SET_PARAMETER, mac_addr, "", value, (uint)value.Length, ref parameter);
#else
        return DT.Mobile.BC.SetParameter(DT.Conv.ConvertStringToByteArray(mac_addr), parameter, value);
#endif
    }
	

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

	public static uint deditec_bc_get_string_parameter(String mac_addr, uint parameter, ref String value)
	{
        uint ret;
        Byte[] buffer = new Byte[256];

		#if (!__MOBILE__)
        	ret = DT.Ext.DapiInternCommand(DT.Ext.DAPI_INTERN_CMD_BC_GET_PARAMETER, mac_addr, "", buffer, (uint)buffer.Length, ref parameter);
		#else
			ret = DT.Mobile.BC.GetParameter(DT.Conv.ConvertStringToByteArray(mac_addr), parameter, buffer);
		#endif
        value = DT.Conv.ConvertByteArrayToString(buffer, (uint)buffer.Length);

        return ret;
	}

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint deditec_bc_get_numeric_parameter(String mac_addr, uint parameter, ref uint value)
    {
        uint ret;
        Byte[] buffer = new Byte[256];

		#if (!__MOBILE__)
			ret = DT.Ext.DapiInternCommand(DT.Ext.DAPI_INTERN_CMD_BC_GET_PARAMETER, mac_addr, "", buffer, (uint)buffer.Length, ref parameter);
		#else
			ret = DT.Mobile.BC.GetParameter(DT.Conv.ConvertStringToByteArray(mac_addr), parameter, buffer);
		#endif

        try
        {
            value = Convert.ToUInt32(DT.Conv.ConvertByteArrayToString(buffer, (uint)buffer.Length));
        }
        catch (Exception)
        {
            return DT.RETURN_ERROR;
        }

        return ret;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
	
	public static uint deditec_bc_set_string_parameter(String mac_addr, uint parameter, String value)
	{
        Byte[] buffer = DT.Conv.ConvertStringToByteArray(value);

#if (!__MOBILE__)
        return DT.Ext.DapiInternCommand(DT.Ext.DAPI_INTERN_CMD_BC_SET_PARAMETER, mac_addr, "", buffer, (uint)buffer.Length, ref parameter);
#else
        return DT.Mobile.BC.SetParameter(DT.Conv.ConvertStringToByteArray(mac_addr), parameter, buffer);
#endif
	}
	
	
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

	public static uint deditec_bc_eth0_config(String mac_addr)
	{
        #if (!__MOBILE__)
        uint dummy_uint = 0;
            return DT.Ext.DapiInternCommand(DT.Ext.DAPI_INTERN_CMD_BC_ETH0_CONFIG, mac_addr, "", new Byte[0], 0, ref dummy_uint);
        #else
            return DT.Mobile.BC.Eth0Config(DT.Conv.ConvertStringToByteArray(mac_addr));
        #endif
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public class ETHDeviceConfig
    {
        public Byte[] dev_cfg_buffer = new Byte[256];

        public bool JS_Flag_ignore = false;
        public bool suppport_gc_dev_cfg = false;
        public bool suppport_bc_dev_cfg = false;

        public _Network Network = new _Network();
        public _Protocol Protocol = new _Protocol();
        public _DIP DIP = new _DIP();
        public _WriteProtection WriteProtection = new _WriteProtection();
        public _BLFWInfo BLFWInfo = new _BLFWInfo();
        public _HWKey HWKey = new _HWKey();
        public _BLFWRevision BLFWRevision = new _BLFWRevision();
        public _BoardName BoardName = new _BoardName();
        public _BoardID BoardID = new _BoardID();

        public ETHDeviceConfig()
        {
            // konstruktor für dummy / und ShowDeviceConfig()
        }

        public ETHDeviceConfig(Byte[] mac)
        {
            // konstruktor ALTE BC-Methode.. 
            Network.mac = DT.Conv.ConvertByteArrayToString(mac, (uint)mac.Length);

            DT.String.dt_convert_string_to_mac_addr_string(Network.mac, ref Network.mac_formatted);
        }

        public ETHDeviceConfig(Byte[] mac, Byte[] dev_cfg)
        {
            // konstruktor NEUE BC-Methode.. 
            suppport_gc_dev_cfg = true;

            // --------------------
            // MAC
            Network.mac = DT.Conv.ByteArrayToHexString(mac);
            DT.String.dt_convert_string_to_mac_addr_string(Network.mac, ref Network.mac_formatted);

            // --------------------
            // DeviceConfig
            Array.Copy(dev_cfg, dev_cfg_buffer, dev_cfg.Length);    // ka ob man die sicherheitskopie braucht... sicher ist sicher...

            SetDeviceConfig(dev_cfg);
        }

        public class _Network
        {
            public bool supported;
            public String mac;
            public String mac_formatted;
            public String ip;
            public String netmask;
            public String gateway;
            public bool dhcp;

			public uint port;
        }

        public class _Protocol
        {
            public bool supported;
            public bool normal;
            public bool enc_normal;
            public bool enc_admin;
            public bool enc_admin_temp;
        }

        public class _DIP
        {
            public bool supported;
            public bool dip0;
            public bool dip1;
            public bool dip2;
            public bool dip3;
        }

        public class _WriteProtection
        {
            public bool supported;
            public bool is_wp;
        }

        public class _HWKey
        {
            public bool supported;
            public bool temp_admin_key;
            public uint temp_admin_key_remaining;
            public uint temp_admin_remaining;
        }

        public class _BoardName
        {
            public bool supported;
            public String boardname;
        }

        public class _BLFWInfo
        {
            public bool supported;

            public bool is_bl;
            public uint bootloader_id0;
            public uint bootloader_id1;
            public uint bootloader_id2;
            public uint bootloader_id3;
            public uint bootloader_id4;
            public uint bootloader_id5;
            public uint bootloader_id6;
            public uint bootloader_id7;

            public bool is_fw;
            public uint delib_module_id;
        }

        public class _BLFWRevision
        {
            public bool supported;
            public bool is_bl;
            public bool is_fw;
            public uint ver_nr;
            public String ver_nr_formatted;
        }

        public class _BoardID
        {
            public bool supported;
            public uint boardID;
        }

        public void SetDeviceConfig(Byte[] dev_cfg)
        {
            Network.supported = (dev_cfg[0] == PACKET_VALID);
            Network.ip = DT.Conv.Convert4BytesToIPString(dev_cfg, 1);
            Network.netmask = DT.Conv.Convert4BytesToIPString(dev_cfg, 5);
            Network.gateway = DT.Conv.Convert4BytesToIPString(dev_cfg, 9);
            Network.dhcp = (dev_cfg[13] == 1);

            DIP.supported = (dev_cfg[14] == PACKET_VALID);
            DIP.dip0 = (((dev_cfg[15] >> 0) & 1) == 1);
            DIP.dip1 = (((dev_cfg[15] >> 1) & 1) == 1);
            DIP.dip2 = (((dev_cfg[15] >> 2) & 1) == 1);
            DIP.dip3 = (((dev_cfg[15] >> 3) & 1) == 1);

            Protocol.supported = (dev_cfg[16] == PACKET_VALID);
            Protocol.normal = (((dev_cfg[17] >> 0) & 1) == 1);
            Protocol.enc_normal = (((dev_cfg[17] >> 1) & 1) == 1);
            Protocol.enc_admin = (((dev_cfg[17] >> 2) & 1) == 1);
            Protocol.enc_admin_temp = (((dev_cfg[17] >> 3) & 1) == 1);

            WriteProtection.supported = (dev_cfg[18] == PACKET_VALID);
            WriteProtection.is_wp = (dev_cfg[19] == 1);

            HWKey.supported = (dev_cfg[20] == PACKET_VALID);
            HWKey.temp_admin_key = (dev_cfg[21] == 1);
            HWKey.temp_admin_key_remaining = (uint)(dev_cfg[22] << 8);
            HWKey.temp_admin_key_remaining |= (uint)(dev_cfg[23] << 0);
            HWKey.temp_admin_remaining = (uint)(dev_cfg[24] << 8);
            HWKey.temp_admin_remaining |= (uint)(dev_cfg[25] << 0);

            BoardName.supported = (dev_cfg[26] == PACKET_VALID);
            BoardName.boardname = DT.Conv.ConvertByteArrayToString(
                (Byte[])DT.Conv.SplitArray(dev_cfg, 27, 64),
                64
            );

            BLFWInfo.supported = (dev_cfg[92] == PACKET_VALID);
            BLFWInfo.is_bl = (dev_cfg[93] == IS_BL);
            BLFWInfo.bootloader_id0 = dev_cfg[94];
            BLFWInfo.bootloader_id1 = dev_cfg[95];
            BLFWInfo.bootloader_id2 = dev_cfg[96];
            BLFWInfo.bootloader_id3 = dev_cfg[97];
            BLFWInfo.bootloader_id4 = dev_cfg[98];
            BLFWInfo.bootloader_id5 = dev_cfg[99];
            BLFWInfo.bootloader_id6 = dev_cfg[100];
            BLFWInfo.bootloader_id6 = dev_cfg[101];
            BLFWInfo.is_fw = (dev_cfg[93] == IS_FW);
            BLFWInfo.delib_module_id = dev_cfg[94];
            if (BLFWInfo.is_bl) BLFWInfo.delib_module_id = 0;

            BLFWRevision.supported = (dev_cfg[102] == PACKET_VALID);
            BLFWRevision.is_bl = (dev_cfg[103] == IS_BL);
            BLFWRevision.is_fw = (dev_cfg[103] == IS_FW);
            BLFWRevision.ver_nr  = (uint)(dev_cfg[104] << 8);
            BLFWRevision.ver_nr |= (uint)(dev_cfg[105] << 0);
            BLFWRevision.ver_nr_formatted = DT.Conv.ByteArrayToHexString(dev_cfg, 104, 2);

            BoardID.supported = (dev_cfg[106] == PACKET_VALID);
            BoardID.boardID  = (uint)(dev_cfg[107]) << 8;
            BoardID.boardID |= (uint) dev_cfg[108];

			if (dev_cfg [109] == PACKET_VALID)
			{
				Network.port =  (uint)(dev_cfg [110]) << 8;
				Network.port |= (uint) dev_cfg [111];
			} 
			else 
			{
				Network.port = 9912;
			}

        }

        public void ShowDeviceConfig()
        {
            String dev_cfg_text = "";

            if (Network.supported)
            {
                dev_cfg_text += "----------------------------------\n";
                dev_cfg_text += "Network\n";
                dev_cfg_text += "----------------------------------\n";
                dev_cfg_text += "MAC:\t" + Network.mac_formatted + "\n";
                dev_cfg_text += "IP:\t" + Network.ip + "\n";
                dev_cfg_text += "Netmask:\t" + Network.netmask + "\n";
                dev_cfg_text += "Gateway:\t" + Network.gateway + "\n";
                dev_cfg_text += "DHCP:\t" + Network.dhcp + "\n";
            }

            if (DIP.supported)
            {
                dev_cfg_text += "----------------------------------\n";
                dev_cfg_text += "DIP\n";
                dev_cfg_text += "----------------------------------\n";
                dev_cfg_text += "DIP1:\t" + DIP.dip0 + "\n";
                dev_cfg_text += "DIP2:\t" + DIP.dip1 + "\n";
                dev_cfg_text += "DIP3:\t" + DIP.dip2 + "\n";
                dev_cfg_text += "DIP4:\t" + DIP.dip3 + "\n";
            }

            if (Protocol.supported)
            {
                dev_cfg_text += "----------------------------------\n";
                dev_cfg_text += "Protocol\n";
                dev_cfg_text += "----------------------------------\n";
                dev_cfg_text += "Normal:\t\t" + Protocol.normal + "\n";
                dev_cfg_text += "Normal (Enc):\t" + Protocol.enc_normal + "\n";
                dev_cfg_text += "Admin (Enc):\t" + Protocol.enc_admin + "\n";
                dev_cfg_text += "Admin Temp:\t" + Protocol.enc_admin_temp + "\n";
            }

            if (WriteProtection.supported)
            {
                dev_cfg_text += "----------------------------------\n";
                dev_cfg_text += "Write Protection\n";
                dev_cfg_text += "----------------------------------\n";
                dev_cfg_text += "WP:\t" + WriteProtection.is_wp + "\n";
            }

            if (HWKey.supported)
            {
                dev_cfg_text += "----------------------------------\n";
                dev_cfg_text += "Hardware Key\n";
                dev_cfg_text += "----------------------------------\n";
                dev_cfg_text += "Admin Key:\t" + HWKey.temp_admin_key + "\n";
                dev_cfg_text += "Admin Key Zeit:\t" + HWKey.temp_admin_key_remaining + "\n";
                dev_cfg_text += "Admin Zeit:\t" + HWKey.temp_admin_remaining + "\n";
            }

            if (BoardName.supported)
            {
                dev_cfg_text += "----------------------------------\n";
                dev_cfg_text += "Board Name\n";
                dev_cfg_text += "----------------------------------\n";
                dev_cfg_text += "Name:\t" + BoardName.boardname + "\n";
            }

            if (BLFWInfo.supported)
            {
                dev_cfg_text += "----------------------------------\n";
                dev_cfg_text += "BL/FW Info\n";
                dev_cfg_text += "----------------------------------\n";

                if (BLFWInfo.is_bl)
                {
                    dev_cfg_text += "State:\tBootloader\n";
                    dev_cfg_text += "BL ID0:\t" + BLFWInfo.bootloader_id0 + "\n";
                    dev_cfg_text += "BL ID1:\t" + BLFWInfo.bootloader_id1 + "\n";
                    dev_cfg_text += "BL ID2:\t" + BLFWInfo.bootloader_id2 + "\n";
                    dev_cfg_text += "BL ID3:\t" + BLFWInfo.bootloader_id3 + "\n";
                    dev_cfg_text += "BL ID4:\t" + BLFWInfo.bootloader_id4 + "\n";
                    dev_cfg_text += "BL ID5:\t" + BLFWInfo.bootloader_id5 + "\n";
                    dev_cfg_text += "BL ID6:\t" + BLFWInfo.bootloader_id6 + "\n";
                    dev_cfg_text += "BL ID7:\t" + BLFWInfo.bootloader_id7 + "\n";
                }
                else if (BLFWInfo.is_fw)
                {
                    dev_cfg_text += "State:\tFirmware\n";
                    dev_cfg_text += "DELIB-Mod-ID:\t" + BLFWInfo.delib_module_id + "\n";
                }
            }

            if (BLFWRevision.supported)
            {
                dev_cfg_text += "----------------------------------\n";
                dev_cfg_text += "BL/FW Revision\n";
                dev_cfg_text += "----------------------------------\n";
                if (BLFWInfo.is_bl)
                {
                    dev_cfg_text += "State:\tBootloader\n";
                }
                else if (BLFWInfo.is_fw)
                {
                    dev_cfg_text += "State:\tFirmware\n";
                }
                dev_cfg_text += "Version:\t" + BLFWRevision.ver_nr_formatted + "\n";
            }

            if (BoardID.supported)
            {
                dev_cfg_text += "----------------------------------\n";
                dev_cfg_text += "Board ID\n";
                dev_cfg_text += "----------------------------------\n";
                dev_cfg_text += "ID:\t" + BoardID.boardID + "\n";
            }

			#if (!__MOBILE__)
            MessageBox.Show(dev_cfg_text);
			#endif
        }

    }   // public class ETHDeviceConfig
}
