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
        public class Constructor
        {
            public String Name { get; set; }
            public String Value { get; set; }
        }

        public Constructor Ctor { get; set; }
        public Task<float?> PageCloseTask { get { return tcs.Task; } }

        TaskCompletionSource<float?> tcs;
        float? taskResult = null;

        public PopupSetDA(Constructor ctor)
        {
            Ctor = ctor;
            tcs = new TaskCompletionSource<float?>();

            StackLayout slMain = new StackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = DT.COLOR_POPUP,
                Padding = new Thickness(20, 20, 20, 20)
            };

            Label header = new Label()
            { 
                Text = Ctor.Name,
                TextColor = DT.COLOR_POPUP_HEADER,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center,
            };
            slMain.Children.Add(header);

            Label headline = new Label()
            {
                BackgroundColor = DT.COLOR_POPUP_HEADER,
                HeightRequest = 0.4
            };
            slMain.Children.Add(headline);

            /*
            Label text = new Label()
            {
                Text = "Neuen D/A-Wert setzen",
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center,
            };
            slMain.Children.Add(text);
            */

            StackLayout slPicker = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Padding = new Thickness(0, 20, 0, 20)
            };

            Picker pickerV = new Picker() { Title = "V", Scale = 1.3 };
            Picker pickermV = new Picker() { Title = "mV", Scale = 1.3 };

            for (int i = 0; i != 10; i++)
            {
                pickerV.Items.Add(i.ToString());
                pickermV.Items.Add((i * 100).ToString("D3"));
            }

            string val = Ctor.Value.Substring(0, Ctor.Value.Length - 2);    // leerzeichen + 'V';
            string[] split = val.Split(',');
            pickerV.SelectedIndex = Convert.ToInt32(split[0]);
            pickermV.SelectedIndex = Convert.ToInt32(split[1].Substring(0, 1));

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
                    DTControl.ShowToast(Resx.AppResources.DA_InvalidValue);
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
