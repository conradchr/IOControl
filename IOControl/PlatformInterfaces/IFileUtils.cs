using System;

/*
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
*/

namespace IOControl
{
    public interface IFileUtils
    {

        //var fileService = DependencyService.Get<ISaveAndLoad>();
        // fileService.LoadTextAsync(fileName);

        void SaveToXML(String filepath, Object obj);
        Object ReadFromXML(String filepath, Type t);
        bool FileExist(String filepath);
        void FileDelete(String filepath);
        String GetPathToFile(String filename);
        void CreateAppDir();
    }
}
