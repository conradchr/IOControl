using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

using IOControl;

#if (!__MOBILE__)
//using System.Windows.Forms;
#endif

public class DT_ConvertUtils
{
    const uint RETURN_OK = 0;
    const uint RETURN_ERROR = 1;

    const Char LF = '\n';
    const Char CR = '\r';

#if (!__MOBILE__)
    /*
    public static void ByteDump(Byte[] buffer, uint pos_start, uint length)
    {
        if (buffer.Length < (pos_start + length))
        {
            MessageBox.Show("buffer.length= " + buffer.Length + " < pos_start=" + pos_start.ToString() + " + length=" + length.ToString());
            return;
        }

        String dump = "";
        uint i;
        uint j;

        for (i = pos_start; i < length; i += 16)
        {
            dump += i.ToString("X2") + "\t";

            for (j = 0; j != 16; j++)
            {
                dump += buffer[i + j].ToString("X2") + " ";
            }

            dump += "\t";

            for (j = 0; j != 16; j++)
            {
                if ((buffer[i + j] > 0x20) && (buffer[i + j] < 0x7f))
                {
                    dump += Convert.ToChar(buffer[i + j]) + " ";
                }
                else
                {
                    //dump += buffer[i + j].ToString() + " ";   damit sieht man dann die ip
                    dump += "  ";
                }
            }

            dump += "\n";
        }

        MessageBox.Show(dump);

    }
    */
#endif

	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
	// ----------------------------------------------------------------------------
    
