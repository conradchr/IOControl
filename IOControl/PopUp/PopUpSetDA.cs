using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace IOControl
{
    public class PopupSetDA : PopupPage
    {
        public Task<float?> PageCloseTask { get { return tcs.Task; } }
        TaskCompletionSource<float?> tcs;
        float? taskResult = null;

        public PopupSetDA()
        {
            tcs = new TaskCompletionSource<float?>();

            StackLayout slMain = new StackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = DT.COLOR_POPUP,
                Padding = new Thickness(10, 0, 10, 0)
            };

            Label header = new Label()
            { 
                Text = "Neuen D/A-Wert setzen",
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center,
            };
            slMain.Children.Add(header);

            StackLayout slPicker = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Padding = new Thickness(0, 10, 0, 10)
            };

            Picker pickerV = new Picker() { Title = "V", Scale = 1.3 };
            Picker pickermV = new Picker() { Title = "mV", Scale = 1.3 };

            for (int i = 0; i != 10; i++)
            {
                pickerV.Items.Add(i.ToString());
                pickermV.Items.Add((i * 100).ToString());
            }

            slPicker.Children.Add(pickerV);
            slPicker.Children.Add(pickermV);
            slMain.Children.Add(slPicker);

            Grid footer = new Grid() { VerticalOptions = LayoutOptions.End };

            TapGestureRecognizer tgp;
            Image imgCancel = new Image() { Source = ImageSource.FromFile("btn_cancel.png") };
            tgp = new TapGestureRecognizer();
            tgp.Tapped += (s, e) => PopupNavigation.PopAsync();
            imgCancel.GestureRecognizers.Add(tgp);
            footer.Children.Add(imgCancel, 0, 0);

            Image imgOK = new Image() { Source = ImageSource.FromFile("btn_ok.png") };
            tgp = new TapGestureRecognizer();
            tgp.Tapped += (s, e) =>
            {
                if ((pickerV.SelectedIndex != -1) && (pickermV.SelectedIndex != -1))
                {
                    float v = Convert.ToSingle(pickerV.Items[pickerV.SelectedIndex]);
                    float mv = Convert.ToSingle(pickermV.Items[pickermV.SelectedIndex]);
                    taskResult = v + (mv / 1000);
                    PopupNavigation.PopAsync();
                }
                else
                {
                    DTControl.ShowToast("Bitte geben Sie einen gültigen Wert ein");
                }
            };
            imgOK.GestureRecognizers.Add(tgp);
            footer.Children.Add(imgOK, 1, 0);
            slMain.Children.Add(footer);

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
