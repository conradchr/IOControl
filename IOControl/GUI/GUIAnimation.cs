using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using Acr.UserDialogs;

namespace IOControl
{
    public class GUIAnimation
    {
        public async static Task<T> ShowLoading<T>(Task<T> task)
        {
            Stopwatch sw = new Stopwatch();
            UserDialogs.Instance.ShowLoading(Resx.AppResources.MSG_PleaseWait);

            task.Start();
            sw.Start();

            T ret = await task;

            // künstlicher sleep damit die search animation durchkommt
            while (sw.ElapsedMilliseconds < GUI.TIME_ANIMATION_MIN_MS)
            {
                await Task.Delay(50);
            }
            UserDialogs.Instance.HideLoading();

            return ret;
        }
    }
}
