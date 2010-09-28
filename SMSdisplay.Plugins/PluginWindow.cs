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
using System.Windows.Media;

namespace SMSdisplay.Plugins
{
    public class PluginWindow : Window
    {
        private Point distanceFormMouse;
        private Brush originalBackground;
        private bool originalAllowsTransparency;
        
        public PluginWindow()
            : base()
        {
            this.WindowState = WindowState.Normal;
            this.WindowStyle = WindowStyle.None;
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.ResizeMode = ResizeMode.NoResize;
            this.Topmost = true;
            this.ShowInTaskbar = false;
            this.ShowActivated = false;
            this.Closing += new System.ComponentModel.CancelEventHandler(PluginWindow_Closing);
            this.MouseDown += new MouseButtonEventHandler(PluginWindow_MouseDown);
            this.MouseMove += new MouseEventHandler(PluginWindow_MouseMove);
            this.KeyDown +=new KeyEventHandler(PluginWindow_KeyDown);
            originalBackground = this.Background;
            originalAllowsTransparency = this.AllowsTransparency;
        }

        public void ApplyThemeStyleDictionary(ResourceDictionary styleDictionary)
        {
            Resources.BeginInit();

            try
            {
                // Clear any previous dictionaries loaded
                Resources.MergedDictionaries.Clear();
                // Add in newly loaded Resource Dictionary
                Resources.MergedDictionaries.Add(styleDictionary);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to apply new theme style");
            }
            finally
            {
                Resources.EndInit();
            }
        }

        private void PluginWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Visibility != Visibility.Hidden) e.Cancel = true;
        }

        private void PluginWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.Right:
                    Point MousePosition = this.PointToScreen(Mouse.GetPosition(this));
                    distanceFormMouse = new Point(MousePosition.X - this.Left, MousePosition.Y - this.Top);
                    break;
            }
        }

        private void PluginWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (this.WindowState != WindowState.Maximized)
                {
                    Point MousePosition = this.PointToScreen(Mouse.GetPosition(this));
                    Point newPosition = new Point(MousePosition.X - distanceFormMouse.X, MousePosition.Y - distanceFormMouse.Y);
                    this.Left = newPosition.X;
                    this.Top = newPosition.Y;
                }
            }
        }

        void PluginWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.T) Topmost = !Topmost;
            if (e.Key == Key.B)
            {
                if (Background == originalBackground)
                {
                    Background = new SolidColorBrush(Colors.Black);
                    //AllowsTransparency = false;
                }
                else
                {
                    Background = originalBackground;
                    //AllowsTransparency = originalAllowsTransparency;
                }
            }
        }

    }
}
