using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;    // DependencyService

namespace IOControl
{
    public class XML
    {
        public const int XML_FILE_VERSION = 100;
        public const int XML_FILE_MAX = 10;
        public const string XML_DIRECTORY = "DEDITEC.IO.Control";

        public const string XML_FILENAME_CONTENT = "custom_view_{0}.xml";
        public const string XML_FILENAME_MODULE = "modules.xml";
        public const string XML_FILENAME_CONFIG = "app_config.xml";


        public class XMLConfig
        {
            public int FileVersion { get; set; } = XML_FILE_VERSION;
            public bool ShowDemoModule { get; set; } = true;
            public bool ShowSetting { get; set; } = true;

            public XMLConfig() { }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public class XMLModul
        {
            public int FileVersion { get; set; } = XML_FILE_VERSION;
            public List<SessModule.Module> modules;

            public XMLModul() { }
            public XMLModul(List<SessModule.Module> modules)
            {
                this.modules = modules;

                foreach (SessModule.Module module in modules)
                {
                    try
                    {
                        module.pw_encryption = DT.ENC.Encrypt(DT.DEDITEC_ENCRYPTION_TEMPORARY_ADMIN_PW, module.enc_pw);
                    }
                    catch (Exception) { }
                }
            }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public class SessionXML
        {
            public int fileVersionModule;

            public List<XMLView> loc = new List<XMLView>();
            public List<SessModule.Module> modules = new List<SessModule.Module>();
            public XMLConfig config = new XMLConfig();

            XMLModul cModules;

            public void Save()
            {
                var fileService = DependencyService.Get<IFileUtils>();
                string file;

                fileService.CreateAppDir();

                for (int i = 0; i != XML_FILE_MAX; i++)
                {
                    file = fileService.GetPathToFile(String.Format(XML_FILENAME_CONTENT, i));
                    if (i < loc.Count)
                    {
                        fileService.SaveToXML(file, loc[i]);
                    }
                    else if (fileService.FileExist(file))
                    {
                        fileService.FileDelete(file);
                    }
                }

                cModules = new XMLModul(modules);
                file = fileService.GetPathToFile(XML_FILENAME_MODULE);
                fileService.SaveToXML(file, cModules);

                file = fileService.GetPathToFile(XML_FILENAME_CONFIG);
                fileService.SaveToXML(file, config);
            }

            public void Load()
            {
                var fileService = DependencyService.Get<IFileUtils>();
                string file;
                XMLView tempL;
                XMLConfig tempC;

                loc.Clear();
                for (int i = 0; i != XML_FILE_MAX; i++)
                {
                    file = fileService.GetPathToFile(String.Format(XML_FILENAME_CONTENT, i));
                    if ((tempL = (XMLView)fileService.ReadFromXML(file, typeof(XMLView))) != null)
                    {
                        loc.Add(tempL);
                    }
                }

                file = fileService.GetPathToFile(XML_FILENAME_MODULE);
                if ((cModules = (XMLModul)fileService.ReadFromXML(file, typeof(XMLModul))) != null)
                {
                    this.modules = cModules.modules;
                    this.fileVersionModule = cModules.FileVersion;

                    foreach (SessModule.Module module in modules)
                    {
                        try
                        {
                            module.enc_pw = DT.ENC.Decrypt(DT.DEDITEC_ENCRYPTION_TEMPORARY_ADMIN_PW, module.pw_encryption);
                        }
                        catch (Exception) { }
                    }
                }

                file = fileService.GetPathToFile(XML_FILENAME_CONFIG);
                if ((tempC = (XMLConfig)fileService.ReadFromXML(file, typeof(XMLConfig))) != null)
                {
                    config = tempC;
                }
            }
        }

        public class XMLView
        {
            public int FileVersion { get; set; } = XML_FILE_VERSION;
            public String Name { get; set; }
            public List<XMLViewGroup> Groups = new List<XMLViewGroup>();

            public XMLView() { }
        }

        public class XMLViewGroup
        {
            public String Name { get; set; }
            public List<XMLViewGroupIO> IOs = new List<XMLViewGroupIO>();

            public XMLViewGroup() { }
        }

        public class XMLViewGroupIO
        {
            public IOTypes IOType { get; set; }
            public GUIConfigs GUICfg { get; set; }
            public String MAC { get; set; }
            public uint Channel { get; set; }

            public XMLViewGroupIO() { }
            public XMLViewGroupIO(IOTypes ioType, String moduleMAC, uint channel)
            {
                this.IOType = ioType;
                this.Channel = channel;
                this.MAC = moduleMAC;

                switch (ioType)
                {
                    case IOTypes.DO:
                        GUICfg = GUIConfigs.SWITCH;
                        break;

                    case IOTypes.AD:
                    case IOTypes.DA:
                        GUICfg = GUIConfigs.VOLTAGE;
                        break;
                }
            }

            public bool Equals(XMLViewGroupIO io)
            {
                return ((IOType == io.IOType) && (MAC == io.MAC) && (Channel == io.Channel));
            }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public enum GUIConfigs
        {
            SWITCH,
            BUTTON,
            VOLTAGE,
            CURRENT,
            NONE
        }

        public enum IOTypes
        {
            DI,
            DO,
            AD,
            DA,
            PWM,
            DO_TIMER,
            TEMP,
            UNKNOWN,
        }

        public class xIOTypes
        {
            public IOTypes ioType;
            public string name;

            public xIOTypes(string name, IOTypes ioType)
            {
                this.name = name;
                this.ioType = ioType;
            }
        }
    }
}
