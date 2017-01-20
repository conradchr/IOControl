/*
using System;
using Xamarin.Forms;
using System.IO; // StreamWriter
using System.Xml.Serialization; // XML
using IOControl.Droid;

[assembly: Dependency(typeof(FileUtils_Android))]

namespace IOControl.Droid
{
    public class FileUtils_Android : IFileUtils
    {
        public void SaveToXML(String filepath, Object obj)
        {
            XmlSerializer writer = new XmlSerializer(obj.GetType());
            StreamWriter file = new StreamWriter(filepath);
            writer.Serialize(file, obj);
            file.Close();
        }

        public Object ReadFromXML(String filepath, Type t)
        {
            Object obj = new Object();

            try
            { 
                XmlSerializer reader = new XmlSerializer(t);
                System.IO.StreamReader file = new System.IO.StreamReader(filepath);
                obj = Convert.ChangeType(reader.Deserialize(file), t);
                file.Close();
            }
            catch
            {
                return null;
            }

            return obj;
        }

        public bool FileExist(String filepath)
        {
            return File.Exists(filepath);
        }

        public void FileDelete(String filepath)
        {
            File.Delete(filepath);
        }

        public String GetPathToFile(String filename)
        {
            return Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, filename);
            //Environment.GetFolderPath(Environment.SpecialFolder.Personal)
        }
    }
}
*/