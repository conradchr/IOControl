//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//
//
//
//	delib_Dapi_io_commands.cs
//	project: DELIB
//
//
//	(c) DEDITEC GmbH, 2009-2014
//	web: http://www.deditec.de/
//	mail: vertrieb@deditec.de
//
//
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************
//***********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Net;
//using System.Net.Sockets;

using IOControl;


namespace DELIB
{
    public partial class DeLibNET
    {
        public static uint WriteMultipleParams(uint handle, String[] parameter, String[] value)
        {
            string[] dummy;
            return RWMultipleParams (handle, DT.DEDITEC_TCPSPECIAL_CMD_PARAMETER_WRITE_MULTIPLE, parameter, out dummy, value);
        }

        public static uint ReadMultipleParams(uint handle, String[] parameter, out String[] value)
        {
            return RWMultipleParams (handle, DT.DEDITEC_TCPSPECIAL_CMD_PARAMETER_READ_MULTIPLE, parameter, out value, null);
        }

        public static uint RWMultipleParams(uint handle, uint cmd, String[] parameter, out String[] valueR, String[] valueW)
        {
            byte[] tx = new byte[4000];
            byte[] rx = new byte[4000];
            int tx_pos = 0;
            int rx_pos = 0;
            uint len = 0;
            int i;
            uint ret;

            for (i = 0; i != parameter.Length; i++)
            {
                if (parameter[i].Contains("#"))
                {
                    string[] split = parameter[i].Split('#');
                    int number = Convert.ToInt32(split[1]);

                    tx[tx_pos++] = (byte)((number >> 0) & 0xff);
                    tx[tx_pos++] = (byte)((number >> 8) & 0xff);
                    tx[tx_pos++] = (byte)((number >> 16) & 0xff);
                    tx[tx_pos++] = (byte)((number >> 24) & 0xff);

                    tx[tx_pos++] = (byte)((split[0].Length >> 0) & 0xff);
                    tx[tx_pos++] = (byte)((split[0].Length >> 8) & 0xff);
                    tx[tx_pos++] = (byte)((split[0].Length >> 16) & 0xff);
                    tx[tx_pos++] = (byte)((split[0].Length >> 24) & 0xff);

                    Array.Copy(DT.Conv.ConvertStringToByteArray(split[0]), 0, tx, tx_pos, split[0].Length);
                    tx_pos += split[0].Length;
                }
                else
                {
                    tx[tx_pos++] = 0xff;
                    tx[tx_pos++] = 0xff;
                    tx[tx_pos++] = 0xff;
                    tx[tx_pos++] = 0xff;

                    tx[tx_pos++] = (byte)((parameter[i].Length >> 0) & 0xff);
                    tx[tx_pos++] = (byte)((parameter[i].Length >> 8) & 0xff);
                    tx[tx_pos++] = (byte)((parameter[i].Length >> 16) & 0xff);
                    tx[tx_pos++] = (byte)((parameter[i].Length >> 24) & 0xff);

                    Array.Copy(DT.Conv.ConvertStringToByteArray(parameter[i]), 0, tx, tx_pos, parameter[i].Length);
                    tx_pos += parameter[i].Length;
                }


                if (cmd == DT.DEDITEC_TCPSPECIAL_CMD_PARAMETER_WRITE_MULTIPLE)
                {
                    tx[tx_pos++] = (byte)((valueW[i].Length >> 0) & 0xff);
                    tx[tx_pos++] = (byte)((valueW[i].Length >> 8) & 0xff);
                    tx[tx_pos++] = (byte)((valueW[i].Length >> 16) & 0xff);
                    tx[tx_pos++] = (byte)((valueW[i].Length >> 24) & 0xff);

                    Array.Copy(DT.Conv.ConvertStringToByteArray(valueW[i]), 0, tx, tx_pos, valueW[i].Length);
                    tx_pos += valueW[i].Length;
                }
            }

            ret = DT.Delib.DapiSpecialCommandExt( handle, DT.Ext.DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_CONFIG_DATA, 0, 1, 0, ref DT.dummy_uint, ref DT.dummy_uint, ref DT.dummy_uint,
                                            tx, (uint)tx_pos, DT.dummy_byte, 0, rx, (uint)rx.Length, ref len);

            //empfangen
            valueR = new string[parameter.Length];
            if (cmd == DT.DEDITEC_TCPSPECIAL_CMD_PARAMETER_READ_MULTIPLE)
            {
                for (i = 0; i != valueR.Length; i++)
                {
                    len = (uint)rx[rx_pos++] << 0;
                    len |= (uint)rx[rx_pos++] << 8;
                    len |= (uint)rx[rx_pos++] << 16;
                    len |= (uint)rx[rx_pos++] << 24;

                    valueR[i] = DT.Conv.ConvertByteArrayToString(
                        (byte[])DT.Conv.SplitArray(rx, (uint)rx_pos, (uint)len),
                        (uint)len
                    );
                    rx_pos += (int) len;
                }
            }


            Sess.Log("RWMultipleParams ret= " + ret.ToString());
            return ret;
        }

    }
}
