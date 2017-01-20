using System;

namespace IOControl
{
    public interface IDelibUtils
    {
        uint DapiOpenModuleEx(String ipAdresseDeditec, uint portNoDeditec, uint timeout_ms, String enc_pw);
        void DapiCloseModule(uint handle);
        DapiHandle GetDapiHandle(uint handle);

        void TCPSend(DapiHandle handle, Byte[] message, int message_length);
        int TCPReceive(DapiHandle handle, ref Byte[] data);
    }
}
