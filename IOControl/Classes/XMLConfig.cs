using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization; // XML
using System.IO;    // file delete

using Xamarin.Forms;    // DependencyService

namespace IOControl
{
    public class ContentConfig
    {
        public int FileVersion { get; set; }
        public bool ShowDemoModule { get; set; }
        public bool ShowSetting { get; set; }

        public ContentConfig()
        {
            FileVersion = DT.Const.CONFIG_FILE_VERSION_CONFIG;

            ShowDemoModule = true;
            ShowSetting = true;
        }
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public class ContentModules
    {
        public int fileVersion = DT.Const.CONFIG_FILE_VERSION_MODULE;
        public List<Module> modules;

        public ContentModules() { }
        public ContentModules(List<Module> modules)
        {
            this.modules = modules;

            foreach (Module module in modules)
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

    public class XMLContent
    {
        public int fileVersionModule;

        public List<ContentLocation> loc = new List<ContentLocation>();
        public List<Module> modules = new List<Module>();
        public ContentConfig config = new ContentConfig();

        ContentModules cModules;

        public void Save()
        {
            var fileService = DependencyService.Get<IFileUtils>();
            string file;

            fileService.CreateAppDir();

            for (int i = 0; i != DT.Const.CONFIG_FILE_MAX; i++)
            {
                file = fileService.GetPathToFile(String.Format(DT.Const.CONFIG_FILE_NAME_CONTENT, i));
                if (i < loc.Count)
                {
                    fileService.SaveToXML(file, loc[i]);
                }
                else if (fileService.FileExist(file))
                {
                    fileService.FileDelete(file);
                }
            }

            cModules = new ContentModules(modules);
            file = fileService.GetPathToFile(DT.Const.CONFIG_FILE_NAME_MODULE);
            fileService.SaveToXML(file, cModules);

            file = fileService.GetPathToFile(DT.Const.CONFIG_FILE_NAME_CONFIG);
            fileService.SaveToXML(file, config);
        }

        public void Load()
        {
            var fileService = DependencyService.Get<IFileUtils>();
            string file;
            ContentLocation tempL;
            ContentConfig tempC;

            loc.Clear();
            for (int i = 0; i != DT.Const.CONFIG_FILE_MAX; i++)
            {
                file = fileService.GetPathToFile(String.Format(DT.Const.CONFIG_FILE_NAME_CONTENT, i));
                if ((tempL = (ContentLocation)fileService.ReadFromXML(file, typeof(ContentLocation))) != null)
                {
                    loc.Add(tempL);
                }
            }

            file = fileService.GetPathToFile(DT.Const.CONFIG_FILE_NAME_MODULE);
            if ((cModules = (ContentModules)fileService.ReadFromXML(file, typeof(ContentModules))) != null)
            {
                this.modules = cModules.modules;
                this.fileVersionModule = cModules.fileVersion;

                foreach (Module module in modules)
                {
                    try
                    {
                        module.enc_pw = DT.ENC.Decrypt(DT.DEDITEC_ENCRYPTION_TEMPORARY_ADMIN_PW, module.pw_encryption);
                    }
                    catch (Exception) { }
                }
            }

            file = fileService.GetPathToFile(DT.Const.CONFIG_FILE_NAME_CONFIG);
            if ((tempC = (ContentConfig)fileService.ReadFromXML(file, typeof(ContentConfig))) != null)
            {
                config = tempC;
            }
        }
    }

    public class ContentLocation
    {
        public float fileVersion = DT.Const.CONFIG_FILE_VERSION_CONTENT;
        public string name;
        public List<ContentGroup> groups = new List<ContentGroup>();

        public ContentLocation() { }
        public ContentLocation(string name)
        {
            this.name = name;
        }
    }

    public class ContentGroup
    {
        public string name;
        public List<ContentIO> io = new List<ContentIO>();

        public ContentGroup() { }
        public ContentGroup(string name)
        {
            this.name = name;
        }
    }

    public class ContentIO
    {
        public IOType ioType;
        public String moduleMAC;
        public uint channel;

        public ContentIO() { }
        public ContentIO(IOType ioType, String moduleMAC, uint channel)
        {
            this.ioType = ioType;
            this.channel = channel;
            this.moduleMAC = moduleMAC;
        }
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public enum IOType
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

    public class IOTypes
    {
        public IOType ioType;
        public string name;

        public IOTypes(string name, IOType ioType)
        {
            this.name = name;
            this.ioType = ioType;
        }
    }
}