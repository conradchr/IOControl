using System;

namespace IOControl
{
    public interface IWifiUtils
    {
        bool WlanIsConnected();
        string GetDeviceIP();
    }
}
