#define OPTIMAL_S5

using System.Collections.Generic;
using Xamarin.Forms;
//using XLabs.Forms.Controls;
using Acr.UserDialogs;
using System.Threading.Tasks;
using System.Diagnostics;
using System;

namespace IOControl
{
    public class DTControl
    {

        public static Label GetLabel(String text)
        {
            return new Label()
            {
                Text = text,
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                VerticalOptions = LayoutOptions.Center
            };
        }

        public enum Images
        {
            WARNING,
            UPDATE,
        }

        public static Image GetImage(Images img)
        {
            string imgSource = "";

            switch (img)
            {
                case Images.WARNING: imgSource = "ic_error_outline_white"; break;
                case Images.UPDATE: imgSource = "ic_system_update_white"; break;       
            }

            imgSource += "_48dp.png";
            return new Image() { Source = ImageSource.FromFile(imgSource) };
        }

        public static View GetImageCell(string imgSource, string text)
        {
            Image img = new Image() { Source = ImageSource.FromFile(imgSource), HorizontalOptions = LayoutOptions.Start };
            StackLayout layout = new StackLayout() { Orientation = StackOrientation.Horizontal };
            layout.Children.Add(img);
            layout.Children.Add(GetLabel(text));
            return layout;
        }

        public static View GetImageCell(Images image, string text)
        {
            Image img = GetImage(image);
            img.HorizontalOptions = LayoutOptions.Start;
            StackLayout layout = new StackLayout() { Orientation = StackOrientation.Horizontal };
            layout.Children.Add(img);
            layout.Children.Add(GetLabel(text));
            return layout;
        }


        public enum IOViewStyle
        {
            AD,
            DA,
            TEMP,
            TEXT
        }

        public static void ShowToast(string text)
        {
            ToastConfig tc = new ToastConfig(text);
            tc.SetDuration(2000);
            UserDialogs.Instance.Toast(tc);
        }

        public static void ShowSuccess(string text)
        {
            /*
            ToastConfig tc = new ToastConfig(text);
            tc.SetDuration(2000);
            */
        }

        public static void ShowError(string text)
        {
            //UserDialogs.Instance.ShowError(text);
        }


        
		

        public static Layout View(Dictionary<int, View> dict, string text, IOViewStyle style, int id)
        {
            StackLayout layout = new StackLayout();
            layout.Orientation = StackOrientation.Horizontal;
            layout.Padding = new Thickness(10, 0, 10, 0);

            Label header = GetLabel(text);
            header.HorizontalOptions = LayoutOptions.StartAndExpand;
            layout.Children.Add(header);

            SpecialLabel val = new SpecialLabel();
            val.TextColor = Color.White;
            val.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
            val.VerticalOptions = LayoutOptions.Center;
            val.HorizontalOptions = LayoutOptions.EndAndExpand;
            dict.Add(id, val);

            if (style == IOViewStyle.DA)
            {
                val.BackgroundColor = DT.COLOR;
            }
            layout.Children.Add(val);

            return layout;
        }


        public static Layout ListViewSeparator()
        {
            StackLayout layout = new StackLayout();

            Label label = new Label()
            {
                BackgroundColor = Color.White,
#if (OPTIMAL_S5)
                HeightRequest = 1
#else
                HeightRequest = 0.2
#endif
            };

            layout.Children.Add(label);

            return layout;
        }

        public static Layout Separator()
        {
            StackLayout layout = new StackLayout();

            Label label = new Label()
            {
                BackgroundColor = Color.White,
#if (OPTIMAL_S5)
                HeightRequest = 1
#else
                HeightRequest = 0.2
#endif
            };   

            layout.Children.Add(label);
            layout.Padding = new Thickness(0, 10, 0, 10);

            return layout;
        }


        // achtung: es gibt auch "SwitchCell" hat direkt schon label drin

        public static Layout Switch(string text, bool toggled, NamedSize textSize)
        {
            return Switch(null, text, true, toggled, DT.NULL, textSize);
        }

        public static Layout Switch(string text, bool enabled, bool toggled)
        {
            return Switch(null, text, enabled, toggled, DT.NULL, NamedSize.Medium);
        }

        public static Layout Switch(Dictionary<int, View> dict, string text, bool enabled, bool toggled, int id)
        {
            //return Switch(null, text, enabled, toggled, DT.NULL, NamedSize.Medium);
            return Switch(dict, text, enabled, toggled, id, NamedSize.Medium);
        }

        public static Layout Switch(Dictionary<int, View> dict, string text, bool enabled, bool toggled, int id, NamedSize textSize)
        {
            //var xx = new SwitchCell();

            StackLayout layout = new StackLayout();
            layout.Orientation = StackOrientation.Horizontal;

#if (OPTIMAL_S5)
            layout.Padding = new Thickness(10, 0, 10, 0);
#else
            layout.Padding = new Thickness(10, 3, 10, 3);
#endif


            Label label = new Label();
            label.Text = text;
            label.TextColor = Color.White;
            label.VerticalOptions = LayoutOptions.Center;
            label.HorizontalOptions = LayoutOptions.StartAndExpand;
            label.FontSize = Device.GetNamedSize(textSize, typeof(Label));

            Switch sw = new Switch();
            //ExtendedSwitch sw = new ExtendedSwitch();

            sw.VerticalOptions = LayoutOptions.Center;
            sw.HorizontalOptions = LayoutOptions.EndAndExpand;
            sw.IsEnabled = enabled;
            //sw.TintColor = Color.Red;

            // test
            if ((id != DT.NULL) && (dict != null))
            {
                dict.Add(id, sw);
            }

            // kommt raus
            sw.IsToggled = toggled;

            

            layout.Children.Add(label);
            layout.Children.Add(sw);

            var clickEvent = new TapGestureRecognizer();
            if (enabled)
            {
                clickEvent.Tapped += (s, e) =>
                {
                    sw.IsToggled = !sw.IsToggled;
                };
                layout.GestureRecognizers.Add(clickEvent);
            }

            return layout;
        }

