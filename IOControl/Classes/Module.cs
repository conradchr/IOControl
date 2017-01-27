using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization; // XML
using Xamarin.Forms;

namespace IOControl
{
    public class ModuleTask
    {
        public IOType ioType;
        public List<ModuleTaskParam> param;
        public Module module;
        public Dictionary<int, View> dict;

        public ModuleTask(IOType ioType, Module module, Dictionary<int, View> dict)
        {
            this.ioType = ioType;
            this.module = module;
            this.dict = dict;

            param = new List<ModuleTaskParam>();
        }
    }

    // --------------------
    // --------------------
    // --------------------

    public class ModuleTaskParam
    {
        public uint ch;
        public int id;

        public ModuleTaskParam(uint ch, int viewId)
        {
            this.ch = ch;
            this.id = viewId;
        }
    }

    // --------------------
    // --------------------
    // --------------------

    public class Module
    {
        public String boardname;
        public String tcp_hostname;
        public int tcp_port;
        public int tcp_timeout;
        //public long id;
        public String pw_encryption;
        public String mac;

        [XmlIgnoreAttribute]
        public bool ioNamesValid = false;

        [XmlIgnoreAttribute]
        public String enc_pw;

        [XmlIgnoreAttribute]
        public uint handle;

        [XmlIgnoreAttribute]
        public String fw_ver;

        [XmlIgnoreAttribute]
        public _IO IO = new _IO();

        [XmlIgnoreAttribute]
        public _IOName IOName = new _IOName();

        [XmlIgnoreAttribute]
        bool ioInit = false;

        [XmlIgnoreAttribute]
        public List<ModuleTask>[] refreshTasks;

        [XmlIgnoreAttribute]
        public List<Action> todoTasks = new List<Action>();

        // --------------------
        // --------------------
        // --------------------

        
        public uint OpenModule()
        {
            return (handle = DT.Delib.DapiOpenModuleEx(tcp_hostname, (uint)tcp_port, (uint)tcp_timeout, enc_pw));
        }

        public void CloseModule()
        {
            DT.Delib.DapiCloseModule(handle);
            handle = 0;
        }
        

        // --------------------
        // --------------------
        // --------------------

        public void IOInit()
        {
            if (!ioInit)
            {
                GetModuleConfig();
                GetIONames();
                ioInit = true;
            }
        }

        // --------------------
        // --------------------
        // --------------------

            
        void GetModuleConfig()
        {
            if (handle != 0)
            { 
                IO.cnt_do = DT.Delib.DapiSpecialCommand(handle, DT.Delib.DAPI_SPECIAL_CMD_GET_MODULE_CONFIG, DT.Delib.DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_DO, 0, 0);
                IO.cnt_di = DT.Delib.DapiSpecialCommand(handle, DT.Delib.DAPI_SPECIAL_CMD_GET_MODULE_CONFIG, DT.Delib.DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_DI, 0, 0);
                IO.cnt_ao = DT.Delib.DapiSpecialCommand(handle, DT.Delib.DAPI_SPECIAL_CMD_GET_MODULE_CONFIG, DT.Delib.DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_DA, 0, 0);
                IO.cnt_ai = DT.Delib.DapiSpecialCommand(handle, DT.Delib.DAPI_SPECIAL_CMD_GET_MODULE_CONFIG, DT.Delib.DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_AD, 0, 0);
                IO.cnt_do_pwm = DT.Delib.DapiSpecialCommand(handle, DT.Delib.DAPI_SPECIAL_CMD_GET_MODULE_CONFIG, DT.Delib.DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_PWM_OUT, 0, 0);
                IO.cnt_temp = DT.Delib.DapiSpecialCommand(handle, DT.Delib.DAPI_SPECIAL_CMD_GET_MODULE_CONFIG, DT.Delib.DAPI_SPECIAL_GET_MODULE_CONFIG_PAR_TEMP, 0, 0);

                fw_ver = DT.Delib.DapiSpecialCommand(handle, DT.Delib.DAPI_SPECIAL_CMD_GET_MODULE_VERSION, DT.Delib.DAPI_SPECIAL_GET_MODULE_PAR_VERSION_0, 0, 0)
                          + DT.Delib.DapiSpecialCommand(handle, DT.Delib.DAPI_SPECIAL_CMD_GET_MODULE_VERSION, DT.Delib.DAPI_SPECIAL_GET_MODULE_PAR_VERSION_1, 0, 0)
                          + "." +
                          +DT.Delib.DapiSpecialCommand(handle, DT.Delib.DAPI_SPECIAL_CMD_GET_MODULE_VERSION, DT.Delib.DAPI_SPECIAL_GET_MODULE_PAR_VERSION_2, 0, 0)
                          + DT.Delib.DapiSpecialCommand(handle, DT.Delib.DAPI_SPECIAL_CMD_GET_MODULE_VERSION, DT.Delib.DAPI_SPECIAL_GET_MODULE_PAR_VERSION_3, 0, 0);
            }
        }
        

