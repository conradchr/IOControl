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


namespace DELIB
{
    public partial class DeLibNET
    {
        public static uint DapiSpecialCommandExt(uint handle, uint cmd, uint par1, uint par2, uint par3, ref uint par4, ref uint par5, ref uint par6,
            byte[] buff1, uint buff1_size, byte[] buff2, uint buff2_size, byte[] buff3, uint buff3_size, ref uint buff3_length)
        {

            byte[] buff = new byte[4000];
            int buff_length = 0;
            int buff_cnt = 0;


            /*
            DAPI_HANDLE* DapiHandle;
            DAPI_MODULE_PARAMS* DapiModuleParams;
            char debug_msg[200];
            unsigned char ch;

            // DEBUG
# ifdef DEBUG_OUTPUT
            char msg[200];
#endif

            unsigned char buff[200];
            unsigned long buff_length;
            unsigned long buff_cnt;
            ULONG ret = 0;
            ULONG i;
            ULONG bank;
            ULONG pos;
            ULONGLONG d64;
            ULONG result;

            ULONGLONG handle_ull;

            if (DapiInternHandlePointerGet(handle32, &handle_ull) == RETURN_OK)
            {
                DapiHandle = (DAPI_HANDLE*)handle_ull;
            }
            else
            {
                DapiSetError(DAPI_ERR_GEN_ILLEGAL_HANDLE, 0, NULL, __FILE__, __LINE__);
                return 0;
            }

            DapiModuleParams = (DAPI_MODULE_PARAMS*)DapiHandle->addr_dapi_module_params;

            // DEBUG
# ifdef DEBUG_OUTPUT
            debug_set_function_name("DapiSpecialCommandExt");
            sprintf(debug_msg, "DapiSpecialCommandExt(handle=0x%lx, cmd=0x%lx, par1=0x%lx, par2=0x%lx, par3=0x%lx)", handle32, cmd, par1, par2, par3);
            debug_print(debug_msg);
            sprintf(debug_msg, "                     (par4=%lx, par5=%lx, par6=%lx, buff1_size=%ld, buff2_size=%ld, buff3_size=%ld)", (unsigned long) par4, (unsigned long) par5, (unsigned long) par6, buff1_size, buff2_size, buff3_size);
            debug_print(debug_msg);

            strcpy_n(msg, sizeof(msg), (char*)buff1, buff1_size);
            sprintf(debug_msg, "buff1_size=%ld buff1=%s", buff1_size, msg);
            debug_print(debug_msg);

            strcpy_n(msg, sizeof(msg), (char*)buff2, buff2_size);
            sprintf(debug_msg, "buff2_size=%ld buff2=%s", buff2_size, msg);
            debug_print(debug_msg);
#endif
            // -----
            */
            uint ret = DAPI_ERR_NONE;

            switch (cmd)
            {

                // ------------------------------------
                // ------------------------------------
                // ------------------------------------
                /*
                case DAPI_SPECIAL_CMDEXT_SET_RTC:
                    // par4: unix timestamp (LSB)

                    if ((DapiModuleParams->tcp_feature1 & DAPI_TCP_FEATURE_BIT_SUPP_RTC) == 0)
                    {
                        debug_error("DapiSpecialCommandExt->DAPI_SPECIAL_CMDEXT_SET_RTC: Module does not support this command");
                        ret = RETURN_ERROR;
                        break;
                    }

                    debug_info("DAPI_SPECIAL_CMDEXT_SET_RTC");
                    buff_cnt = 0;

                    buff[buff_cnt++] = (unsigned char) ((*par4 >> 24) & 0xff);
                    buff[buff_cnt++] = (unsigned char) ((*par4 >> 16) & 0xff);
                    buff[buff_cnt++] = (unsigned char) ((*par4 >> 8) & 0xff);
                    buff[buff_cnt++] = (unsigned char) ((*par4 >> 0) & 0xff);

                    ret = DapiTCPSpecialCmd(handle32, DEDITEC_TCPSPECIAL_CMD_SET_RTC, buff, buff_cnt, buff, sizeof(buff), &buff_length);
                    break;

                // ------------------------------------
                // ------------------------------------
                // ------------------------------------

                case DAPI_SPECIAL_CMDEXT_GET_RTC:
                    // par4: unix timestamp (LSB)

                    debug_info("DAPI_SPECIAL_CMDEXT_GET_RTC");
                    buff_cnt = 0;
                    buff[0] = 0xaa;

                    if ((DapiModuleParams->tcp_feature1 & DAPI_TCP_FEATURE_BIT_SUPP_RTC) == 0)
                    {
                        debug_error("DapiSpecialCommandExt->DAPI_SPECIAL_CMDEXT_GET_RTC: Module does not support this command");
                        ret = RETURN_ERROR;
                        break;
                    }

                    *par4 = 0;
                    if ((ret = DapiTCPSpecialCmd(handle32, DEDITEC_TCPSPECIAL_CMD_GET_RTC, buff, 1, buff, sizeof(buff), &buff_length)) == DAPI_ERR_NONE)
                    {
                        *par4 |= (*(buff + buff_cnt++) << 24);
                        *par4 |= (*(buff + buff_cnt++) << 16);
                        *par4 |= (*(buff + buff_cnt++) << 8);
                        *par4 |= (*(buff + buff_cnt++) << 0);
                    }
                    break;

                // ------------------------------------
                // ------------------------------------
                // ------------------------------------

                case DAPI_SPECIAL_CMDEXT_READ_EE_DIRECTORY:
                    // par1 subModuleID
                    // par2 eeprom directory
                    // buff3 dir inhalt

                    // buffer lesen DEDITEC_TCPSPECIAL_CMD_READ_EE_DIRECTORY
                    // 1b subID
                    // 1b fs_dir

                    debug_print("DAPI_SPECIAL_CMDEXT_READ_EE_DIRECTORY");
                    buff_cnt = 0;

                    if ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_EEPROM_FS_SUPPORT) == 0)
                    {
                        debug_error("DapiSpecialCommandExt->DAPI_SPECIAL_CMDEXT_READ_EE_DIRECTORY: Module does not support this command");
                        ret = RETURN_ERROR;
                        break;
                    }

                    switch (DapiInternGetBusType(DapiHandle->moduleID))
                    {
                        // ------------------------------------
                        case bus_ETH:
                            //debug_print("BusType = bus_ETH");

                            buff[buff_cnt++] = (unsigned char) (par1 & 0xff);
                            buff[buff_cnt++] = (unsigned char) (par2 & 0xff);

                            if ((ret = DapiTCPSpecialCmd(handle32, DEDITEC_TCPSPECIAL_CMD_READ_EE_DIRECTORY, buff, buff_cnt, buff, sizeof(buff), &buff_length)) == DAPI_ERR_NONE)
                            {
                                // abfrage ok
                                ret = buffcpy_n(buff3, buff3_size, buff, buff_length);
                            }
                            break;
                        // ------------------------------------
                        default:
                            debug_error("DapiSpecialCommandExt->DAPI_SPECIAL_CMDEXT_READ_EE_DIRECTORY: Unknown BusType");
                            ret = RETURN_ERROR;
                            break;
                            // ------------------------------------
                    }

                    if (buff3_length != NULL)
                    {
                        *buff3_length = ((ret == DAPI_ERR_NONE) ? buff_length : 0);
                    }

                    break;

                // ------------------------------------
                // ------------------------------------
                // ------------------------------------

                case DAPI_SPECIAL_CMDEXT_WRITE_EE_DIRECTORY:
                    // par1 subModuleID
                    // par2 eeprom directory id
                    // par3 option flag - z.b. DT_EE_FS_FLAG_CREATE_DIR_IF_NOT_EXIST
                    // buff1 content

                    // buffer schreiben DEDITEC_TCPSPECIAL_CMD_WRITE_EE_DIRECTORY
                    // 1b subModuleID
                    // 1b eeprom directory id
                    // 1b option flag - z.b. DT_EE_FS_FLAG_CREATE_DIR_IF_NOT_EXIST
                    // 2b content length
                    // xb content

                    debug_print("DAPI_SPECIAL_CMDEXT_WRITE_EE_DIRECTORY");
                    buff_cnt = 0;

                    if ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_EEPROM_FS_SUPPORT) == 0)
                    {
                        debug_error("DapiSpecialCommandExt->DAPI_SPECIAL_CMDEXT_WRITE_EE_DIRECTORY: Module does not support this command");
                        ret = RETURN_ERROR;
                        break;
                    }

                    switch (DapiInternGetBusType(DapiHandle->moduleID))
                    {
                        // ------------------------------------
                        case bus_ETH:
                            //debug_print("BusType = bus_ETH");

                            buff[buff_cnt++] = (unsigned char) (par1 & 0xff);
                            buff[buff_cnt++] = (unsigned char) (par2 & 0xff);
                            buff[buff_cnt++] = (unsigned char) (par3 & 0xff);
                            buff[buff_cnt++] = (unsigned char) ((buff1_size >> 0) & 0xff);
                            buff[buff_cnt++] = (unsigned char) ((buff1_size >> 8) & 0xff);

# ifdef DEBUG_OUTPUT
                            // DEBUG
                            for (i = 0; i < buff1_size; i += 16)
                            {
                                debug_hexdump_line_to_msg(debug_msg, sizeof(debug_msg), buff1, buff1_size, i, buff1_size);
                                debug_info(debug_msg);
                            }
                            // -----
#endif

                            if ((ret = buffcpy_n(buff + buff_cnt, sizeof(buff) - buff_cnt, buff1, buff1_size)) == DAPI_ERR_NONE)
                            {
                                buff_cnt += buff1_size;
                                ret = DapiTCPSpecialCmd(handle32, DEDITEC_TCPSPECIAL_CMD_WRITE_EE_DIRECTORY, buff, buff_cnt, buff, sizeof(buff), &buff_length);
                            }

                            break;
                        // ------------------------------------
                        default:
                            debug_error("DapiSpecialCommandExt->DAPI_SPECIAL_CMDEXT_WRITE_EE_DIRECTORY: Unknown BusType");
                            ret = RETURN_ERROR;
                            break;
                            // ------------------------------------
                    }

                    if (buff3_length != NULL)
                    {
                        *buff3_length = 0;
                    }

                    break;
                    */
                // ------------------------------------
                // ------------------------------------
                // ------------------------------------
                
                case DT.DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_CONFIG_DATA:
                    // 17.03.2016 - neu:
                    // par2: multiple_flag
                    // wenn flag da ist, wird der buff1 1:1 durchgeschleift - muss also selbst vorher zusammen gebaut werden
                    // andernfalls wird die anfrage als single behandelt

                    // par1: Nummer der config
                    // buff1 + buff1_size:	Namen zum lesen/schreiben
                    // buff2 + buff2_size:	Wert zum schreiben (nur beim write)
                    // buff3 + buff3_size:	Antwort die zurückgegeben wird
                    // *buff3_length		Grösse des zurückgegebenen Wertes

                    //debug_print("DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_CONFIG_DATA");
                    DT.Log("DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_CONFIG_DATA");


                    /*
                    if ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_SUPP_INDIVIDUAL_CH_NAMES) == 0)
                    {
                        //debug_error("DapiSpecialCommandExt->DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_CONFIG_DATA: Module does not support this command");
                        ret = RETURN_ERROR;
                        break;
                    }
                    */

                    buff_cnt = 0;

                    if (par2 == 1)
                    {
                        try
                        {
                            DT.Log("multiple mode");
                            Array.Copy(buff1, buff, (int) buff1_size);
                            buff_cnt = (int) buff1_size;
                        }
                        catch (Exception)
                        {
                            DT.Log("Array.Copy buff1->buff");
                            return DT.RETURN_ERROR;
                        }
                    }
                    else
                    {
                        // single mode
                        DT.Log("single mode");
                        // Sende Nummer der Konfiguration (bei durchnummerierten)
                        buff[buff_cnt++] = (byte)((par1) & 0xff);
                        buff[buff_cnt++] = (byte)((par1 >> 8) & 0xff);
                        buff[buff_cnt++] = (byte)((par1 >> 16) & 0xff);
                        buff[buff_cnt++] = (byte)((par1 >> 24) & 0xff);

                        // Sende Key-länge
                        buff[buff_cnt++] = (byte)((buff1_size) & 0xff);
                        buff[buff_cnt++] = (byte)((buff1_size >> 8) & 0xff);
                        buff[buff_cnt++] = (byte)((buff1_size >> 16) & 0xff);
                        buff[buff_cnt++] = (byte)((buff1_size >> 24) & 0xff);
                        // Sende-Daten

                        try
                        {
                            Array.Copy(buff1, 0, buff, buff_cnt, (int)buff1_size);
                            buff_cnt += (int)buff1_size;
                        }
                        catch (Exception)
                        {
                            DT.Log("Array.Copy buff1->buff");
                            return DT.RETURN_ERROR;
                        }
                    }

                    buff_length = 0;
                    ret = DT.TCP.DapiTCPSpecialCmd(handle, DT.DEDITEC_TCPSPECIAL_CMD_PARAMETER_READ, buff, buff_cnt, buff, buff.Length, ref buff_length);
                    if (ret == DAPI_ERR_NONE)
                    {
                        // die antwort hat immer 4B vallen + nB val
                        // bei multiple werden diese längen mit durchgeschleift
                        // bei single wird das abgeschnitten und direkt als string kopiert
                        if (par2 == 1)
                        {
                            try
                            {
                                Array.Copy(buff, buff3, buff_length);
                                buff3_length = (uint) buff_length;
                            }
                            catch (Exception)
                            {
                                DT.Log("Array.Copy buff->buff3");
                                return DT.RETURN_ERROR;
                            }
                        }
                        else
                        {
                            DT.Log(" *********** NICHT IMPLEMENTIERT ************");

                            /*
                            // single mode
                            ret = strcpy_n((char*)buff3, buff3_size, (char*)buff + 4, buff_length - 4);
                            *buff3_length = buff_length - 4;
                            */
                        }
                    }

                    // DEBUG

                    /*
                    sprintf(debug_msg, "buff3_len=%ld size=*par6=%ld buff3=%s", buff3_size, *buff3_length, buff3);
                    debug_print(debug_msg);
                    */


                    break;

                // ------------------------------------
                // ------------------------------------
                // ------------------------------------
                case DT.DAPI_SPECIAL_CMDEXT_TCP_MODULE_SET_CONFIG_DATA:
                    // 17.03.2016 - neu:
                    // par2: multiple_flag
                    // wenn flag da ist, wird der buff1 1:1 durchgeschleift - muss also selbst vorher zusammen gebaut werden
                    // andernfalls wird die anfrage als single behandelt

                    // par1: Nummer der config
                    // buff1 + buff1_size:	Namen zum lesen/schreiben
                    // buff2 + buff2_size:	Wert zum schreiben (nur beim write)
                    // buff3 + buff3_size:	Antwort die zurückgegeben wird
                    // *buff3_length		Grösse des zurückgegebenen Wertes

                    DT.Log("DAPI_SPECIAL_CMDEXT_TCP_MODULE_SET_CONFIG_DATA");

                    /*
                    if ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_SUPP_INDIVIDUAL_CH_NAMES) == 0)
                    {
                        debug_error("DapiSpecialCommandExt->DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_CONFIG_DATA: Module does not support this command");
                        ret = RETURN_ERROR;
                        break;
                    }
                    */

                    buff_cnt = 0;

                    if (par2 == 1)
                    {
                        // muliple mode
                        try
                        {
                            DT.Log("multiple mode");
                            Array.Copy(buff1, buff, (int)buff1_size);
                            buff_cnt = (int)buff1_size;
                        }
                        catch (Exception)
                        {
                            DT.Log("Array.Copy buff1->buff");
                            return DT.RETURN_ERROR;
                        }
                    }
                    else
                    {
                        // single mode
                        DT.Log("single mode");
                        // Sende Nummer der Konfiguration (bei durchnummerierten)
                        buff[buff_cnt++] = (byte)((par1) & 0xff);
                        buff[buff_cnt++] = (byte)((par1 >> 8) & 0xff);
                        buff[buff_cnt++] = (byte)((par1 >> 16) & 0xff);
                        buff[buff_cnt++] = (byte)((par1 >> 24) & 0xff);

                        // Sende Key-länge
                        buff[buff_cnt++] = (byte)((buff1_size) & 0xff);
                        buff[buff_cnt++] = (byte)((buff1_size >> 8) & 0xff);
                        buff[buff_cnt++] = (byte)((buff1_size >> 16) & 0xff);
                        buff[buff_cnt++] = (byte)((buff1_size >> 24) & 0xff);
                        // Sende-Daten
                        try
                        {
                            Array.Copy(buff1, 0, buff, buff_cnt, (int)buff1_size);
                            buff_cnt += (int)buff1_size;
                        }
                        catch (Exception)
                        {
                            DT.Log("Array.Copy buff1->buff");
                            return DT.RETURN_ERROR;
                        }

                        // Sende Daten-länge
                        buff[buff_cnt++] = (byte)((buff2_size) & 0xff);
                        buff[buff_cnt++] = (byte)((buff2_size >> 8) & 0xff);
                        buff[buff_cnt++] = (byte)((buff2_size >> 16) & 0xff);
                        buff[buff_cnt++] = (byte)((buff2_size >> 24) & 0xff);
                        // Sende-Daten
                        try
                        {
                            Array.Copy(buff2, 0, buff, buff_cnt, (int)buff2_size);
                            buff_cnt += (int)buff2_size;
                        }
                        catch (Exception)
                        {
                            DT.Log("Array.Copy buff2->buff");
                            return DT.RETURN_ERROR;
                        }
                    }


                    /*
                    //unsigned long i;
                    //char debug_msg[128];
                    for (i = 0; i < buff_cnt; i += 16)
                    {
                        debug_hexdump_line_to_msg(debug_msg, sizeof(debug_msg), buff, sizeof(buff), i, buff_cnt);
                        debug_info("%s", debug_msg);
                    }
                    */


                    if (ret == DAPI_ERR_NONE)
                    {
                        buff_length = 0;
                        ret = DT.TCP.DapiTCPSpecialCmd(handle, DT.DEDITEC_TCPSPECIAL_CMD_PARAMETER_WRITE, buff, buff_cnt, buff, buff.Length, ref buff_length);
                        buff3_length = 0;
                    }

                    /*
                    // DEBUG
# ifdef DEBUG_OUTPUT
                    sprintf(debug_msg, "buff3_len=%ld size=*par6=%ld buff3=%s", buff3_size, *buff3_length, buff3);
                    debug_print(debug_msg);
#endif
*/

                    break;

                    // ------------------------------------
                    // ------------------------------------
                    // ------------------------------------
                    /*
                    case DAPI_SPECIAL_CMDEXT_TCP_MODULE_IDENTIFY:
                        // cmd = DEDITEC_TCPSPECIAL_CMD_IDENTIFY
                        // par1 = Anzahl Blink-Sequenzen			2Byte
                        // par2 = Blink-Sequenz high Zeit [ms]		2Byte
                        // par3 = Blink-Sequenz low Zeit [ms]		2Byte
                        // kein return
                        // verschlüsselung: egal!

                        sprintf(debug_msg, "DAPI_SPECIAL_CMDEXT_TCP_MODULE_IDENTIFY");
                        debug_print(debug_msg);

                        buff_cnt = 0;

                        // Anzahl Blink-Sequenzen
                        buff[buff_cnt++] = (unsigned char) ((par1 >> 8) & 0xff);
                        buff[buff_cnt++] = (unsigned char) ((par1) & 0xff);

                        // Blink-Sequenz high Zeit [ms]
                        buff[buff_cnt++] = (unsigned char) ((par2 >> 8) & 0xff);
                        buff[buff_cnt++] = (unsigned char) ((par2) & 0xff);

                        // Blink-Sequenz low Zeit [ms]
                        buff[buff_cnt++] = (unsigned char) ((par3 >> 8) & 0xff);
                        buff[buff_cnt++] = (unsigned char) ((par3) & 0xff);

                        ret = DapiTCPSpecialCmd(handle32, DEDITEC_TCPSPECIAL_CMD_IDENTIFY, buff, buff_cnt, buff, sizeof(buff), &buff_length);
                        break;

                    // ------------------------------------
                    // ------------------------------------
                    // ------------------------------------

                    case DAPI_SPECIAL_CMDEXT_TCP_MODULE_ACTIVATE_ADMIN_TEMP:
                        // cmd = DEDITEC_TCPSPECIAL_CMD_ACTIVATE_ADMIN_TEMP
                        // aktiviert "blink1" + taster auf modul
                        // kein return
                        // verschlüsselung: egal!

                        sprintf(debug_msg, "DAPI_SPECIAL_CMDEXT_TCP_MODULE_ACTIVATE_ADMIN_TEMP");
                        debug_print(debug_msg);

                        buff_cnt = 0;
                        ret = DapiTCPSpecialCmd(handle32, DEDITEC_TCPSPECIAL_CMD_ACTIVATE_ADMIN_TEMP, buff, buff_cnt, buff, sizeof(buff), &buff_length);
                        break;

                    // ------------------------------------
                    // ------------------------------------
                    // ------------------------------------

                    case DAPI_SPECIAL_CMDEXT_TCP_MODULE_RELOAD_ETH_CONFIG:
                        // cmd = DEDITEC_TCPSPECIAL_CMD_RELOAD_ETH_CONFIG
                        // macht das selbe wie configure eth0 aus dem broadcast teil
                        // kein return
                        // NUR MIT ADMIN ODER ADMIN_TEMP Verschlüsselung

                        sprintf(debug_msg, "DAPI_SPECIAL_CMDEXT_TCP_MODULE_RELOAD_ETH_CONFIG");
                        debug_print(debug_msg);

                        buff_cnt = 0;
                        ret = DapiTCPSpecialCmd(handle32, DEDITEC_TCPSPECIAL_CMD_RELOAD_ETH_CONFIG, buff, buff_cnt, buff, sizeof(buff), &buff_length);
                        break;

                    // ------------------------------------
                    // ------------------------------------
                    // ------------------------------------

                    case DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_CURRENT_CONFIG:
                        // cmd = DEDITEC_TCPSPECIAL_CMD_GET_CURRENT_CONFIG
                        // gibt einen buffer mit der aktuell benutzten TCP-Config, DIP-Schalter und Verschlüsselungsflags zurück
                        // return: byte buffer
                        // verschlüsselung: egal!

                        sprintf(debug_msg, "DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_CURRENT_CONFIG");
                        debug_print(debug_msg);

                        buff_cnt = 0;
                        ret = DapiTCPSpecialCmd(handle32, DEDITEC_TCPSPECIAL_CMD_GET_CURRENT_CONFIG, buff, buff_cnt, buff, sizeof(buff), &buff_length);

                        *buff3_length = buff_length;
                        buffcpy_n(buff3, buff3_size, buff, buff_length);

                        break;

                    // ------------------------------------
                    // ------------------------------------
                    // ------------------------------------

                    case DAPI_SPECIAL_CMDEXT_TCP_MODULE_SET_MAC_ADDR:
                        // cmd = DEDITEC_TCPSPECIAL_CMD_SET_MAC_ADDR
                        // setzt MAC-Addr
                        // verschlüsselung: egal!
                        // kein return

                        sprintf(debug_msg, "DAPI_SPECIAL_CMDEXT_TCP_MODULE_SET_MAC_ADDR");
                        debug_print(debug_msg);

                        ret = DapiTCPSpecialCmd(handle32, DEDITEC_TCPSPECIAL_CMD_SET_MAC_ADDR, buff1, buff1_size, buff, sizeof(buff), &buff_length);
                        break;

                    // ------------------------------------
                    // ------------------------------------
                    // ------------------------------------

                    case DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_MAC_ADDR:
                        // cmd = DEDITEC_TCPSPECIAL_CMD_GET_MAC_ADDR
                        // holt MAC-Addr
                        // verschlüsselung: egal!
                        // kein return

                        sprintf(debug_msg, "DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_MAC_ADDR");
                        debug_print(debug_msg);

                        buff_cnt = 0;
                        ret = DapiTCPSpecialCmd(handle32, DEDITEC_TCPSPECIAL_CMD_GET_MAC_ADDR, buff, buff_cnt, buff, sizeof(buff), &buff_length);

                        *buff3_length = buff_length;
                        buffcpy_n(buff3, buff3_size, buff, buff_length);
                        break;

                    // ------------------------------------
                    // ------------------------------------
                    // ------------------------------------

                    case DAPI_SPECIAL_CMDEXT_DELIB_SET_PROG_NAME:
                        debug_set_function_name("DapiSpecialCommand");
                        sprintf(debug_msg, "DAPI_SPECIAL_CMDEXT_DELIB_SET_PROG_NAME ProgName=%s", buff1);
                        debug_print(debug_msg);

                        i = 0;
                        do
                        {
                            ch = (unsigned char) *(buff1 + i);
                            DapiHandle->ProgramName[i] = ch;
                            ++i;
                        } while ((ch != 0) && (i < 12));
                        --i;

                        // KEINE Terminierung - rest muss aus Leerzeichen bestehen !
                        while (i < 12)
                        {
                            DapiHandle->ProgramName[i] = ' ';
                            ++i;
                        }
                        DapiHandle->ProgramName[11] = 0;    // Max 12 Zeichen (11 char + Terminierung)

                        break;

                    // ------------------------------------
                    // ------------------------------------
                    // ------------------------------------

                    case DAPI_SPECIAL_CMDEXT_ROCPU_SUBMODULE_GET_INFOS:
                        sprintf(debug_msg, "DAPI_SPECIAL_CMDEXT_ROCPU_SUBMODULE_GET_INFOS-Start");
                        debug_print(debug_msg);

                        for (i = 0; i != 8; ++i) *(buff1 + i) = 0;

                        DapiWriteWord(handle32, 0xfe00, par1 & 0xff | (74 << 8));           // Submodule-Nr + Command: Submodule Get Infos

                        DapiReadMultipleBytes(handle32, 0xfe80, 8, 1, buff1, 8);    // read 8 Bytes

                        sprintf(debug_msg, "DAPI_SPECIAL_CMDEXT_ROCPU_SUBMODULE_GET_INFOS: SUBMODULE = %2d  * data = %2x %2x %2x %2x %2x %2x %2x %2x", par1, *(buff1 + 0), *(buff1 + 1), *(buff1 + 2), *(buff1 + 3), *(buff1 + 4), *(buff1 + 5), *(buff1 + 6), *(buff1 + 7));
                        debug_print(debug_msg);

                        break;

                    // ------------------------------------
                    // ------------------------------------
                    // ------------------------------------

                    case DAPI_SPECIAL_CMDEXT_NETCPU_SUBMODULE_GET_INFOS:
                        sprintf(debug_msg, "DAPI_SPECIAL_CMDEXT_NETCPU_SUBMODULE_GET_INFOS-Start");
                        debug_print(debug_msg);

                        for (i = 0; i != 32; ++i) *(buff1 + i) = 0;

                        DapiWriteWord(handle32, 0xfe00, par1 & 0xff | (74 << 8));           // Submodule-Nr + Command: Submodule Get Infos

                        DapiReadMultipleBytes(handle32, 0xfe80, 32, 1, buff1, 32);  // read 8 Bytes

                        sprintf(debug_msg, "DAPI_SPECIAL_CMDEXT_NETCPU_SUBMODULE_GET_INFOS: SUBMODULE = %2d  * data = %2x %2x %2x %2x %2x %2x %2x %2x", par1, *(buff1 + 0x00), *(buff1 + 0x01), *(buff1 + 0x02), *(buff1 + 0x03), *(buff1 + 0x04), *(buff1 + 0x05), *(buff1 + 0x06), *(buff1 + 0x07));
                        debug_print(debug_msg);
                        sprintf(debug_msg, "DAPI_SPECIAL_CMDEXT_NETCPU_SUBMODULE_GET_INFOS: SUBMODULE = %2d  * data = %2x %2x %2x %2x %2x %2x %2x %2x", par1, *(buff1 + 0x08), *(buff1 + 0x09), *(buff1 + 0x0a), *(buff1 + 0x0b), *(buff1 + 0x0c), *(buff1 + 0x0d), *(buff1 + 0x0e), *(buff1 + 0x0f));
                        debug_print(debug_msg);
                        sprintf(debug_msg, "DAPI_SPECIAL_CMDEXT_NETCPU_SUBMODULE_GET_INFOS: SUBMODULE = %2d  * data = %2x %2x %2x %2x %2x %2x %2x %2x", par1, *(buff1 + 0x10), *(buff1 + 0x11), *(buff1 + 0x12), *(buff1 + 0x13), *(buff1 + 0x14), *(buff1 + 0x15), *(buff1 + 0x16), *(buff1 + 0x17));
                        debug_print(debug_msg);
                        sprintf(debug_msg, "DAPI_SPECIAL_CMDEXT_NETCPU_SUBMODULE_GET_INFOS: SUBMODULE = %2d  * data = %2x %2x %2x %2x %2x %2x %2x %2x", par1, *(buff1 + 0x18), *(buff1 + 0x19), *(buff1 + 0x1a), *(buff1 + 0x1b), *(buff1 + 0x1c), *(buff1 + 0x1d), *(buff1 + 0x1e), *(buff1 + 0x1f));
                        debug_print(debug_msg);

                        break;

                    // ------------------------------------
                    // ------------------------------------
                    // ------------------------------------

                    case DAPI_SPECIAL_CMDEXT_ROCPU_EEPROM_READ_1K_2K:
                        // module supports new Software-Features ?
                        if ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_SUPPORTED_BY_FIRMWARE) != 0)
                        {
                            // module has DAPI_SW_FEATURE_BIT_EEPROM_RN23 ?
                            if ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_EEPROM_RN23) != 0)
                            {

                                // read 1k/2k from EEPROM
                                // par1 = 1=first 1k, 2=second1k, 3 = 2K EEPROM
                                // buff1 = Buffer

                                pos = 0;


                                for (bank = 0; bank <= 1; ++bank)
                                {
                                    if ((par1 & (bank + 1)) != 0)
                                    {
                                        //read first 1k
                                        for (ULONG j = (bank * 4); j != ((bank + 1) * 4); ++j)
                                        {
                                            // read 256 Byte from EEPROM
                                            DapiWriteWord(handle32, 0xfdf0, (j) & 7 | 0x01 << 8);       // EEPROM Read
                                            if (DapiGetLastError() != 0) return 0;

                                            for (i = 0; i != 256; i += 8)
                                            {
                                                /*ret=DapiReadByte(handle, 0xf000+i);
                                                if(DapiGetLastError() != 0) return 0;
                                                *(buff1+pos) = (unsigned char) ret;
                                                ++pos;*/


                    /*
                                            d64 = DapiReadLongLong(handle32, 0xf000 + i);
                                            if (DapiGetLastError() != 0) return 0;
                                            *(buff1 + pos + 0) = (unsigned char)   d64 & 0x00000000000000ff;
                            *(buff1 + pos + 1) = (unsigned char) ((d64 & 0x000000000000ff00) >> 8);
                            *(buff1 + pos + 2) = (unsigned char) ((d64 & 0x0000000000ff0000) >> 16);
                            *(buff1 + pos + 3) = (unsigned char) ((d64 & 0x00000000ff000000) >> 24);
                            *(buff1 + pos + 4) = (unsigned char) ((d64 & 0x000000ff00000000) >> 32);
                            *(buff1 + pos + 5) = (unsigned char) ((d64 & 0x0000ff0000000000) >> 40);
                            *(buff1 + pos + 6) = (unsigned char) ((d64 & 0x00ff000000000000) >> 48);
                            *(buff1 + pos + 7) = (unsigned char) ((d64 & 0xff00000000000000) >> 56);
                            pos += 8;
                        }
                    }
            }
        }
    }
				// module has DAPI_SW_FEATURE_BIT_EEPROM_E2_2K ?
				else if ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_EEPROM_E2_2K) != 0)
				{
		
					// read 1k/2k from EEPROM
					// par1 = 1=first 1k, 2=second1k, 3 = 2K EEPROM
					// buff1 = Buffer

					pos=0;


					for(bank=0;bank<=1;++bank)
					{
						if((par1 & (bank+1))!=0)
						{
							//read first 1k
							for (ULONG j = (bank * 16); j!=((bank+1)*16);++j)
							{
                                // read 64 Byte from EEPROM
                                DapiWriteWord(handle32, 0xfdf0, (j&31) | (0x05<<8));		// EEPROM Read
								if(DapiGetLastError() != 0) return 0;

								for(i=0;i!=64;i+=8)
								{
									d64=DapiReadLongLong(handle32, 0xf000+i);
									if(DapiGetLastError() != 0) return 0;

                                    *(buff1+pos+0) = (unsigned char)   d64 & 0x00000000000000ff;

                                    *(buff1+pos+1) = (unsigned char) ((d64 & 0x000000000000ff00) >> 8);

                                    *(buff1+pos+2) = (unsigned char) ((d64 & 0x0000000000ff0000) >> 16);

                                    *(buff1+pos+3) = (unsigned char) ((d64 & 0x00000000ff000000) >> 24);

                                    *(buff1+pos+4) = (unsigned char) ((d64 & 0x000000ff00000000) >> 32);

                                    *(buff1+pos+5) = (unsigned char) ((d64 & 0x0000ff0000000000) >> 40);

                                    *(buff1+pos+6) = (unsigned char) ((d64 & 0x00ff000000000000) >> 48);

                                    *(buff1+pos+7) = (unsigned char) ((d64 & 0xff00000000000000) >> 56);
									pos+=8;
								}
							}
						}
					}
				}
				else
				{

                    sprintf(debug_msg, "DapiSpecialCommand(handle, DAPI_SPECIAL_CMD_ROCPU_EEPROM_READ..../DAPI_SW_FEATURE_BIT_EEPROM_RN23) -> Module does not support this command");

                    DapiSetError(DAPI_ERR_GEN_NOT_SUPPORTED_IO_TYPE, 0, debug_msg, __FILE__, __LINE__);
					return RETURN_ERROR;
				}
			}
			break;

		// ------------------------------------
		// ------------------------------------
		// ------------------------------------
		
		case DAPI_SPECIAL_CMDEXT_ROCPU_EEPROM_WRITE_1K_2K:
			// module supports new Software-Features ?
			if ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_SUPPORTED_BY_FIRMWARE) != 0)	
			{
				// module has DAPI_SW_FEATURE_BIT_EEPROM_RN23 ?
				if ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_EEPROM_RN23) != 0)
				{
			
					// write 1k/2k to EEPROM
					// par1 = 1=first 1k, 2=second1k, 3 = 2K EEPROM
					// buff1 = Buffer

					pos=0;	// Start at beginning of the buffer

					for(bank=0;bank<=1;++bank)
					{
						if((par1 & (bank+1))!=0) // &1  oder &2
						{
							//write first 1k
							ULONG bank_start = bank * 4;
ULONG bank_end = (bank + 1) * 4;


							for (ULONG j = bank_start; j!=(bank_end);++j)	// 0-3 oder 4-7
							{

                                DapiWriteMultipleBytes(handle32, 0xf000 , 64, 1, buff1+pos , 64); pos+=64;

                                DapiWriteMultipleBytes(handle32, 0xf040 , 64, 1, buff1+pos , 64); pos+=64;

                                DapiWriteMultipleBytes(handle32, 0xf080 , 64, 1, buff1+pos , 64); pos+=64;

                                DapiWriteMultipleBytes(handle32, 0xf0c0 , 64, 1, buff1+pos , 64); pos+=64;


                                sprintf(debug_msg,"DAPI_SPECIAL_CMD_ROCPU_EEPROM_WRITE: Write State 1");

                                debug_print(debug_msg);

                                // write 256 Byte to EEPROM
                                DapiWriteWord(handle32, 0xfdf0, (j)&7 | 0x02<<8);		// EEPROM Write


                                sprintf(debug_msg,"DAPI_SPECIAL_CMD_ROCPU_EEPROM_WRITE: Write State 2");

                                debug_print(debug_msg);

                                // Verify Data
                                DapiWriteByte(handle32, 0xfdf1, 0x03);      // Verify Write
result = DapiReadByte(handle32, 0xfdf2);		// Get Status


                                sprintf(debug_msg,"DAPI_SPECIAL_CMD_ROCPU_EEPROM_WRITE (RN23): Write State 3-result=%ld bank=%ld j=%ld", result, bank, j);

                                debug_print(debug_msg);

								if(result==0)
								{
									ret = 0;
								}
								else
								{
									ret = -1;
									return ret;
								}
							}
						}
					}
				}
				// module has DAPI_SW_FEATURE_BIT_EEPROM_E2_2K ?
				else if ((DapiModuleParams->sw_feature1 & DAPI_SW_FEATURE_BIT_EEPROM_E2_2K) != 0)
				{
			
					// write 1k/2k to EEPROM
					// par1 = 1=first 1k, 2=second1k, 3 = 2K EEPROM
					// buff1 = Buffer

					pos=0;	// Start at beginning of the buffer

					for(bank=0;bank<=1;++bank)
					{
						if((par1 & (bank+1))!=0) // &1  oder &2
						{
							//write first 1k
							ULONG bank_start = bank * 16;
ULONG bank_end = (bank + 1) * 16;

							for (ULONG j = bank_start; j!=(bank_end);++j)	// 0-15 oder 16-31
							{

                                DapiWriteMultipleBytes(handle32, 0xf000 , 64, 1, buff1+pos , 64);
pos+=64;


                                sprintf(debug_msg,"DAPI_SPECIAL_CMD_ROCPU_EEPROM_WRITE: Write State 1");

                                debug_print(debug_msg);

                                // write 64 Byte to EEPROM
                                DapiWriteWord(handle32, 0xfdf0, (j&31) | (0x06<<8));		// EEPROM Write


                                sprintf(debug_msg,"DAPI_SPECIAL_CMD_ROCPU_EEPROM_WRITE: Write State 2");

                                debug_print(debug_msg);

                                // Verify Data
                                DapiWriteByte(handle32, 0xfdf1, 0x07);      // Verify Write
result = DapiReadByte(handle32, 0xfdf2);		// Get Status


                                sprintf(debug_msg,"DAPI_SPECIAL_CMD_ROCPU_EEPROM_WRITE (E2_2K): Write State 3-result=%ld bank=%ld j=%ld", result, bank, j);

                                debug_print(debug_msg);

								if(result==0)
								{
									ret = 0;
								}
								else
								{
									ret = -1;
									return ret;
								}
							}
						}
					}

				}
				else
				{

                    sprintf(debug_msg, "DapiSpecialCommand(handle, DAPI_SPECIAL_CMD_ROCPU_EEPROM_WRITE....) -> Module does not support this command");

                    DapiSetError(DAPI_ERR_GEN_NOT_SUPPORTED_IO_TYPE, 0, debug_msg, __FILE__, __LINE__);
					return RETURN_ERROR;
				}
			}

   			break;

		// ------------------------------------
		// ------------------------------------
		// ------------------------------------

		case DAPI_SPECIAL_CMDEXT_CALL_SPECIAL_CMD_TYP_BUFF:
            //DAPI_FUNCTION_PRE64 ULONG DAPI_FUNCTION_PRE DapiSpecialCommandExt(
            //ULONG handle, ULONG cmd, 
            //ULONG par1, ULONG par2, ULONG par3, 
            //ULONG *par4, ULONG *par5, ULONG *par6, 
            //UCHAR *buff1, ULONG buff1_size, 
            //UCHAR *buff2, ULONG buff2_size, 
            //UCHAR *buff3, ULONG buff3_size, ULONG *buff3_length)

            DapiSpecialCommandXXX(handle32, par1, par2, par3, (unsigned long) buff1);
			break;
	
		// ------------------------------------
		// ------------------------------------
		// ------------------------------------
	}

    // DEBUG
# ifdef DEBUG_OUTPUT
        debug_set_function_name("DapiSpecialCommandExt");

        sprintf(debug_msg, "return=0x%lx", ret);

        debug_print(debug_msg);
	#endif
	// -----
	*/

            } // switch

            return ret;
        }
    }
}
