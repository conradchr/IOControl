using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOControl
{
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public static class TypeExtensions
    {
        // List<T>
        public static void ForEach<T>(this List<T> list, Action<T> action)
        {
            foreach (var item in list)
                action(item);
        }

        // ObservableCollection<T>
        public static void AddRange<T>(this ObservableCollection<T> oc, IEnumerable<T> collection)
        {
            oc.ToList().AddRange(collection);
        }


        public static void ClearMAC<T>(this List<T> list, String MAC) where T : ETHModule.Module
        {
            list.ForEach(module =>
            {
                if (module.mac == MAC)
                {
                    // mach stuff
                }

            });
        }
    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public class ListViewGroups<T> : ObservableCollection<T>
    {
        internal ObservableCollection<ListViewGroupHeader<T>> groups;
        internal class ListViewGroupHeader<T> : ObservableCollection<T>
        {
            public string LongName { get; set; }
            public string ShortName { get; set; }

            public ListViewGroupHeader(String name)
            {
                LongName = name.ToUpper();
                ShortName = name.ToUpper().Substring(0, 1);
            }
        }

        

        public void AddGroup(String name, ObservableCollection<T> items)
        {
            if (items.Count > 0)
            { 
                var grp = new ListViewGroupHeader<T>(name);
                grp.AddRange(items);
                Groups.Add(grp);
            }
        }

        /*
        public void AddGroup (this ObservableCollection<ListViewGroupHeader<T>> source, String name, ObservableCollection<T> items, Action<T> action)
        {
            var grp = new ListViewGroupHeader<T>(name);
            foreach (var item in items)
            {
                
            }
        }
        */
    }


}
}
