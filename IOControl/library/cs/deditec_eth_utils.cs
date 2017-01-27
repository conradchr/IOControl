using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;   // for dll import
using System.Net;                   
using System.Net.NetworkInformation;    // for ping
//using System.Net.Sockets;

#if (!__MOBILE__)
using System.Windows.Forms;
#endif

using ETHDeviceConfig = DT_BCFunctions.ETHDeviceConfig;


public class DT_ETH
{
    public const uint TCP_DEFAULT_TIMEOUT = 5000;
    public const uint TCP_DEFAULT_PORT = 9912;

    public const uint CONNECTION_TCP = 1;
    public const uint CONNECTION_UDP = 2;

    public const uint ETH_TIMEOUT_MIN = 5;
    public const uint ETH_TIMEOUT_MAX = 30000;
    public const uint ETH_PORT_MIN = 1024;
    public const uint ETH_PORT_MAX = 65535;

    public const uint DIP_SWITCH_DELIVERY_STANDARD = 0xf; // 1111 [bin] -> alles an!



#if (!__MOBILE__)
    static uint dummy_uint = 0;

    // GetMacOfIP()
    /*
    [DllImport("iphlpapi.dll", ExactSpelling = true)]
    static private extern int SendARP(int destIp, int srcIP, byte[] macAddr, ref uint physicalAddrLen);
    */

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    // Ping()
    public struct PingReturn
    {
        public bool pingSuccess;
        public bool macSuccess;
        public byte[] ip;
        public byte[] mac;
        public long pingTime;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint SetChannelName(uint handle, uint channel, String key, String value)
    {
        Byte[] key_buffer = DT.Conv.ConvertStringToByteArray(key);
        Byte[] val_buffer = DT.Conv.ConvertStringToByteArray(value);

        return DT.Delib.DapiSpecialCommandExt(handle, DT.Ext.DAPI_SPECIAL_CMDEXT_TCP_MODULE_SET_CONFIG_DATA, channel, 0, 0, ref dummy_uint, ref dummy_uint, ref dummy_uint, key_buffer, (uint)key_buffer.Length, val_buffer, (uint)val_buffer.Length, new Byte[] { 0 }, 0, ref dummy_uint);
    }

    public static uint GetChannelName(uint handle, uint channel, String key, ref String value)
    {
        Byte[] key_buffer = DT.Conv.ConvertStringToByteArray(key);
        Byte[] val_buffer = new Byte[256];
        uint val_length = 0;
        uint ret;

        if ((ret = DT.Delib.DapiSpecialCommandExt(handle, DT.Ext.DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_CONFIG_DATA, channel, 0, 0, ref dummy_uint, ref dummy_uint, ref dummy_uint, key_buffer, (uint)key_buffer.Length, new Byte[] { 0 }, 0, val_buffer, (uint)val_buffer.Length, ref val_length)) == DT.RETURN_OK)
        {
            value = DT.Conv.ConvertByteArrayToString(val_buffer, val_length);
            return DT.RETURN_OK;
        }

        return ret;
    }

    public static uint SetParameter(uint handle, String key, String value)
    {
        Byte[] key_buffer = DT.Conv.ConvertStringToByteArray(key);
        Byte[] val_buffer = DT.Conv.ConvertStringToByteArray(value);

        return DT.Delib.DapiSpecialCommandExt(handle, DT.Ext.DAPI_SPECIAL_CMDEXT_TCP_MODULE_SET_CONFIG_DATA, UInt32.MaxValue, 0, 0, ref dummy_uint, ref dummy_uint, ref dummy_uint, key_buffer, (uint)key_buffer.Length, val_buffer, (uint) val_buffer.Length, new Byte[] { 0 }, 0, ref dummy_uint);
    }

    public static uint GetParameter(uint handle, String key, ref String value)
    {
        Byte[] key_buffer = DT.Conv.ConvertStringToByteArray(key);
        Byte[] val_buffer = new Byte[256];
        uint val_length = 0;
        uint ret;

        if ((ret = DT.Delib.DapiSpecialCommandExt(handle, DT.Ext.DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_CONFIG_DATA, UInt32.MaxValue, 0, 0, ref dummy_uint, ref dummy_uint, ref dummy_uint, key_buffer, (uint)key_buffer.Length, new Byte[] { 0 }, 0, val_buffer, (uint)val_buffer.Length, ref val_length)) == DT.RETURN_OK)
        {
            value = DT.Conv.ConvertByteArrayToString(val_buffer, val_length);
            return DT.RETURN_OK;
        }

        return ret;
    }


