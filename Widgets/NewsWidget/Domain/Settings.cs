using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace NewsWidget.Domain
{
    public class Settings
    {
        public List<string> Sources;

        public double Interval = 15;

        public double Left;

        public double Top;

        public int NewsCount = 10;

        public double ScaleFactor = 1.0f;

        public bool TopMost;

        public bool Pinned;

        public static Settings Read(string path)
        {
            var result = new Settings();
            if (File.Exists(path))
            {
                var f = new FileInfo(path);
                if (f.Length > 0)
                {
                    using (TextReader textReader = new StreamReader(path))
                    {
                        var deserializer = new XmlSerializer(typeof(Settings));
                        result = (Settings)deserializer.Deserialize(textReader);
                    }
                }
                else
                {
                    HTCHome.Core.Logger.Log("Settings file is corrupted.");
                }
            }
            if (result.Sources == null || result.Sources.Count == 0)
            {
                result.Sources =  new List<string>();
                result.Sources.Add("http://www.htchome.org/rss/en.xml");
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
