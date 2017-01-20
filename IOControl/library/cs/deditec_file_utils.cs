using System;
using System.Collections.Generic;
using System.Text;
using System.IO; // StreamWriter
using System.Xml.Serialization; // XML
#if (!__MOBILE__)
using System.Windows.Forms;
#endif


public class DT_FileUtils
{
    /*
    // --------------------------------------------------------------------
    // path = "C:\\Textfile.txt"
    // text = "Hello world"
    // appendText = true            text will be appended to existing text
    // appendText = false           text will overwrite all existing text
    static public uint WriteToFile(string path, string text, bool appendText)
    {
        uint ret = DT.RETURN_OK;

        try
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(path, appendText);
            file.WriteLine(text);
            file.Close();
        }
        catch (Exception)
        {
            // file is read-only
            // path is to long
            // path is invalid
            // disk is full
            ret = DT.RETURN_ERROR;
        }


        return ret;
    }

    // --------------------------------------------------------------------
    // path = "C:\\Textfile.txt"
    static public uint ReadFromFile(string path, out string[] buffer)
    {
        uint ret = DT.RETURN_OK;
        
        try
        {
            buffer = System.IO.File.ReadAllLines(@path);
        }
        catch (Exception)
        {
            // file not found
            ret = DT.RETURN_ERROR;
            buffer = new string[0];
        }

        return ret;
    }

    // --------------------------------------------------------------------
    // --------------------------------------------------------------------
    // --------------------------------------------------------------------
    // --------------------------------------------------------------------
    // --------------------------------------------------------------------

    static public uint WriteObjectToXml(String path, Object obj)
    {
        uint ret = DT.RETURN_OK;


        try
        {   
            XmlSerializer writer = new XmlSerializer(obj.GetType());
            System.IO.StreamWriter file = new System.IO.StreamWriter(path);
            writer.Serialize(file, obj);
            file.Close();
        }
        catch (Exception e)
        {
			#if (!__MOBILE__)
				DT.DBG.Error(e.ToString());
			#endif
            return DT.RETURN_ERROR;
        }

        return ret;
    }

    static public uint WriteListToXml(String path, Object obj, Type[] objTypes)
    {
        try
        {
            XmlSerializer writer = new XmlSerializer(obj.GetType(), objTypes);
            System.IO.StreamWriter file = new System.IO.StreamWriter(path);
            writer.Serialize(file, obj);
            file.Close();            
        }
        catch (Exception e)
        {
			#if (!__MOBILE__)
				DT.DBG.Error(e.ToString());
			#endif
            return DT.RETURN_ERROR;
        }

        return DT.RETURN_OK;
    }


    // --------------------------------------------------------------------
    // --------------------------------------------------------------------
    // --------------------------------------------------------------------
    // --------------------------------------------------------------------
    // --------------------------------------------------------------------
	
	/* --------------------------------------------------------------------
	CALL:
	Object temp = new Object();
	CANConfiguration tempConfiguration = new CANConfiguration();
	DT.File.ReadObjectFromXml(FILENAME, out temp, typeof(CANConfiguration));
	tempConfiguration = (CANConfiguration) temp;
	-------------------------------------------------------------------- */

    /*
    static public uint ReadObjectFromXml(String path, out Object obj, Type t)
    {
        uint ret = DT.RETURN_OK;

        //try
        {
            //XmlSerializer reader = new XmlSerializer(t);
            XmlSerializer reader = new XmlSerializer(t, new Type[] { typeof(DT_Control.Control), typeof(DT_Control.SelectBox) });
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            obj = Convert.ChangeType(reader.Deserialize(file), t);
            file.Close();
        }
        //catch (Exception)
        {
            //obj = new Object();

            //ret = DT.RETURN_ERROR;
        }

        return ret;
    }*/
    /*
    static public Object ReadListFromXml(String path, Type objType, Type[] objTypes)
    {
        Object obj = new Object();

        try
        {
            XmlSerializer reader = new XmlSerializer(objType, objTypes);
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            obj = Convert.ChangeType(reader.Deserialize(file), objType);
            file.Close();
        }
        catch (Exception)
        {
            return null;
        }

        return obj;
    }

    static public Object ReadObjectFromXml(String path, Type t)
    {
        Object obj = new Object();

        try
        {
            XmlSerializer reader = new XmlSerializer(t);
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            obj = Convert.ChangeType(reader.Deserialize(file), t);
            file.Close();
        }
        catch (Exception e)
        {
            //DT.DBG.Error(e.ToString());
            return null;
        }

        return obj;
    }





    #if (!__MOBILE__)

    // --------------------------------------------------------------------
    // --------------------------------------------------------------------
    // --------------------------------------------------------------------
    // --------------------------------------------------------------------
    // --------------------------------------------------------------------
    /// <summary>
    /// Open a windows help file (.chm)
    /// </summary>
    /// <param name="path">e.g. "file:\\c:\\WindowsHelpFile.chm"</param>
    /// <returns></returns>
    static public uint OpenCHMwindowsHelp(string path)
    {
        return OpenCHMwindowsHelp(path, null);
    }

    // --------------------------------------------------------------------
    /// <summary>
    /// Open a windows help file (.chm)
    /// </summary>
    /// <param name="path">e.g. "file:\\c:\\WindowsHelpFile.chm"</param>
    /// <param name="chapter">e.g. "chapter.htm" (open chm with a zip program to see the htm files)</param>
    /// <returns></returns>

    static public uint OpenCHMwindowsHelp(string path, string chapter)
    {
        uint ret = DT.RETURN_OK;

        if (File.Exists(path) == true)
        {
            try
            {
                if (chapter == null)
                {
                    Help.ShowHelp(null, path);
                }
                else
                {
                    // does not verify if chapter could be open!
                    // no easy method available to check the chapter...
                    Help.ShowHelp(null, path, chapter);
                }
            }
            catch (Exception)
            {
                ret = DT.RETURN_ERROR;
            }
        }
        else
        {
            // file does not exist
            ret = DT.RETURN_ERROR;
        }

        return ret;
    }
    #endif

    // --------------------------------------------------------------------
    */
}