    public static String GetIPIfDeviceIsInBL(List<ETHDeviceConfig> eth_devices)
    {
        int i;

        for (i = 0; i != eth_devices.Count; i++)
        {
            if ((eth_devices[i].BLFWInfo.is_bl == true) && (eth_devices[i].BLFWInfo.supported == true))
            {
                //MessageBox.Show("GetIPIfDeviceIsInBL: IP " + eth_devices[i].Network.ip + " ist im Bootloader!");
                return eth_devices[i].Network.ip;
            }
        }

        return String.Empty;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint IdentifyEthernetDevice(uint handle, ETHDeviceConfig eth_device, uint connection_type, uint blink_count, uint blink_time_on, uint blink_time_off)
    {
        uint ret;
        Byte[] buffer = new Byte[6];
        uint pos = 0;

        switch (connection_type)
        {
            case CONNECTION_TCP:
                ret = DT.Delib.DapiSpecialCommandExt(handle, DT.Ext.DAPI_SPECIAL_CMDEXT_TCP_MODULE_IDENTIFY, blink_count, blink_time_on, blink_time_off, ref pos, ref pos, ref pos, new Byte[] { 0 }, 0, new Byte[] { 0 }, 0, new Byte[] { 0 }, 0, ref pos);
                if ((DT.Utility.IsError() == true) || (ret != DT.Error.DAPI_ERR_NONE))
                {
                    return DT.RETURN_ERROR;
                }
                break;

            case CONNECTION_UDP:
                buffer[pos++] = (Byte)((blink_count >> 8) & 0xff);
                buffer[pos++] = (Byte)((blink_count >> 0) & 0xff);
                buffer[pos++] = (Byte)((blink_time_on >> 8) & 0xff);
                buffer[pos++] = (Byte)((blink_time_on >> 0) & 0xff);
                buffer[pos++] = (Byte)((blink_time_off >> 8) & 0xff);
                buffer[pos++] = (Byte)((blink_time_off >> 0) & 0xff);
                ret = DT.Bc.deditec_bc_set_byte_parameter(eth_device.Network.mac, DT.Bc.Parameter.DEDITEC_BC_PACKET_PARAM_IDENTIFY, buffer);
                if ((DT.Utility.IsError() == true) || (ret != DT.Error.DAPI_ERR_NONE))
                {
                    return DT.RETURN_ERROR;
                }
                break;

            default:
                return DT.RETURN_ERROR;
        }

        return DT.RETURN_OK;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint IdentifyEthernetDeviceIP(string ip_addr, uint blink_count, uint blink_time_on, uint blink_time_off)
    {
        DT.Delib.DAPI_OPENMODULEEX_STRUCT exbuffer = new DeLib.DeLibNET.DAPI_OPENMODULEEX_STRUCT();
        uint open_options;
        uint Proghandle;
        uint ret;

        open_options = 0;
        exbuffer.address = ip_addr;
        exbuffer.portno = 0;
        exbuffer.timeout = 500;
        Proghandle = DT.Delib.DapiOpenModuleEx(DT.Delib.ModuleID.ETHERNET_MODULE, 0x8000, exbuffer, open_options);
        if (Proghandle != 0)
        {
            ret = DT.ETH.IdentifyEthernetDevice(Proghandle, new ETHDeviceConfig(), DT.ETH.CONNECTION_TCP, blink_count, blink_time_on, blink_time_off);       // 10 sec
            // Blink 500ms an / 500ms aus  = Langsames Blinken = fertig !! 
            DT.Delib.DapiCloseModule(Proghandle);
            return DT.RETURN_OK;
        }

        return DT.RETURN_ERROR;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint GetEthernetDeviceDIPSwitch(uint handle, ref uint dip_switch)
    {
        uint dummy_uint = 0;
        uint ret;
        Byte[] dev_cfg = new Byte[256];
        uint dev_cfg_len = 0;

        ret = DT.Delib.DapiSpecialCommandExt(handle, DT.Ext.DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_CURRENT_CONFIG, 0, 0, 0, ref dummy_uint, ref dummy_uint, ref dummy_uint, new Byte[] { 0 }, 0, new Byte[] { 0 }, 0, dev_cfg, (uint)dev_cfg.Length, ref dev_cfg_len);
        if ((DT.Utility.IsError() == true) || (ret != DT.Error.DAPI_ERR_NONE))
        {
            // delib fehler beim command
            return DT.RETURN_ERROR;
        }

        if ((dev_cfg_len < 15) || (dev_cfg[14] != DT.Bc.PACKET_VALID))
        {
            // das ist ein buffer OHNE dip-schalter ... ODER ... dip schalter sind nicht supportet
            return DT.RETURN_ERROR;
        }

        dip_switch = Convert.ToUInt32(dev_cfg[15]);
        return DT.RETURN_OK;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    

    public static uint GetEthernetDeviceConfig(uint handle, ETHDeviceConfig eth_device, uint connection_type, ref Byte[] dev_cfg)
    {
        uint dummy_uint = 0;
        uint ret;

        switch (connection_type)
        {
            case CONNECTION_TCP:
                ret = DT.Delib.DapiSpecialCommandExt(handle, DT.Ext.DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_CURRENT_CONFIG, 0, 0, 0, ref dummy_uint, ref dummy_uint, ref dummy_uint, new Byte[] { 0 }, 0, new Byte[] { 0 }, 0, dev_cfg, (uint)dev_cfg.Length, ref dummy_uint);
                if ((DT.Utility.IsError() == true) || (ret != DT.Error.DAPI_ERR_NONE))
                {
                    return DT.RETURN_ERROR;
                }
                break;

            case CONNECTION_UDP:
                ret = DT.Bc.deditec_bc_get_byte_parameter(eth_device.Network.mac, DT.Bc.Parameter.DEDITEC_BC_PACKET_PARAM_TCP_CONFIG_USED, ref dev_cfg);
                if ((DT.Utility.IsError() == true) || (ret != DT.Error.DAPI_ERR_NONE))
                {
                    return DT.RETURN_ERROR;
                }
                break;

            default:
                return DT.RETURN_ERROR;
        }

        return DT.RETURN_OK;
    }

    // ----------------------------------------------------------------
    // ----------------------------------------------------------------
    // ----------------------------------------------------------------
    // ----------------------------------------------------------------
    // ----------------------------------------------------------------
    /// <summary>
    /// Ping a ip or hostname and get a PingReturn struct back
    /// </summary>
    /// <param name="ipOrHostname">e.g. "192.168.1.1" or "www.deditec.de"</param>
    /// <param name="timeoutInMilliseconds">e.g. 5000 (smaller values than 1000 won't work proberbly)</param>
    /// <param name="pingReturn">e.g. DT_tcp.PingReturn pingReturn</param>
    /// <returns></returns>
    static public uint Ping(string ipOrHostname, int timeoutInMilliseconds, out PingReturn pingReturn)
    {
        uint ret = DT.RETURN_OK;

        Ping ping = new Ping();
        PingReply pingReply;

        pingReturn = new PingReturn();

        try
        {
            pingReply = ping.Send(ipOrHostname, timeoutInMilliseconds);

            if (pingReply.Status == IPStatus.Success)
            {
                pingReturn.ip = new byte[4];
                pingReturn.mac = new byte[6];

                // ip
                if(DT.Conv.ConvertIPStringTo4Bytes(Convert.ToString(pingReply.Address), ref pingReturn.ip, 0) == DT.RETURN_OK)
                {
                    pingReturn.pingSuccess = true;
                }
                else
                {
                    pingReturn.pingSuccess = false;
                }

                if (pingReturn.pingSuccess == true)
                {
                    // mac
                    string macTemp = GetMacOfIP(Convert.ToString(pingReply.Address));

                    if (macTemp != "")
	                {
                        if (DT.Conv.ConvertMacStringTo6Bytes(macTemp, ref pingReturn.mac, 0) == DT.RETURN_OK)
                        {
                            pingReturn.macSuccess = true;
                        }
                        else
                        {
                            pingReturn.macSuccess = false;
                        }
	                }
                    else
                    {
                        pingReturn.macSuccess = false;
                    }
                    
                    // ping time
                    pingReturn.pingTime = pingReply.RoundtripTime;
                }

                ret = DT.RETURN_OK;
            }
            else
            {
                pingReturn.pingSuccess = false;
                pingReturn.ip = new byte[0];
                pingReturn.mac = new byte[0];;
                pingReturn.macSuccess = false;
                pingReturn.pingTime = 9999;
                ret = DT.RETURN_ERROR;
            }
        }
        catch (Exception)
        {
            // hostname could not be resolved
            ret = DT.RETURN_ERROR;
        }
     
        return ret;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    // ----------------------------------------------------------------

    // ----------------------------------------------------------------
    static private string GetMacOfIP(string ip)
    {
        IPAddress dst = IPAddress.Parse(ip); // the destination IP address

        byte[] macAddr = new byte[6];
        uint macAddrLen = (uint)macAddr.Length;

        if (SendARP(BitConverter.ToInt32(dst.GetAddressBytes(), 0), 0, macAddr, ref macAddrLen) != 0)
        {   
            // get mac failed
            return "";
        }
        else
        {
            // get mac success
            string[] str = new string[(int)macAddrLen];
            for (int i = 0; i < macAddrLen; i++)
            {
                str[i] = macAddr[i].ToString("x2");
            }

            return string.Join(":", str);
        }
    }

    // ----------------------------------------------------------------
    // ----------------------------------------------------------------
    // ----------------------------------------------------------------
    // ----------------------------------------------------------------
    // ----------------------------------------------------------------

    public class NetworkAdapterInfo
    {
        public String ip_addr;
        public String netmask;
        public String domain;

        public NetworkAdapterInfo()
        {
        }

        public NetworkAdapterInfo(String ip, String nm, String dom)
        {
            this.ip_addr = ip;
            this.netmask = nm;
            this.domain = dom;
        }
    }

    static public void GetAvailableNetworkAdapterInfo(List<NetworkAdapterInfo> adapterInfos)
    {
        IPAddress dns;
        int i;
        bool ok;

        adapterInfos.Clear();

        foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
        {
            ok = false;
            NetworkAdapterInfo networkAdapterInfo = new NetworkAdapterInfo();
            IPInterfaceProperties adapterProperties = adapter.GetIPProperties();

            // IP-Addr und Netzmaske 
            foreach (UnicastIPAddressInformation addr in adapterProperties.UnicastAddresses)
            {
                // check Adresse ist ipv4 UND NICHT Loopback Adresse
                if ( (addr.Address.AddressFamily == AddressFamily.InterNetwork) &&
                     (!IPAddress.IsLoopback(addr.Address)) )
                {
                    networkAdapterInfo.ip_addr = addr.Address.ToString();
                    networkAdapterInfo.netmask = addr.IPv4Mask.ToString();
                    ok = true;
                }
            }

            // DOMAIN
            // DHCP
            IPAddressCollection addresses = adapterProperties.DhcpServerAddresses;
            if (addresses.Count > 0)
            {
                // adapter hat DHCP
                foreach (IPAddress address in addresses)
                {
                    networkAdapterInfo.domain = adapterProperties.DnsSuffix.ToString();
                }
            }

            // Static IP
            if (adapterProperties.DnsAddresses.Count > 0)
            {
                for (i = 0; i != adapterProperties.DnsAddresses.Count; i++)
                {
                    if (IPAddress.TryParse(adapterProperties.DnsAddresses[i].ToString(), out dns))
                    {
                        if (dns.AddressFamily == AddressFamily.InterNetwork)
                        {
                            // adapter hat einen ipv4 DNS server
                            networkAdapterInfo.domain = Dns.GetHostEntry(dns).HostName;
                        }
                    }
                }
            }

            if ((ok) || (networkAdapterInfo.domain != null))
            {
                adapterInfos.Add(networkAdapterInfo);
            }
        }
    }

    // ----------------------------------------------------------------
    // ----------------------------------------------------------------
    // ----------------------------------------------------------------
    // ----------------------------------------------------------------
    // ----------------------------------------------------------------

    public class DT_NET_ERR
    {
        public const uint NONE = 0;
        public const uint ILLEGAL_SRC = 1;
        public const uint RESOLVE_SRC = 2;
        public const uint ILLEGAL_DEST = 3;
        public const uint RESOLVE_DEST = 4;
        public const uint ILLEGAL_NETMASK = 5;
        public const uint DIFF_NETWORK = 6;
        public const uint NETID_SRC = 7;
        public const uint NETID_DEST = 8;
        public const uint BC_SRC = 9;
        public const uint BC_DEST = 10;
        public const uint LOOPBACK_SRC = 11;
        public const uint LOOPBACK_DEST = 12;
    }

    public class DT_NET_IGNORE
    {
        public const uint NONE = 0x0;
        public const uint HOSTNAME = 0x1;
    }

    public static uint NetworkStuff(String src_hostname, String dest_hostname, String subnetmask, uint ignore_flag)
    {
        int PING_TIMEOUT_MS = 100;

        PingReturn pingReturn;
        uint i;

        uint srcIp = 0;
        uint destIp = 0;

        uint netmask = 0;
        uint netmaskBitPos = 0;
        bool netmaskPosFound = false;
        uint checkMask = 0;

        // --------------------
        // source check
        

        if (DT.String.dt_check_string_is_ip(src_hostname) == DT.RETURN_OK)
        {
            // ip
            DT.Conv.ConvertIPStringToUint(src_hostname, ref srcIp);
        }
        else
        {
            // check ignore flag
            if ((ignore_flag & DT_NET_IGNORE.HOSTNAME) == DT_NET_IGNORE.HOSTNAME)
            {
                // hostname check wird ignoriert -> also error
                return DT_NET_ERR.ILLEGAL_SRC;
            }

            // keine ip
            if (DT.String.dt_check_string_is_hostname(src_hostname) != DT.RETURN_OK)
            {
                // kein hostname
                return DT_NET_ERR.ILLEGAL_SRC;
            }

            if (DT.ETH.Ping(src_hostname, PING_TIMEOUT_MS, out pingReturn) != DT.RETURN_OK)
            {
                //hostname konnte NICHT angepingt/aufgelöst werden
                return DT_NET_ERR.RESOLVE_SRC;
            }

            DT.Conv.ConvertIPStringToUint(DT.Conv.Convert4BytesToIPString(pingReturn.ip, 0), ref srcIp);
        }

        // --------------------
        // destination check

        if (DT.String.dt_check_string_is_ip(dest_hostname) == DT.RETURN_OK)
        {
            // ip
            DT.Conv.ConvertIPStringToUint(dest_hostname, ref destIp);
        }
        else
        {
            // check ignore flag
            if ((ignore_flag & DT_NET_IGNORE.HOSTNAME) == DT_NET_IGNORE.HOSTNAME)
            {
                // hostname check wird ignoriert -> also error
                return DT_NET_ERR.ILLEGAL_DEST;
            }

            // keine ip
            if (DT.String.dt_check_string_is_hostname(dest_hostname) != DT.RETURN_OK)
            {
                // kein hostname
                return DT_NET_ERR.ILLEGAL_DEST;
            }

            if (DT.ETH.Ping(dest_hostname, PING_TIMEOUT_MS, out pingReturn) != DT.RETURN_OK)
            {
                //hostname konnte NICHT angepingt/aufgelöst werden
                return DT_NET_ERR.RESOLVE_DEST;
            }

            DT.Conv.ConvertIPStringToUint(DT.Conv.Convert4BytesToIPString(pingReturn.ip, 0), ref destIp);
        }

        // --------------------
        // netmask check

        if (DT.String.dt_check_string_is_ip(subnetmask) != DT.RETURN_OK)
        {
            return DT_NET_ERR.ILLEGAL_NETMASK;
        }

        DT.Conv.ConvertIPStringToUint(subnetmask, ref netmask);

        // schauen, ab welcher pos die netzmaske "wirklich" anfängt
        i = 0;
        do
        {
            if (((netmask >> (int)i) & 1) == 1)
            {
                netmaskBitPos = i;
                netmaskPosFound = true;
            }
            else
            {
                i++;
            }
        } while ((i <= 32) && (netmaskPosFound == false));

        // subnetzmaske 0 || irgendwo ne 0 drin || subnetzmaske = BC-Addr
        if ((netmaskPosFound == false) ||
            ((netmask >> (int)netmaskBitPos) != ((uint)Math.Pow(2, (32 - netmaskBitPos)) - 1)) ||
            (netmaskBitPos == 0))
        {
            return DT_NET_ERR.ILLEGAL_NETMASK;
        }

        // --------------------
        // ab hier beide ips und netzmaske "allein" ok
        // check untereinander..

        checkMask = (uint)Math.Pow(2, netmaskBitPos) - 1;

        if ((srcIp & netmask) != (destIp & netmask))
        {
            // gateway und ip sind nicht im selben netz
            return DT_NET_ERR.DIFF_NETWORK;
        }

        if ((srcIp & checkMask) == 0)
        {
            // ip-addr ist netzid
            return DT_NET_ERR.NETID_SRC;
        }

        if ((destIp & checkMask) == 0)
        {
            // gateway ist netzid
            return DT_NET_ERR.NETID_DEST;
        }

        if ((srcIp & checkMask) == checkMask)
        {
            // ip-addr ist broadcast
            return DT_NET_ERR.BC_SRC;
        }

        if ((destIp & checkMask) == checkMask)
        {
            // gateway ist broadcast
            return DT_NET_ERR.BC_DEST;
        }

        if (((srcIp >> 24) & 0xff) == 127)
        {
            // gateway ist broadcast
            return DT_NET_ERR.LOOPBACK_SRC;
        }

        if (((destIp >> 24) & 0xff) == 127)
        {
            // gateway ist broadcast
            return DT_NET_ERR.LOOPBACK_DEST;
        }

        // kein fehler
        return DT_NET_ERR.NONE;
    }

    // ----------------------------------------------------------------
    // ----------------------------------------------------------------
    // ----------------------------------------------------------------
    // ----------------------------------------------------------------
    // ----------------------------------------------------------------
#endif

}