	public static uint ArrayCompare(Array array_a, uint index_a, Array array_b, uint index_b, uint length)
	{
		uint i;
		uint cnt_not_equal = 0;

		for (i=0; i!=length; i++)
		{
            //DT.DBG.Print(array_a.GetValue(index_a + i).ToString() + " vs " + array_b.GetValue(index_b + i).ToString());

            if (array_a.GetValue((int)(index_a + i)).ToString() != array_b.GetValue((int)(index_b + i)).ToString())
            //if (((byte)array_a.GetValue(index_a + i)) != ((byte)array_b.GetValue(index_b + i)))
			{
                Sess.Log(array_a.GetValue((int)(index_a + i)).ToString() + " != " + array_b.GetValue((int)(index_b + i)).ToString());
				cnt_not_equal++;
			}
		}

        //DT.DBG.Print("ArrayCompare: return=" + cnt_not_equal.ToString());
		return cnt_not_equal;
	}
    
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    
    public static Array SplitArray(Array source, uint index, uint amount_bytes)
    {
        Array ret_array = Array.CreateInstance(source.GetValue((int) index).GetType(), (int) amount_bytes);
        Array.Copy(source, (int) index, ret_array, 0, (int) amount_bytes);
        return ret_array;
    }
    
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static Array[] ConvertArray2DTo1D(Array[,] source, uint source_index_a, uint source_index_b, uint amount_bytes)
    {
        Array[] ret = new Array[amount_bytes];
        uint i;
        uint pos = source_index_b;

        for (i = 0; i != amount_bytes; i++)
        {
            ret[i] = source[source_index_a, pos++];
        }

        return ret;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static String ConvertUIntToHexString(uint numeric)
	{
        return numeric.ToString("X2");
	}

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint ConvertHexStringToUInt(String hex_string)
    {
        return Convert.ToUInt32(hex_string, 16);
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static Byte[] ConvertStringToByteArray(String str)
    {
        Char[] chars = str.ToCharArray();
        Byte[] bytes = new Byte[str.Length];

        for (int i = 0; i != chars.Length; i++)
        {
            bytes[i] = Convert.ToByte(chars[i]);
        }

        return bytes;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static String ConvertByteArrayToStringWithOffset(Byte[] bytes, uint offset, uint amount_bytes_to_convert)
    {
        uint anz;
        uint i;

        // Genaue Laenge bis zur 0 ermitteln
        i = offset;
        while (((i < amount_bytes_to_convert)) && (bytes[i] != 0)) ++i;
        anz = i;

        // Zeichenkette mit korrekter Laenge        
        Char[] chars = new Char[(int)anz];

        for (i = offset; i != anz; i++)
        {
            chars[i] = Convert.ToChar(bytes[i]);
        }

        return new String(chars);
    }

    public static String ConvertByteArrayToString(Byte[] bytes, uint amount_bytes_to_convert)
    {
        return ConvertByteArrayToStringWithOffset(bytes, 0, amount_bytes_to_convert);
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static String[] ConvertByteArrayToStringArrayWithoutLF(Byte[] bytes, uint amount_bytes_to_convert)
    {
        String str = DT.Conv.ConvertByteArrayToString(bytes, amount_bytes_to_convert);
        
        return str.Split(LF);
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static String Convert4BytesToIPString(Byte[] bytes, uint offset)
    {
        StringBuilder str = new StringBuilder();
        uint i = 0;

        if (bytes.Length > (int)offset)
        {
            str.Append(bytes[offset].ToString());
        }

        for (i = (offset + 1); i != (offset + 4); i++)
        {
            if (bytes.Length > (int)i)
            {
                str.Append(".");
                str.Append(bytes[i].ToString());
            }
        }

        return str.ToString();
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    /*
    public static uint ConvertIPStringTo4Bytes(string ipString, ref byte[] ipByte, uint ipByteOffset)
    {
        uint ret = DT.RETURN_OK;
        IPAddress ipAddress;

        try
        {
            if (IPAddress.TryParse(ipString, out ipAddress))
            {
                byte[] temp = ipAddress.GetAddressBytes();

                for (int i = 0; i < temp.Length; i++)
                {
                    ipByte[i + ipByteOffset] = temp[i];
                }

                ret = DT.RETURN_OK;
            }
            else
            {
                ret = DT.RETURN_ERROR;
            }
        }
        catch (Exception)
        {
            ret = DT.RETURN_ERROR;
        }

        return ret;
    }
    */
    /*
    public static uint ConvertIPStringToUint(String ipString, ref uint ipUint)
    {
        Byte[] ipByte = new Byte[4];
        int i;
        ipUint = 0;

        if (ConvertIPStringTo4Bytes(ipString, ref ipByte, 0) != DT.RETURN_ERROR)
        {
            for (i = 0; i != 4; i++)
            {
                ipUint |= (((uint) ipByte[3-i]) << i*8);
            }

            return DT.RETURN_OK;
        }

        return DT.RETURN_ERROR;
    }
    */
    // ----------------------------------------------------------------------------

    public static uint ConvertMacStringTo6Bytes(string macString, ref byte[] macByte, uint macByteOffset)
    {
        uint ret = DT.RETURN_OK;

        // remove : from mac string
        while (macString.Contains(":"))
        {
            macString = macString.Replace(":", "");
        }

        try
        {
            if (macString.Length == 12)
            {
                byte[] temp = HexToByteArray(macString);

                for (int i = 0; i < temp.Length; i++)
                {
                    macByte[i + macByteOffset] = temp[i];
                }

                ret = DT.RETURN_OK;
            }
            else
            {
                ret = DT.RETURN_ERROR;
            }
        }
        catch (Exception)
        {
            ret = DT.RETURN_ERROR;
        }
        
        return ret;
    }

    // Convert a Hex string into a byte arry e.g. 00 = 0, FF = 255 ...
    public static byte[] HexToByteArray(String hexString)
    {
        int NumberChars = hexString.Length;
        byte[] bytes = new byte[NumberChars / 2];

        for (int i = 0; i < NumberChars; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
        }

        return bytes;
    }

    public static String ByteArrayToHexString(Byte[] buffer)
    {
        /*
        String ret = "";
        int i;

        for (i = 0; i != buffer.Length; i++)
        {
            ret += buffer[i].ToString("X2");
        }

        return ret;
         */
        return ByteArrayToHexString(buffer, 0, (uint) buffer.Length);
    }

    public static String ByteArrayToHexString(Byte[] buffer, uint index, uint amount_bytes)
    {
        uint i;
        String ret = "";

        for (i = 0; i != amount_bytes; i++)
        {
            ret += buffer[i + index].ToString("X2");
        }

        return ret;
    }

    public static String Convert4BytesToVersionString(Byte[] buffer, bool isLSB)
    {
        String ret = "";

        if (isLSB)
        {
            if (buffer[0].ToString() != "0")
            {
                ret += buffer[0].ToString();
            }
            ret += buffer[1].ToString();
            ret += ".";
            ret += buffer[2].ToString();
            ret += buffer[3].ToString();
        }
        else
        {
            if (buffer[3].ToString() != "0")
            {
                ret += buffer[3].ToString();
            }
            ret += buffer[2].ToString();
            ret += ".";
            ret += buffer[1].ToString();
            ret += buffer[0].ToString();
        }

        return ret;
    }


    public static String ConvertUIntToHexVersionString(uint numeric)
    {
        String verString = ConvertUIntToHexString(numeric);
        return verString.Insert((verString.Length - 2), ".");
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
}

