using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MediaPlayerWidget.Domain
{
    public class Controller
    {
        public static IController Load(string path)
        {
            Assembly assembly = Assembly.LoadFrom(path);
            Type providerType =
                assembly.GetTypes().FirstOrDefault(type => typeof(IController).IsAssignableFrom(type));
            if (providerType == null)
            {

                throw new TypeLoadException(String.Format("Failed to find IController in {0}", path));
            }

            return Activator.CreateInstance(providerType) as IController;
        }
    }
}
