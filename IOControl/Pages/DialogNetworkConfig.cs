using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;    // INotifyPropertyChanged

using Xamarin.Forms;
using Acr.UserDialogs;

namespace IOControl
{
    public class DialogNetworkConfig : ContentPage
    {
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public enum ViewType
        {
            EDIT,
            ADD,
            CONFIG
        }

        public class Constructor
        {
            public Module Module { get; set; }
            public ViewType ViewType { get; set; }
        }

        public Constructor Ctor { get; set; }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        StackLayout slMain;
        
        StackLayout slContent;
        StackLayout slFooter;

        ToolbarItem tiHelp;

        

        Entry entName;
        Entry entHostname;
        Entry entPort;
        Entry entTimeout;
        Entry entEncryption;
        
        Grid grid;
        Image imgOK;
        Image imgCancel;

        public Task<Module> PageCloseTask { get { return tcs.Task; } }
        TaskCompletionSource<Module> tcs;
        Module taskResult = null;

        public DialogNetworkConfig(Constructor ctor)
        {
            Ctor = ctor;

            switch (Ctor.ViewType)
            {
                case ViewType.ADD:  Title = Resx.AppResources.NC_AddHeader;  break;
                case ViewType.EDIT: Title = Resx.AppResources.NC_EditHeader; break;
            }

            FormInit();

            if (Ctor.ViewType == ViewType.EDIT)
            {
                SetForm();
            }

            tcs = new TaskCompletionSource<Module>();

            this.Disappearing += (s, e) =>
            {
                tcs.SetResult(taskResult);
            };
        }

