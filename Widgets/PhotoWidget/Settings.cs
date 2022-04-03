﻿using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace PhotoWidget
{
    public class Settings
    {
        public int Interval = 5;
        public double Left;
        public int Mode = 0;
        public string PicsPath;
        public bool Pinned;
        public double ScaleFactor = 1.0f;
        public bool SwitchAutomatically = true;
        public double Top;
        public bool Topmost;

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
