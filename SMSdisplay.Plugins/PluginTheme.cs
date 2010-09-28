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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace SMSdisplay.Plugins
{
    public class PluginTheme
    {
        public PluginTheme(string xamlFile)
        {
            string fileName;
            if (Path.IsPathRooted(xamlFile))
                fileName = xamlFile;
            else
                fileName = Environment.CurrentDirectory + @"\Themes\" + xamlFile + ".theme.xaml";

            if (File.Exists(fileName))
            {
                Name = Path.GetFileNameWithoutExtension(fileName).Replace(".theme", "");
                try
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Open))
                    {
                        // Read in ResourceDictionary File
                        StyleDictionary = (ResourceDictionary)XamlReader.Load(fs);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while loading theme file '{0}': {1}", fileName, e.Message);
                }
            }
        }

        public string Name { private set; get; }
        public ResourceDictionary StyleDictionary { private set; get; }

        public override string ToString()
        {
            return Name;
        }

        public static List<PluginTheme> GetPluginThemes()
        {
            return GetPluginThemes(Path.Combine(Environment.CurrentDirectory, "Themes"));
        }
        public static List<PluginTheme> GetPluginThemes(string indexFolder)
        {
            List<PluginTheme> foundThemes = new List<PluginTheme>();
            
            string[] files = Directory.GetFiles(indexFolder, "*.theme.xaml");
            foreach (string f in files)
            {
                PluginTheme theme = new PluginTheme(Path.Combine(indexFolder, f));
                if (theme.StyleDictionary != null) foundThemes.Add(theme);
            }            
            return foundThemes;
        }
    }
}