        public void SetForm()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                entName.Text = Ctor.Module.boardname;
                entHostname.Text = Ctor.Module.tcp_hostname;
                entPort.Text = Ctor.Module.tcp_port.ToString();
                entTimeout.Text = Ctor.Module.tcp_timeout.ToString();
            });
        }

        public Label LabelTemplate(String text)
        {
            return new Label()
            {
                TextColor = Color.White,
                Text = text,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };
        }

        public void FormInit()
        {
            ToolbarItems.Add(new ToolbarItem() { Text = "Help", Icon = "btn_help.png", Command = new Command(ShowHelp) });

            slMain = new StackLayout() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, Padding = VCModels.PAD_HEADER };
            slContent = new StackLayout() { VerticalOptions = LayoutOptions.FillAndExpand };

            if (Ctor.ViewType != ViewType.ADD)
            { 
                // name
                slContent.Children.Add(LabelTemplate("Module Name:"));
                entName = new Entry() { Keyboard = Keyboard.Text, Placeholder = "My DEDITEC Module" };
                slContent.Children.Add(entName);
            }

            // hostname
            slContent.Children.Add(LabelTemplate("IP/Hostname:"));
            entHostname = new Entry() { Keyboard = Keyboard.Text, Placeholder = "192.168.1.123 / my.device.net" };
            slContent.Children.Add(entHostname);

            // port
            slContent.Children.Add(LabelTemplate("Port:"));
            entPort = new Entry() { Keyboard = Keyboard.Numeric, Placeholder = "Default: 9912" };
            slContent.Children.Add(entPort);

            // timeout
            slContent.Children.Add(LabelTemplate("Timeout:"));
            entTimeout = new Entry() { Keyboard = Keyboard.Numeric, Placeholder = "Default: 5000" };
            slContent.Children.Add(entTimeout);

            /*
            // encryption pw
            slContent.Children.Add(LabelTemplate("Encryption password:"));
            entEncryption = new Entry() { Keyboard = Keyboard.Text, Placeholder = "Enter password", IsPassword = true };
            slContent.Children.Add(entEncryption);
            */

            slMain.Children.Add(slContent);

            // footer
            slFooter = new StackLayout() { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.End, Padding = VCModels.PAD_FOOTER };

            if (Ctor.ViewType == ViewType.EDIT)
            {
                slFooter.Children.Add(DTControl.GetImageCell(DTControl.Images.WARNING, Resx.AppResources.NC_WarningConfig));
            }

            grid = new Grid();
            grid.Padding = new Thickness(0, 5, 0, 0);

            imgCancel = new Image() { Source = ImageSource.FromFile("btn_cancel.png") };
            var imgCancelTapped = new TapGestureRecognizer();
            imgCancelTapped.Tapped += (s, e) => Navigation.PopAsync();
            imgCancel.GestureRecognizers.Add(imgCancelTapped);

            Image imgUpdate = DTControl.GetImage(DTControl.Images.UPDATE);
            var imgUpdateTapped = new TapGestureRecognizer();
            imgUpdateTapped.Tapped += async (s, e) => await LoadConfigFromModule();
            imgUpdate.GestureRecognizers.Add(imgUpdateTapped);

            imgOK = new Image() { Source = ImageSource.FromFile("btn_ok.png") };
            var imgOKTapped = new TapGestureRecognizer();
            imgOKTapped.Tapped += async (s, e) => await SaveModule();
            imgOK.GestureRecognizers.Add(imgOKTapped);

            grid.Children.Add(imgCancel, 0, 0);
            if (Ctor.ViewType == ViewType.EDIT)
            { 
                grid.Children.Add(imgUpdate, 1, 0);
            }
            grid.Children.Add(imgOK, 2, 0);
            slFooter.Children.Add(grid);
            slMain.Children.Add(slFooter);
            
            Content = new ScrollView() { Content = slMain };
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public async Task<bool> LoadConfigFromModule()
        {
            Task<bool> t = new Task<bool>(() =>
            {
                byte[] buffer = new byte[256];
                
                if (DT.ETH.GetEthernetDeviceConfig(0, Ctor.Module.mac.Replace(":", ""), DT_ETH.ConnectionType.UDP, ref buffer) == 0)
                {
                    DT.Bc.ETHDeviceConfig devcfg = new DT.Bc.ETHDeviceConfig();
                    devcfg.SetDeviceConfig(buffer);

                    Ctor.Module.boardname = devcfg.BoardName.boardname;
                    Ctor.Module.tcp_hostname = devcfg.Network.ip;
                    Ctor.Module.tcp_port = (int) devcfg.Network.port;

                    SetForm();
                    return true;
                }

                // modul nicht gefunden
                return false;
            });

            var answer = await UserDialogs.Instance.ConfirmAsync(
                Resx.AppResources.NC_LoadModuleConfigText,
                Resx.AppResources.NC_LoadModuleConfigHeader,
                Resx.AppResources.MSG_Yes,
                Resx.AppResources.MSG_No
            );

            if (answer)
            {
                // user hat confirmed
                if (await DTControl.ShowLoadingWhileTask(t))
                {
                    // modul wurde geupdated
                    DTControl.ShowToast(Resx.AppResources.NC_LoadModulConfigOK);
                    return true;
                }

                // modul konfig konnte nicht ausgelesen werden
                await UserDialogs.Instance.AlertAsync(
                    Resx.AppResources.NC_LoadModulConfigErrorText,
                    Resx.AppResources.NC_LoadModulConfigErrorHeader,
                    Resx.AppResources.MSG_OK
                );
            }

            return false;
        }


        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public async Task<bool> SaveModule()
        {
            if (await SaveModuleX())
            {
                // Communcation OK
                await Navigation.PopAsync();
                DTControl.ShowToast(Resx.AppResources.NC_EditToastOK);
                return true;
            }

            var answer = await UserDialogs.Instance.ConfirmAsync(
                Resx.AppResources.NC_CommErrorText,
                Resx.AppResources.NC_CommErrorHeader,
                Resx.AppResources.MSG_Yes,
                Resx.AppResources.MSG_No
            );

            if (answer)
            {
                // User möchte trotzdem fortfahren..
                await Navigation.PopAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> SaveModuleX()
        {
            Task<bool> t = new Task<bool>(() =>
            {
                byte[] buffer = new byte[256];
                Module module = new Module(entHostname.Text, Convert.ToInt32(entPort.Text), Convert.ToInt32(entTimeout.Text));
                uint handle = module.OpenModule();

                if (handle != 0)
                {
                    if (DT.ETH.GetEthernetDeviceConfig(handle, null, DT_ETH.ConnectionType.TCP, ref buffer) == 0)
                    {
                        if (Ctor.ViewType == ViewType.ADD)
                        { 
                            // nur dann ok, weil ich die mac als ID brauch
                            DT.Bc.ETHDeviceConfig devcfg = new DT.Bc.ETHDeviceConfig();
                            devcfg.SetDeviceConfig(buffer);

                            module.boardname = devcfg.BoardName.boardname;
                            module.mac = devcfg.Network.mac_formatted;
                        }

                        taskResult = module;
                        return true;
                    }
                }                
                
                // modul nicht gefunden
                taskResult = null;
                return false;
            });

            return await DTControl.ShowLoadingWhileTask(t);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

            /*
        public async Task<TaskResult> SetNewModuleConfig()
        {
            Func<bool, int> Visibility;
            Visibility = (vis) =>
            {
                slContent.IsVisible = vis;
                slFooter.IsVisible = vis;
                slUploading.IsVisible = !vis;
                return 0;
            };

            Task<TaskResult> t = new Task<TaskResult>(() =>
            {
                uint ret;

                if ((ret = DT.Bc.deditec_bc_set_string_parameter(Ctor.Module.mac, DT.Bc.Parameter.DEDITEC_BC_PACKET_PARAM_BOARD_NAME, entName.Text)) != DT.Error.DAPI_ERR_NONE)
                {
                    return new TaskResult() { ErrorCode = ret, ErrorMsg = "DEDITEC_BC_PACKET_PARAM_BOARD_NAME" };
                }

                if ((ret = DT.Bc.deditec_bc_set_string_parameter(Ctor.Module.mac, DT.Bc.Parameter.DEDITEC_BC_PACKET_PARAM_IP_ADDR, entHostname.Text)) != DT.Error.DAPI_ERR_NONE)
                {
                    return new TaskResult() { ErrorCode = ret, ErrorMsg = "DEDITEC_BC_PACKET_PARAM_IP_ADDR" };
                }

                
                if ((ret = DT.Bc.deditec_bc_set_string_parameter(Ctor.Module.mac, DT.Bc.Parameter.DED, entHostname.Text)) != DT.Error.DAPI_ERR_NONE)
                {
                    DT.Log("DEDITEC_BC_PACKET_PARAM_IP_ADDR");
                }
                

                if ((ret = DT.Bc.deditec_bc_eth0_config(Ctor.Module.mac)) != DT.Error.DAPI_ERR_NONE)
                {
                    return new TaskResult() { ErrorCode = ret, ErrorMsg = "DEDITEC_BC_PACKET_CMD_ETH0_CONFIGURE" };
                }

                return new TaskResult() { ErrorCode = DT.Error.DAPI_ERR_NONE };
            });

            t.Start();
            Visibility(false);
            TaskResult tr = await t;
            Visibility(true);

            return await t;
        }
        */

        public async void ShowHelp()
        {
            await UserDialogs.Instance.AlertAsync("test", "title", "oktext");
        }
    }
}