        // --------------------
        // --------------------
        // --------------------

        void GetIONames()
        {
            if (handle == 0)
            {
                return;
            }
            
            List<string> pars = new List<string>();
            string[] vals;
            int pos = 0;
            uint ret;

            Func<string, uint, bool> AddParams = (key, cnt) =>
            {
                for (int i=0; i!=cnt; i++)
                {
                    pars.Add(key + i.ToString());
                }
                return true;
            };

            AddParams("io_di#", IO.cnt_di);
            AddParams("io_do#", IO.cnt_do);
            AddParams("io_ai#", IO.cnt_ai);
            AddParams("io_ao#", IO.cnt_ao);
            AddParams("io_temp#", IO.cnt_temp);
            AddParams("io_output_pwm#", IO.cnt_do_pwm);

            if ((ret = DT.Delib.ReadMultipleParams(handle, pars.ToArray(), out vals)) == DT.Error.DAPI_ERR_NONE)
            {
                Func<List<string>, uint, bool> ExtractParams = (list, cnt) =>
                {
                    for (int i = 0; i != cnt; i++)
                    {
                        list.Add(vals[pos++]);
                    }
                    return true;
                };

                pos = 0;
                ExtractParams(IOName.di, IO.cnt_di);
                ExtractParams(IOName.dout, IO.cnt_do);
                ExtractParams(IOName.ai, IO.cnt_ai);
                ExtractParams(IOName.ao, IO.cnt_ao);
                ExtractParams(IOName.temp, IO.cnt_temp);
                ExtractParams(IOName.pwm, IO.cnt_do_pwm);

                ioNamesValid = true;
            }
            else
            {
                Func<List<string>, uint, bool> CreateDefault = (list, cnt) =>
                {
                    for (int i = 0; i != cnt; i++)
                    {
                        list.Add("Channel " + i.ToString());
                    }
                    return true;
                };

                CreateDefault(IOName.di, IO.cnt_di);
                CreateDefault(IOName.dout, IO.cnt_do);
                CreateDefault(IOName.ai, IO.cnt_ai);
                CreateDefault(IOName.ao, IO.cnt_ao);
                CreateDefault(IOName.temp, IO.cnt_temp);
                CreateDefault(IOName.pwm, IO.cnt_do_pwm);
            }
        }

        // --------------------
        // --------------------
        // --------------------

        public List<String> GetIONames(IOType ioType)
        {
            List<string> ret = null;
            switch (ioType)
            {
                case IOType.DI: ret = IOName.di; break;
                case IOType.DO: ret = IOName.dout; break;
                case IOType.PWM: ret = IOName.pwm; break;
                //case IOType.DO_TIMER:   ret = m.IOName.do_timer;break;
                case IOType.AD: ret = IOName.ai; break;
                case IOType.DA: ret = IOName.ao; break;
                case IOType.TEMP: ret = IOName.temp; break;
            }
            return ret;
        }

        public String GetIOName(IOType ioType, uint channel)
        {
            List<string> names = GetIONames(ioType);
            if (names != null)
            {
                if (channel < names.Count)
                {
                    return names[(int) channel];
                }
            }

            return null;
        }

        // --------------------
        // --------------------
        // --------------------

        public Module() { }
        public Module(String name, String ip, int port, int timeout, String mac)
        {
            this.boardname = name;
            this.tcp_hostname = ip;
            this.tcp_port = port;
            this.tcp_timeout = timeout;
            this.mac = mac;
        }

        public Module(String ip, int port, int timeout)
        {
            this.tcp_hostname = ip;
            this.tcp_port = port;
            this.tcp_timeout = timeout;
        }

        // --------------------
        // --------------------
        // --------------------

        public class _IO
        {
            public uint cnt_di;
            public uint cnt_do;
            public uint cnt_ai;
            public uint cnt_ao;
            public uint cnt_temp;
            public uint cnt_do_timer;
            public uint cnt_do_pwm;
        }

        // --------------------
        // --------------------
        // --------------------

        public class _IOName
        {
            public List<string> di = new List<string>();
            public List<string> dout = new List<string>();
            public List<string> ai = new List<string>();
            public List<string> ao = new List<string>();
            public List<string> temp = new List<string>();
            public List<string> do_timer = new List<string>();
            public List<string> pwm = new List<string>();
        }
    }
}