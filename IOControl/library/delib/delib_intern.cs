//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//
//
//
//	delib_intern.cs
//	project: DELIB
//
//
//  (c) DEDITEC GmbH, 2016
//  web: http://www.deditec.de/
//  mail: vertrieb@deditec.de
//
//
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************

using System;
//using System.Text;
//using System.Net.Sockets;	// TcpClient + NetworkStream

public class DapiHandle
{
	public _TCP_IO TCP_IO = new _TCP_IO();

	public string ip_addr;
	public int timeout;
	public int port;

	public uint encryption_type;
	public string encryption_password;

	public uint job_id;

	public class _TCP_IO
	{
        /*
		public TcpClient client;
		public NetworkStream stream;
        */

        public Object client;
        public Object stream;
	}
}