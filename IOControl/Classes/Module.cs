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
        

        // --------------------
        // --------------------
        // --------------------

        void GetIONames()
        {
            int i;
            List<string> pars = new List<string>();
            string[] vals;
            int pos;

            for (i = 0; i != IO.cnt_di; i++)
            {
                pars.Add("io_di#" + i.ToString());
            }

            for (i = 0; i != IO.cnt_do; i++)
            {
                pars.Add("io_do#" + i.ToString());
            }

            for (i = 0; i != IO.cnt_ai; i++)
            {
                pars.Add("io_ai#" + i.ToString());
            }

            for (i = 0; i != IO.cnt_ao; i++)
            {
                pars.Add("io_ao#" + i.ToString());
            }

            for (i = 0; i != IO.cnt_temp; i++)
            {
                pars.Add("io_temp#" + i.ToString());
            }

            for (i = 0; i != IO.cnt_do_pwm; i++)
            {
                pars.Add("io_output_pwm#" + i.ToString());
            }


            DT.Delib.ReadMultipleParams(handle, pars.ToArray(), out vals);
            //vals = new string[256];

            pos = 0;

            for (i = 0; i != IO.cnt_di; i++)
            {
                IOName.di.Add(vals[pos++]);
            }

            for (i = 0; i != IO.cnt_do; i++)
            {
                IOName.dout.Add(vals[pos++]);
            }

            for (i = 0; i != IO.cnt_ai; i++)
            {
                IOName.ai.Add(vals[pos++]);
            }

            for (i = 0; i != IO.cnt_ao; i++)
            {
                IOName.ao.Add(vals[pos++]);
            }

            for (i = 0; i != IO.cnt_temp; i++)
            {
                IOName.temp.Add(vals[pos++]);
            }

            for (i = 0; i != IO.cnt_do_pwm; i++)
            {
                IOName.pwm.Add(vals[pos++]);
            }

            //DT.DBG.Print("testparams= di4" + IOName.di[4] + " do2" + IOName.dout[2]);
        }

        // --------------------
        // --------------------
        // --------------------

        public Module()
        {
        }

        // --------------------
        // --------------------
        // --------------------

            /*
        public Module(String name, String ip, int port, int timeout, long id)
        {
            this.boardname = name;
            this.tcp_hostname = ip;
            this.tcp_port = port;
            this.tcp_timeout = timeout;
            this.id = id;
        }
        */

        public Module(String name, String ip, int port, int timeout, String mac)
        {
            this.boardname = name;
            this.tcp_hostname = ip;
            this.tcp_port = port;
            this.tcp_timeout = timeout;
            this.mac = mac;
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