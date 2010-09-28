/*
 * Copyright (C) 2009, 2010 Jasper Boot
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace SMSdisplay.Plugins
{
    public class PluginNotValidException : Exception
    {
        public PluginNotValidException(Type type, string message) : base(message)
        {
            this.Data.Add("Type", type);
        }
    }

    public static class PluginLoader
    {
        public static List<IPlugin> LoadPlugins()
        {
            List<IPlugin> plugins = new List<IPlugin>();
            string[] files = Directory.GetFiles("PresentationPlugins", "*.plugin.dll");

            foreach (string f in files)
            {

                try
                {
                    Assembly assembly = Assembly.LoadFrom(f);
                    Version version = assembly.GetName().Version;
                    System.Type[] types = assembly.GetTypes();
                    foreach (System.Type type in types)
                    {
                        if (type.GetInterface("IPlugin") != null)
                        {
                            object[] attributes = type.GetCustomAttributes(typeof(PluginAttribute), false);
                            if (attributes.Length < 1)
                                throw new PluginNotValidException(type, "PluginAttribute is not defined");

                            PluginAttribute details = (attributes[0] as PluginAttribute);

                            IPlugin plugin = (Activator.CreateInstance(type) as IPlugin);
                            plugins.Add(plugin);
                            Console.WriteLine("Plugin loaded: {0} v{1} ({3}) - {2}", details.Name, details.Version, details.Description, version.ToString(4));
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Plugin class '{0}' will not be loaded: {1}", e.Data, e.Message);
                }
            }

            return plugins;

        }
    }
}
