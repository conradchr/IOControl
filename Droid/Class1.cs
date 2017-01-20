/*

using System;
using Android.Widget;
using Android.App;
using Android.Graphics.Drawables;
using Android.Views;
using System.ComponentModel;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Graphics;
using IOControl.Droid;

[assembly: ExportRenderer(typeof(IOControl.CustomTabPage), typeof(IOControl.Droid.CustomTabRenderer))]
namespace IOControl.Droid
{
    public class CustomTabRenderer : TabbedRenderer
    {

        private Activity activity;
        private TabbedPage _tabbedPage;
        private const string COLOR = "#FFFFFF";


        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            base.OnElementChanged(e);

            activity = this.Context as Activity;
            _tabbedPage = e.NewElement as TabbedPage;


        }

        protected override void DispatchDraw(Canvas canvas)
        {

            ActionBar actionBar = activity.ActionBar;


            if (actionBar.TabCount > 0)
            {
                ColorDrawable colorDrawable = new ColorDrawable(global::Android.Graphics.Color.ParseColor(COLOR));
                actionBar.SetStackedBackgroundDrawable(colorDrawable);

                ActionBarTabsSetup(actionBar);

            }

            base.DispatchDraw(canvas);
        }


        private void ActionBarTabsSetup(ActionBar actionBar)
        {
            try
            {
                //_tabbedPage.Children[0].IC
                for (int i = 0; i < actionBar.TabCount; i++)
                {
                    Android.App.ActionBar.Tab dashboardTab = actionBar.GetTabAt(i);
                    if (TabIsEmpty(dashboardTab))
                    {
                        int id = Resources.GetIdentifier(_tabbedPage.Children[i].Icon.File, "drawable", Context.PackageName);
                        TabSetup(dashboardTab, id);
                    }

                }

            }
            catch (Exception)
            {

            }

        }

        private bool TabIsEmpty(ActionBar.Tab tab)
        {
            if (tab != null)
                if (tab.CustomView == null)
                    return true;
            return false;
        }

        private void TabSetup(ActionBar.Tab tab, int resourceID)
        {
            ImageView iv = new ImageView(activity);
            iv.SetImageResource(resourceID);
            iv.SetPadding(0, 10, 0, 0);

            tab.SetCustomView(iv);
        }

    }
}

*/