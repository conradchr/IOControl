using System;
using System.Collections.Generic;

using Xamarin.Forms;    // DependencyService

// --------------------
// --------------------
// --------------------

namespace IOControl
{
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public class XML
    {
        public const int XML_FILE_VERSION = 100;
        public const int XML_FILE_MAX = 10;
        public const string XML_DIRECTORY = "DEDITEC.IO.Control";

        public const string XML_FILENAME_CONTENT = "custom_view_{0}.xml";
        public const string XML_FILENAME_MODULE = "modules.xml";
        public const string XML_FILENAME_CONFIG = "app_config.xml";

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

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public class XMLConfig
        {
            public int FileVersion { get; set; } = XML_FILE_VERSION;
            public bool ShowDemoModule { get; set; } = true;
            public bool ShowSetting { get; set; } = true;

            public XMLConfig() { }
        }

        // --------------------
        // --------------------
        // --------------------

        public class XMLModul
        {
            public int FileVersion { get; set; } = XML_FILE_VERSION;
            public List<ETHModule.Module> modules;

            public XMLModul() { }
            public XMLModul(List<ETHModule.Module> modules)
            {
                this.modules = modules;

                foreach (ETHModule.Module module in modules)
                {
                    try
                    {
                        module.pw_encryption = DT.ENC.Encrypt(DT.DEDITEC_ENCRYPTION_TEMPORARY_ADMIN_PW, module.enc_pw);
                    }
                    catch (Exception) { }
                }
            }
        }

        // --------------------
        // --------------------
        // --------------------

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

        public class SessionXML
        {
            public int fileVersionModule;

            public List<XMLView> Views { get; set; } = new List<XMLView>();
            public List<ETHModule.Module> Modules { get; set; } = new List<ETHModule.Module>();
            public XMLConfig Config { get; set; }

            XMLModul cModules;

            // --------------------
            // --------------------
            // --------------------

            public void Save()
            {
                var fileService = DependencyService.Get<IFileUtils>();
                string file;

                fileService.CreateAppDir();

                for (int i = 0; i != XML_FILE_MAX; i++)
                {
                    file = fileService.GetPathToFile(String.Format(XML_FILENAME_CONTENT, i));
                    if (i < Views.Count)
                    {
                        fileService.SaveToXML(file, Views[i]);
                    }
                    else if (fileService.FileExist(file))
                    {
                        fileService.FileDelete(file);
                    }
                }

                cModules = new XMLModul(Modules);
                file = fileService.GetPathToFile(XML_FILENAME_MODULE);
                fileService.SaveToXML(file, cModules);

                file = fileService.GetPathToFile(XML_FILENAME_CONFIG);
                fileService.SaveToXML(file, Config);
            }

            // --------------------
            // --------------------
            // --------------------

            public void Load()
            {
                var fileService = DependencyService.Get<IFileUtils>();
                string file;
                XMLView tempL;
                XMLConfig tempC;

                Views.Clear();
                for (int i = 0; i != XML_FILE_MAX; i++)
                {
                    file = fileService.GetPathToFile(String.Format(XML_FILENAME_CONTENT, i));
                    if ((tempL = (XMLView)fileService.ReadFromXML(file, typeof(XMLView))) != null)
                    {
                        Views.Add(tempL);
                    }
                }

                file = fileService.GetPathToFile(XML_FILENAME_MODULE);
                if ((cModules = (XMLModul)fileService.ReadFromXML(file, typeof(XMLModul))) != null)
                {
                    Modules = cModules.modules;
                    this.fileVersionModule = cModules.FileVersion;

                    foreach (ETHModule.Module module in Modules)
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
                    Config = tempC;
                }
            }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
    }
}
