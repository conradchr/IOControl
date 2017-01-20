using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

#if (!__MOBILE__)
using System.Windows.Forms;
#endif

public class DT_StringUtils
{
    const uint RETURN_OK = 0;
    const uint RETURN_ERROR = 1;

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint dt_check_string_is_hostname(String text)
    {
        // RFC 1123
        Regex regex = new Regex(@"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$");

        if (regex.IsMatch(text) == false)
        {
            return RETURN_ERROR;
        }

        return RETURN_OK;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint dt_check_string_is_ip(String text)
    {
        Regex regex = new Regex(@"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$");
        int i;

        if (regex.IsMatch(text) == false)
        {
            //MessageBox.Show("dt_check_string_is_ip - A");
            return RETURN_ERROR;
        }

        String[] split = text.Split('.');

        for (i = 0; i != split.Length; i++)
        {
            if (dt_check_string_is_valid_numeric(split[i], 0, 255) != RETURN_OK)
            {
                //MessageBox.Show("dt_check_string_is_ip - B");
                return RETURN_ERROR;
            }
        }

        return RETURN_OK;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint dt_check_string_is_numeric(String text)
    {
        Regex regex = new Regex(@"^\d+$");

        if (regex.IsMatch(text) == false)
        {
            //MessageBox.Show("dt_check_string_is_numeric - A");
            return RETURN_ERROR;
        }

        return RETURN_OK;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint CheckStringIsHex(String text)
    {
        Regex regex = new Regex(@"\A\b(0[xX])?[0-9a-fA-F]+\b\Z");

        if (regex.IsMatch(text) == false)
        {
            return RETURN_ERROR;
        }

        return RETURN_OK;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static bool CheckStringIsValidUint(String text, uint value_min, uint value_max)
    {
        uint numeric;

        try
        {
            numeric = Convert.ToUInt32(text);
            return ((numeric >= value_min) && (numeric <= value_max));
        }
        catch (Exception)
        {
            return false;
        }
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint dt_check_string_is_valid_numeric(String text, uint value_min, uint value_max)
    {
        uint numeric;

        if (dt_check_string_is_numeric(text) != RETURN_OK)
        {
            //MessageBox.Show("dt_check_string_is_valid_numeric - A");
            return RETURN_ERROR;
        }

        numeric = Convert.ToUInt32(text);

        if (value_min != value_max)
        {
            //MessageBox.Show(numeric.ToString() + " " + text + " " + value_min.ToString() + " " + value_max.ToString());

            if ((numeric >= value_min) && (numeric <= value_max))
            {
                return RETURN_OK;
            }
        }
        else
        {
            //MessageBox.Show(numeric.ToString() + " " + text + " " + value_min.ToString() + " " + value_max.ToString());

            if (numeric == value_min)
            {
                return RETURN_OK;
            }
        }

        //MessageBox.Show("dt_check_string_is_valid_numeric string=" +text + " - B");
        return RETURN_ERROR;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static uint dt_convert_string_to_mac_addr_string(String source, ref String destination)
    {
        StringBuilder temp_mac = new StringBuilder(30);
        int pos;

        if (source.Length != 12)
        {
            return RETURN_ERROR;
        }

        pos = 0;
        temp_mac.Append(source.Substring(pos, 2));
        pos += 2;

        for (int i = 0; i != ((source.Length/2) - 1); i++)
        {
            temp_mac.Append(":");
            temp_mac.Append(source.Substring(pos, 2));
            pos += 2;
        }

        destination = temp_mac.ToString();

        return RETURN_ERROR;
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
}

