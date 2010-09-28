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

namespace SMSdisplay.Plugins.FullscreenChat
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PresentationWindow : PluginWindow
    {
        public PresentationWindow(MessageList visibleMessages)
            : base()
        {
            InitializeComponent();
            VisibleMessages = visibleMessages;
        }

        private MessageList VisibleMessages
        {
            get; set;
        }

        public void CheckSize()
        {
            messageList.UpdateLayout();
            if ((messageList.DesiredSize.Height > messageList.ActualHeight) && (VisibleMessages.Count > 0))
            {
                removeOldest();
                messageList.UpdateLayout();
            }
        }

        private void removeOldest()
        {
            ContentPresenter firstItem = (messageList.ItemContainerGenerator.ContainerFromIndex(0) as ContentPresenter);
            if (!firstItem.IsEnabled) VisibleMessages.RemoveAt(0);
            Message firstMessage = VisibleMessages[0];
            firstItem = (messageList.ItemContainerGenerator.ContainerFromItem(firstMessage) as ContentPresenter);
            firstItem.IsEnabled = false;
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.AutoReset = false;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(delegate(object sender, System.Timers.ElapsedEventArgs args)
            {
                timer.Dispose();
                Dispatcher.BeginInvoke(new Action(delegate()
                {
                    VisibleMessages.Remove(firstMessage);
                    CheckSize();
                }));
            });
            timer.Enabled = true;
        }
    }
}
