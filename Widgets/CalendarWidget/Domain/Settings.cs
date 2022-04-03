using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace CalendarWidget.Domain
{
    public class Settings
    {
        public double left;

        public double top;

        public double opacity;

        public double scaleFactor = 1.0f;

        public bool topMost;

        public bool pinned;

        public bool synchronizeWithGoogle;

        public string username;

        public string password;

        public static Settings Read(string path)
        {
            var result = new Settings();
            if (File.Exists(path))
            {
                var f = new FileInfo(path);
                if (f.Length > 162)
                {
                    using (TextReader textReader = new StreamReader(path))
                    {
                        var deserializer = new XmlSerializer(typeof(Settings));
                        result = (Settings)deserializer.Deserialize(textReader);
                    }
                }
                else
                {
                    //App.Log("Settings file is corrupted.");
                }
            }
            return result;
        }

        public void Write(string path)
        {
            using (TextWriter textWriter = new StreamWriter(path))
            {
                var serializer = new XmlSerializer(typeof(Settings));
                serializer.Serialize(textWriter, this);
            }
        }
    }
}
