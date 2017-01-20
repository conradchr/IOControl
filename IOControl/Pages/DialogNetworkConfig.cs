using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;    // INotifyPropertyChanged

using Xamarin.Forms;

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

        public Task<bool> PageCloseTask { get { return tcs.Task; } }
        TaskCompletionSource<bool> tcs;
        bool taskComplete = false;

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

            tcs = new TaskCompletionSource<bool>();

            this.Disappearing += (s, e) =>
            {
                tcs.SetResult(taskComplete);
                DT.Log("seite weg");
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
            tiHelp = new ToolbarItem() { Text = "Help", Icon = "btn_help.png"};
            ToolbarItems.Add(tiHelp);

            slMain = new StackLayout() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };
            slContent = new StackLayout() { VerticalOptions = LayoutOptions.FillAndExpand };

            // name
            slContent.Children.Add(LabelTemplate("Module Name:"));
            entName = new Entry() { Keyboard = Keyboard.Text, Placeholder = "My DEDITEC Module" };
            slContent.Children.Add(entName);

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

            // encryption pw
            slContent.Children.Add(LabelTemplate("Encryption password:"));
            entEncryption = new Entry() { Keyboard = Keyboard.Text, Placeholder = "Enter password", IsPassword = true };
            slContent.Children.Add(entEncryption);

            slMain.Children.Add(slContent);

            // footer
            slFooter = new StackLayout() { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.End };
            grid = new Grid();

            imgCancel = new Image() { Source = ImageSource.FromFile("btn_cancel.png") };
            var imgCancelTapped = new TapGestureRecognizer();
            imgCancelTapped.Tapped += (s, e) => {
                Navigation.PopAsync();
            };
            imgCancel.GestureRecognizers.Add(imgCancelTapped);

            imgOK = new Image() { Source = ImageSource.FromFile("btn_ok.png") };
            var imgOKTapped = new TapGestureRecognizer();
            imgOKTapped.Tapped += async (s, e) => {
                DT.Log("SetNewModuleConfig START");
                await SetNewModuleConfig();
                DT.Log("SetNewModuleConfig ENDE");
            };
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

        public class TaskResult
        {
            public uint ErrorCode { get; set; }
            public String ErrorMsg { get; set; }
        }
        
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

                /*
                if ((ret = DT.Bc.deditec_bc_set_string_parameter(Ctor.Module.mac, DT.Bc.Parameter.DED, entHostname.Text)) != DT.Error.DAPI_ERR_NONE)
                {
                    DT.Log("DEDITEC_BC_PACKET_PARAM_IP_ADDR");
                }
                */

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
    }
}