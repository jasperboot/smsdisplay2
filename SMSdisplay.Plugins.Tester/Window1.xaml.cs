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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SMSdisplay.Plugins;

namespace SMSdisplay.Plugins.Tester
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        IPlugin plugin;
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            plugin = new Plugins.FullscreenChat.Plugin();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            plugin.Initialize();
//            plugin.SetViewport(350, 100, 1024, 768);
            plugin.SetViewport(0, 0, 1680, 1050);
            plugin.ApplyThemeStyles(new PluginTheme("Valentine"));
            listBox1.DataContext = plugin.VisibleMessages;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            plugin.Show();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            plugin.NewMessage(new Message(String.Format("{0} - This is a test message with a \n new line somewhere, bla bla bla 28342894y sjkfnhsjkf wuirhjk v efnhsfhwfh iwufu. Almost as unreadable as a proper SMS Enjoy!",new Random().Next(100))));
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            plugin.Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            plugin.Cleanup();
        }

        private void cbCosts_Click(object sender, RoutedEventArgs e)
        {
            plugin.SetCostsVisibility(cbCosts.IsChecked ?? false);
        }

    }
}
