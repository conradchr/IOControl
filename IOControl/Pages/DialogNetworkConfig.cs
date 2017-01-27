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
            ADD
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
        StackLayout slUploading;
        StackLayout slContent;
        StackLayout slFooter;

        ToolbarItem tiHelp;

        ActivityIndicator aiUploading;

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
            entName.Text = Ctor.Module.boardname;
            entHostname.Text = Ctor.Module.tcp_hostname;
            entPort.Text = Ctor.Module.tcp_port.ToString();
            entTimeout.Text = Ctor.Module.tcp_timeout.ToString();
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

            slMain = new StackLayout() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };
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
            slFooter = new StackLayout() { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.End };
            grid = new Grid();

            imgCancel = new Image() { Source = ImageSource.FromFile("btn_cancel.png") };
            var imgCancelTapped = new TapGestureRecognizer();
            imgCancelTapped.Tapped += (s, e) => Navigation.PopAsync();
            imgCancel.GestureRecognizers.Add(imgCancelTapped);

            imgOK = new Image() { Source = ImageSource.FromFile("btn_ok.png") };
            var imgOKTapped = new TapGestureRecognizer();
            imgOKTapped.Tapped += async (s, e) => await SaveModule();
            imgOK.GestureRecognizers.Add(imgOKTapped);

            grid.Children.Add(imgCancel, 0, 0);
            grid.Children.Add(imgOK, 1, 0);
            slFooter.Children.Add(grid);
            slMain.Children.Add(slFooter);


            slUploading = new StackLayout() { VerticalOptions = LayoutOptions.CenterAndExpand, IsVisible = false };
            aiUploading = new ActivityIndicator() { Color = Color.Red, IsRunning = true };
            slUploading.Children.Add(aiUploading);
            slUploading.Children.Add(new Label() { Text = Resx.AppResources.NC_SetNewModuleConfig, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) });
            slMain.Children.Add(slUploading);

            Content = slMain;
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
                await Navigation.PopAsync();
            }

            return true;
        }

        public async Task<bool> SaveModuleX()
        {
            Task<bool> t = new Task<bool>(() =>
            {
                uint ret;
                byte[] buffer = new byte[256];

                Module module = new Module(entHostname.Text, Convert.ToInt32(entPort.Text), Convert.ToInt32(entTimeout.Text));
                uint handle = module.OpenModule();

                if (handle != 0)
                {
                    ret = DT.Delib.DapiSpecialCommandExt(handle, DT.Ext.DAPI_SPECIAL_CMDEXT_TCP_MODULE_GET_CURRENT_CONFIG, 0, 0, 0, ref DT.dummy_uint, ref DT.dummy_uint, ref DT.dummy_uint, new Byte[] { 0 }, 0, new Byte[] { 0 }, 0, buffer, (uint)buffer.Length, ref DT.dummy_uint);
                    if (ret == 0)
                    {
                        // nur dann ok, weil ich die mac als ID brauch
                        DT.Bc.ETHDeviceConfig devcfg = new DT.Bc.ETHDeviceConfig();
                        devcfg.SetDeviceConfig(buffer);

                        module.boardname = devcfg.BoardName.boardname;
                        module.mac = devcfg.Network.mac_formatted;

                        taskResult = module;
                        return true;
                    }
                }                
                
                // modul nicht gefunden
                taskResult = null;
                return false;
            });

            if (await DTControl.ShowLoadingWhileTask(t))
            {
                return true;
            }

            DTControl.ShowToast("Communication nicht OK");
            return false;
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