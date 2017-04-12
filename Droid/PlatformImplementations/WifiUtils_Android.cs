using System;
using Xamarin.Forms;
using IOControl.Droid;

using Android.Net;
using Android.Content;
using Android.Net.Wifi;
using Java.Nio;
using Java.Lang;
using Java.Math;
using Java.Net;

[assembly: Dependency(typeof(WifiUtils_Android))]

namespace IOControl.Droid
{
    public class WifiUtils_Android : IWifiUtils
    {
        public bool WlanIsConnected()
        {
            ConnectivityManager cm = (ConnectivityManager)Xamarin.Forms.Forms.Context.GetSystemService(Context.ConnectivityService);
            NetworkInfo info = cm.ActiveNetworkInfo;
            return (info != null && info.IsConnected && info.Type == ConnectivityType.Wifi);
        }


        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public string GetDeviceIP()
        {
            WifiManager wifiManager = (WifiManager)Xamarin.Forms.Forms.Context.GetSystemService(Context.WifiService);
            int ipAddress = wifiManager.ConnectionInfo.IpAddress;

            // Convert little-endian to big-endianif needed
            if (ByteOrder.NativeOrder().Equals(ByteOrder.LittleEndian))
            {
                ipAddress = Integer.ReverseBytes(ipAddress);
            }

            byte[] ipByteArray = BigInteger.ValueOf(ipAddress).ToByteArray();

            string ipAddressString;
            try
            {
                ipAddressString = InetAddress.GetByAddress(ipByteArray).HostAddress;
            }
            catch (Java.Lang.Exception)
            {
                ipAddressString = null;
            }

            return ipAddressString;
        }
    }
}