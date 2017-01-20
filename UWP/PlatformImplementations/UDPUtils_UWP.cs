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
        UdpClient xSocket { get; set; }
        Byte[] Rx { get; set; }
        int Port { get; set; }

        public void CreateSocket(int port, int timeout_ms)
        {
            /*
            Socket = new UdpClient(port);
            Port = port;

            Socket.Client.EnableBroadcast = true;
            Socket.Client.ReceiveTimeout = timeout_ms;

            //Socket.Broadcast = true;
            //Socket.SoTimeout = timeout_ms;

            Rx = new Byte[1024];
            */
        }

        public void Send(String addr, int port, Byte[] message, int message_length)
        {
            IPEndPoint RemoteEndPoint = new IPEndPoint(IPAddress.Parse(addr), port);

            Socket server = new Socket(AddressFamily.InterNetwork,
                                       SocketType.Dgram, ProtocolType.Udp);
            
            server.SendTo(message, message_length, SocketFlags.Broadcast, RemoteEndPoint);
        }

        public bool DataPresent()
        {
            Bla();

            return true;
        }

        async System.Threading.Tasks.Task<bool> Bla()
        {
            /*
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, Port);
            bool data_present = true;

            UdpReceiveResult x = await Socket.ReceiveAsync();
            


            return data_present;
            */

            return true;
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