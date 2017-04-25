
using System;
using Xamarin.Forms;
using IOControl.UWP;

using System.Net.Sockets;
using System.Runtime.InteropServices;	// GCHandle


[assembly: Dependency(typeof(UDPUtils_UWP))]

namespace IOControl.UWP
    {
    public class DelibUtils_UWP : IDelibUtils
    {
        public uint DapiOpenModuleEx(String ipAdresseDeditec, uint portNoDeditec, uint timeout_ms, String enc_pw)
        {
            DapiHandle dapiHandle = new DapiHandle();
            dapiHandle.ip_addr = ipAdresseDeditec;
            dapiHandle.port = (int)portNoDeditec;
            dapiHandle.timeout = (int)timeout_ms;

            dapiHandle.encryption_type = DT.DAPI_OPEN_MODULE_ENCRYPTION_TYPE_NONE;
            dapiHandle.encryption_password = enc_pw;

            try
            {

                //dapiHandle.TCP_IO.client = new TcpClient(dapiHandle.ip_addr, dapiHandle.port);
                dapiHandle.TCP_IO.client = new TcpClient();
                ((TcpClient)dapiHandle.TCP_IO.client).Client.Connect(dapiHandle.ip_addr, dapiHandle.port);

                dapiHandle.TCP_IO.stream = ((TcpClient)dapiHandle.TCP_IO.client).GetStream();
                ((TcpClient)dapiHandle.TCP_IO.client).ReceiveTimeout = dapiHandle.timeout;
                ((TcpClient)dapiHandle.TCP_IO.client).SendTimeout = dapiHandle.timeout;
            }
            catch (Exception e)
            {
                Sess.LogEX("DapiOpenModuleEx", e.ToString());
                return 0;
            }

            return (uint)((IntPtr)GCHandle.Alloc(dapiHandle));
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public void DapiCloseModule(uint handle)
        {
            try
            {
                DapiHandle dapiHandle = (DapiHandle)((GCHandle)((IntPtr)handle)).Target;

                //((NetworkStream)dapiHandle.TCP_IO.stream).Close();
                ((NetworkStream)dapiHandle.TCP_IO.stream).Dispose();

                //((TcpClient)dapiHandle.TCP_IO.client).Close();
                ((TcpClient)dapiHandle.TCP_IO.client).Dispose();

                dapiHandle = null;
            }
            catch (Exception e)
            {
                Sess.LogEX("DapiCloseModule", e.ToString());
            }
        }

        public DapiHandle GetDapiHandle(uint handle)
        {
            return (DapiHandle)((GCHandle)((IntPtr)handle)).Target;
        }

        public void TCPSend(DapiHandle handle, Byte[] message, int message_length)
        {
            ((NetworkStream)handle.TCP_IO.stream).Write(message, 0, message_length);
            ((NetworkStream)handle.TCP_IO.stream).Flush();
        }

        public int TCPReceive(DapiHandle handle, ref Byte[] data)
        {
            return ((NetworkStream)handle.TCP_IO.stream).Read(data, 0, data.Length);
        }
    }
}