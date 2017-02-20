using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using System.Linq;

using Xamarin.Forms;

namespace IOControl
{
    public class PopUpHelp : PopupPage
    {
        public class Constructor
        {
            public String Title { get; set; }
            public View Content { get; set; } = null;
            public bool IsToolTip { get; set; } = false;
        }

        public Constructor Ctor { get; set; }
        public Task<bool> PageCloseTask { get { return tcs.Task; } }

        TaskCompletionSource<bool> tcs;
        bool taskResult = false;

        public PopUpHelp(Constructor ctor)
        {
            Ctor = ctor;
            tcs = new TaskCompletionSource<bool>();

            StackLayout slMain = new StackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = DT.COLOR_POPUP,
                Padding = new Thickness(20, 20, 20, 20)
            };

            // headline
            Label title = new Label()
            { 
                Text = Ctor.Title,
                TextColor = DT.COLOR_POPUP_HEADER,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center,
            };
            slMain.Children.Add(title);

            Label headline = new Label()
            {
                BackgroundColor = DT.COLOR_POPUP_HEADER,
                HeightRequest = 0.4
            };
            slMain.Children.Add(headline);

            // content
            if (Ctor.Content != null)
            {
                StackLayout content = new StackLayout()
                {
                    Padding = new Thickness(0, 20, 0, 20)
                };
                content.Children.Add(Ctor.Content);
                slMain.Children.Add(content);
            }

            // Footer
            /*
            StackLayout layout = DTControl.Switch(Resx.AppResources.HELP_ShowTooltip, true, NamedSize.Small) as StackLayout;
            Switch sw = layout.Children.Select(x => x is Switch) as Switch;

            slMain.Children.Add(layout);
            */
            slMain.Children.Add(DTControl.Switch(Resx.AppResources.HELP_ShowTooltip, true, NamedSize.Small));

            Button btn = new Button()
            {
                Text = Resx.AppResources.MSG_OK
            };
            btn.Clicked += (s, e) =>
            {
                //DT.Log(sw.IsToggled.ToString());
                PopupNavigation.PopAsync();
            };
            slMain.Children.Add(btn);

            Content = slMain;

            this.BackgroundColor = new Color(0, 0, 0, 0.4);
            this.Padding = new Thickness(40, 0, 40, 0);

            this.Disappearing += (s, e) =>
            {
                tcs.SetResult(taskResult);
            };
        }

        protected override bool OnBackgroundClicked()
        {
            return false;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
