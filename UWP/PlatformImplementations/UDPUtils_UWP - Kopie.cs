using System;
using Xamarin.Forms;
using IOControl.UWP;

using System.Net;
using System.Net.Sockets;

[assembly: Dependency(typeof(UDPUtils_UWP))]

namespace IOControl.UWP
{
    public class UDPUtils_UWP : IUDPUtils
    {
        //DatagramPacket Packet { get; set; }
        UdpClient Socket { get; set; }
        Byte[] Rx { get; set; }
        int Port { get; set; }

        public void CreateSocket(int port, int timeout_ms)
        {
            Socket = new UdpClient(port);
            Port = port;
            //Socket.

            //Socket.Broadcast = true;
            //Socket.SoTimeout = timeout_ms;

            Rx = new Byte[1024];
        }

        public void Send(String addr, int port, Byte[] message, int message_length)
        {
            /*
            DatagramPacket packet = new DatagramPacket(message, message_length, InetAddress.GetByName(addr), port);
            Socket.Send(packet);
            */

            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(addr), port);
            Socket.SendAsync(message, message_length, ep);
        }

        public bool DataPresent()
        {
            Bla();

            return true;
        }

        async System.Threading.Tasks.Task<bool> Bla()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, Port);
            bool data_present = true;

            UdpReceiveResult x = await Socket.ReceiveAsync();



            return data_present;
        }



        public int Receive(ref Byte[] data)
        {
            /*
            data = Packet.GetData();
            return Packet.Length;
            */

            return 1;
        }

        public void CloseSocket()
        {
            //Socket.Close();
        }
    }
}