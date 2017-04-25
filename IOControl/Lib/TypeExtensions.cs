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
        // ObservableCollection<T>
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
                action(item);
        }
        
        // ObservableCollection<T>
        public static void AddRange<T>(this ObservableCollection<T> oc, IEnumerable<T> collection)
        {
            oc.ToList().AddRange(collection);
        }



        /*
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
        
         
         public class MyComparer : IEqualityComparer<Test>
        {
        http://stackoverflow.com/questions/9410321/c-sharp-using-select-and-where-in-a-single-linq-statement
            public bool Equals(Test x, Test y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(Test obj)
            {
                return string.Format("{0}{1}", obj.Id, obj.Name).GetHashCode();
            }
        }
         
         
         
         
         
         */




    }

    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------

    public class ListViewGroup<T> : ObservableCollection<T>
    {
        public string LongName { get; set; }
        public string ShortName { get; set; }

        public ListViewGroup(String name)
        {
            LongName = name.ToUpper();
            ShortName = name.ToUpper().Substring(0, 1);
        }
    }

    public class ListViewItems<T> : ObservableCollection<T>
    {
        public ObservableCollection<ListViewGroup<T>> Groups { get; set; } = new ObservableCollection<ListViewGroup<T>>();

        public void AddGroup(ListViewGroup<T> grp)
        {
            if (grp.Count > 0)
            {
                Groups.Add(grp);
            }
        }
        /*
        public ListViewGroup<T> GetGroup(String name)
        {
            return Groups.ToList().Find(x => x.LongName == name.ToUpper());
        }

        public ListViewGroup<T> CreateGroup(String name)
        {
            var grp = new ListViewGroup<T>(name);
            Groups.Add(grp);
            return grp;
        }
        */

    }
}