        public static Layout Slider(Dictionary<int, View> dict, string text, int id)
        {
            StackLayout innerLayout = new StackLayout();
            StackLayout layout = new StackLayout();
            Label labelText = new Label();
            Slider slider = new Slider();
            Label labelValue = new Label();

            labelText.Text = text;
            labelText.TextColor = Color.White;
            labelText.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
            labelText.VerticalOptions = LayoutOptions.Center;
            labelText.HorizontalOptions = LayoutOptions.StartAndExpand;

            labelValue.Text = "0";
            labelValue.TextColor = Color.White;
            labelValue.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
            labelValue.VerticalOptions = LayoutOptions.Center;
            labelValue.HorizontalOptions = LayoutOptions.EndAndExpand;

            innerLayout.Orientation = StackOrientation.Horizontal;
            innerLayout.Children.Add(labelText);
            innerLayout.Children.Add(labelValue);

            slider.Maximum = 100;
            slider.ValueChanged += new System.EventHandler<ValueChangedEventArgs>((s, e) =>
            {
                //labelValue.Text = slider.Value.ToString();
                labelValue.Text = ((int)slider.Value).ToString();
            });
            dict.Add(id, slider);

            layout.Padding = new Thickness(10, 0, 10, 0);
            layout.Children.Add(innerLayout);
            layout.Children.Add(slider);

            return layout;
        }

        /*
        Image imgDelete = new Image() { Source = ImageSource.FromFile("btn_delete.png") };
        var imgDeleteTapped = new TapGestureRecognizer();
        imgDeleteTapped.Tapped += async(s, e) => await DeleteItem();
        imgDelete.GestureRecognizers.Add(imgDeleteTapped);
            gridFooter.Children.Add(imgDelete, 0, 0);
        */


        public class SpecialLabel : Label
        {
            public float? Value { get; set; }
        }


        public class ImageButton : Image
        {
            //public System.EventHandler Tap;
            //public System.EventHandler EnabledChanged;

            bool enabled = true;
            public bool Enabled
            {
                get { return enabled; }
                set
                {
                    if (enabled != value)
                    {
                        enabled = value;
                        OnEnabledChanged(System.EventArgs.Empty);
                    }
                }
            }

            public TapGestureRecognizer TGR { get; set; }
            //TapGestureRecognizer tpg;

            string imgFile;

            public ImageButton(string file)
            {
                TGR = new TapGestureRecognizer();
                GestureRecognizers.Add(TGR);

                imgFile = file;

                OnEnabledChanged(System.EventArgs.Empty);
            }

            protected virtual void OnEnabledChanged(System.EventArgs e)
            {
                IsEnabled = Enabled;

                if (Enabled)
                {
                    Source = ImageSource.FromFile(string.Format("{0}.png", imgFile));
                }
                else
                {
                    Source = ImageSource.FromFile(string.Format("{0}_disabled.png", imgFile));
                }
            }


        }

        public class DOButton : Button
        {
            public event EventHandler Pressed;
            public event EventHandler Released;
            Color bgColor;
            Color txtColor;
            public bool IsPressed { get; set; } = false;

            public void Release()
            {
                OnReleased();
            }

            public virtual void OnPressed()
            {
                bgColor = this.BackgroundColor;
                txtColor = this.TextColor;

                this.BackgroundColor = Color.White;
                this.TextColor = Color.Black;

                Pressed?.Invoke(this, EventArgs.Empty);
                IsPressed = true;
            }

            public virtual void OnReleased()
            {
                this.BackgroundColor = bgColor;
                this.TextColor = txtColor;

                Released?.Invoke(this, EventArgs.Empty);
                IsPressed = false;
            }
        }


        public static Layout xDOButton(Dictionary<int, View> dict, string text, int id, NamedSize textSize)
        {
            StackLayout layout = new StackLayout();
            layout.Orientation = StackOrientation.Horizontal;

            /*
#if (OPTIMAL_S5)
            layout.Padding = new Thickness(10, 0, 10, 0);
#else
*/
            layout.Padding = new Thickness(10, -3, 10, -3);
//#endif

            Label label = new Label();
            label.Text = text;
            label.TextColor = Color.White;
            label.VerticalOptions = LayoutOptions.Center;
            label.HorizontalOptions = LayoutOptions.StartAndExpand;
            label.FontSize = Device.GetNamedSize(textSize, typeof(Label));

            DOButton btn = new DOButton();
            btn.FontSize = Device.GetNamedSize(textSize, typeof(Button));
            btn.Text = "SET";
            btn.VerticalOptions = LayoutOptions.Center;
            btn.HorizontalOptions = LayoutOptions.EndAndExpand;
            //btn.Scale = 0.7f;
            
            dict.Add(id, btn);
            Sess.Buttons.Add(btn);

            layout.Children.Add(label);
            layout.Children.Add(btn);

            return layout;
        }
    }
}
