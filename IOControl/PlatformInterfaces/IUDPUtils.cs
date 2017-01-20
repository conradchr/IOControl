using System;

namespace IOControl
{
    public interface IUDPUtils
    {
        void CreateSocket(int port, int timeout_ms);
        void Send(String addr, int port, Byte[] message, int message_length);
        bool DataPresent();
        int Receive(ref Byte[] data);
        void CloseSocket();
    }
}
