using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace MediaPlayerWidget.Domain
{
    public class Settings
    {
        public string controller = "WMP";

        public double left = -1;

        public double top = -1;

        public double opacity;

        public double scaleFactor = 1.0f;

        public string skin = "Sense";

        public bool topMost;

        public bool pinned;

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
