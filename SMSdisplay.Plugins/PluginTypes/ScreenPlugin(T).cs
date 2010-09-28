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
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Markup;

namespace SMSdisplay.Plugins.PluginTypes
{
    public abstract class ScreenPlugin<T> : AbstractPlugin
        where T: PluginWindow
    {
        protected T PluginWindow { get; set; }

        public abstract T GetWindow();

        #region IPlugin Members

        public virtual void Initialize()
        {
            // create an empty MessageList for VisibleMessages
            VisibleMessages = new MessageList();

            PluginWindow = GetWindow();
            if (PluginWindow != null)
            {
                // make sure the window really gets created by 'showing' it once
                double oldLeft = PluginWindow.Left;
                double oldTop = PluginWindow.Top;
                double oldWidth = PluginWindow.Width;
                double oldHeight = PluginWindow.Height;
                PluginWindow.Left = -10;
                PluginWindow.Top = -10;
                PluginWindow.Width = 0;
                PluginWindow.Height = 0;
                PluginWindow.Show();
                PluginWindow.Hide();
                PluginWindow.Left = oldLeft;
                PluginWindow.Top = oldTop;
                PluginWindow.Width = oldWidth;
                PluginWindow.Height = oldHeight;
            }
        }

        public virtual void Show()
        {
            PluginWindow.Show();
        }

        public virtual void SetViewport(int left, int top, int width, int height)
        {
            PluginWindow.Left = left;
            PluginWindow.Top = top;
            if (width > 0) PluginWindow.Width = width;
            if (height > 0) PluginWindow.Height = height;
        }

        public virtual void ApplyThemeStyles(PluginTheme theme)
        {
            PluginWindow.ApplyThemeStyleDictionary(theme.StyleDictionary);
        }

        public virtual void Hide()
        {
            if (PluginWindow != null) PluginWindow.Hide();
        }

        public virtual void Cleanup()
        {
            if (PluginWindow != null)
            {
                PluginWindow.Hide();
                PluginWindow.Close();
            }
        }

        public virtual void NewMessage(Message message)
        {
            // We don't do anything with this one
        }

        public virtual void OldMessage(Message message)
        {
            // We don't do anything with this one
        }

        public virtual void NoMessage()
        {
            // We don't do anything with this one
        }

        public virtual void SystemOffline()
        {
            // We don't do anything with this one
        }

        #endregion

        //protected void LoadThemeFile(string xamlFile)
        //{
        //    string fileName;
        //    if (Path.IsPathRooted(xamlFile))
        //        fileName = xamlFile;
        //    else
        //        fileName = Environment.CurrentDirectory + @"\Themes\" + xamlFile + ".theme.xaml";

        //    if (File.Exists(fileName))
        //    {
        //        try
        //        {
        //            using (FileStream fs = new FileStream(fileName, FileMode.Open))
        //            {
        //                // Read in ResourceDictionary File
        //                ResourceDictionary dic = (ResourceDictionary)XamlReader.Load(fs);
        //                PluginWindow.ApplyThemeStyleDictionary(dic);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("Error while loading theme file '{0}': {1}", fileName, e.Message);
        //        }
        //    }
        //}
    }
}
