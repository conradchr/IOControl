﻿using System.Collections.Generic;
using Xamarin.Forms;
//using XLabs.Forms.Controls;

namespace IOControl
{
    public class DTControl
    {
        public enum IOViewStyle
        {
            AD,
            DA,
            TEMP,
            TEXT
        }

        public static Layout View(Dictionary<int, View> dict, string text, IOViewStyle style, int id)
        {
            StackLayout layout = new StackLayout();
            layout.Orientation = StackOrientation.Horizontal;
            layout.Padding = new Thickness(10, 0, 10, 0);

            Label header = new Label();
            header.Text = text;
            header.TextColor = Color.White;
            header.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
            header.VerticalOptions = LayoutOptions.Center;
            header.HorizontalOptions = LayoutOptions.StartAndExpand;

            Label val = new Label();
            val.TextColor = Color.White;
            val.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
            val.VerticalOptions = LayoutOptions.Center;
            val.HorizontalOptions = LayoutOptions.EndAndExpand;
            dict.Add(id, val);

            /*
            switch (style)
            {
                case IOViewStyle.AD:
                    val.Text = value.ToString("0.000") + " V";
                    break;

                case IOViewStyle.DA:
                    val.Text = "4.56 V";
                    val.BackgroundColor = Color.Red;
                    break;

                case IOViewStyle.TEMP:
                    val.Text = "7.89 °C";
                    break;

                case IOViewStyle.TEXT:
                    val.Text = "disconnected";
                    break;

                default:
                    break;
            }
            */

            layout.Children.Add(header);
            layout.Children.Add(val);

            return layout;
        }

        public static Layout Separator()
        {
            StackLayout layout = new StackLayout();

            Label label = new Label()
            {
                BackgroundColor = Color.White,
                HeightRequest = 0.2
            };   

            layout.Children.Add(label);
            layout.Padding = new Thickness(0, 10, 0, 10);

            return layout;
        }


        // achtung: es gibt auch "SwitchCell" hat direkt schon label drin

        public static Layout Switch(string text, bool enabled, bool toggled)
        {
            return Switch(null, text, enabled, toggled, DT.NULL);
        }

        public static Layout Switch(Dictionary<int, View> dict, string text, bool enabled, bool toggled, int id)
        {
            //var xx = new SwitchCell();

            StackLayout layout = new StackLayout();
            layout.Orientation = StackOrientation.Horizontal;
            layout.Padding = new Thickness(10, 0, 10, 0);

            Label label = new Label();
            label.Text = text;
            label.TextColor = Color.White;
            label.VerticalOptions = LayoutOptions.Center;
            label.HorizontalOptions = LayoutOptions.StartAndExpand;
            label.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));



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
    }
}
