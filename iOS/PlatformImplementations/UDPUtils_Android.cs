/*
using System;
using Xamarin.Forms;
using IOControl.Droid;

using Java.Net;

[assembly: Dependency(typeof(UDPUtils_Android))]

namespace IOControl.Droid
{
    public class UDPUtils_Android : IUDPUtils
    {
        DatagramPacket Packet { get; set; }
        DatagramSocket Socket { get; set; }
        Byte[] Rx { get; set; }

        public void CreateSocket(int port, int timeout_ms)
        {
            Socket = new DatagramSocket(port);
            Socket.Broadcast = true;
            Socket.SoTimeout = timeout_ms;

            Rx = new Byte[1024];
        }

        public void Send(String addr, int port, Byte[] message, int message_length)
        {
            DatagramPacket packet = new DatagramPacket(message, message_length, InetAddress.GetByName(addr), port);
            Socket.Send(packet);
        }

        public bool DataPresent()
        {
            Packet = new DatagramPacket(Rx, Rx.Length);
            bool data_present = true;

            try
            {
                Socket.Receive(Packet);
            }
            catch (Exception)
            {
                data_present = false;
            }

            return data_present;
        }

        public int Receive(ref Byte[] data)
        {
            data = Packet.GetData();
            return Packet.Length;
        }

        public void CloseSocket()
        {
            Socket.Close();
        }
    }
}
*/