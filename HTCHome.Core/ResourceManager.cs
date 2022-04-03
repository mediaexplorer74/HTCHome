using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTCHome.Core
{
    public class ResourceManager
    {
        private string widgetFolderPath;
        private string skin;

        public ResourceManager(string widgetFolder, string skin)
        {
            widgetFolderPath = widgetFolder;
            this.skin = skin;
        }

        public string GetResourcePath(string relativePath)
        {
            if (File.Exists(widgetFolderPath + "\\Skins\\" + skin + "\\" + relativePath))
                return widgetFolderPath + "\\Skins\\" + skin + "\\" + relativePath;
            else
                if (File.Exists(widgetFolderPath + "\\Resources\\" + relativePath))
                    return widgetFolderPath + "\\Resources\\" + relativePath;
                else
                    return string.Empty;
        }
    }
}
