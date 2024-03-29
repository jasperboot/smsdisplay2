﻿/*
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
using SMSdisplay.Plugins;
using SMSdisplay.Plugins.PluginTypes;
using SMSdisplay.Plugins.MessageBar.BuildInfo;

namespace SMSdisplay.Plugins.MessageBar
{
    [Plugin("Message bar", "Bar in the bottom of the screen showing one message", Version = 2, ApiVersion = PluginApi.Version)]
    public class Plugin : ScreenPlugin<PresentationWindow>, IPlugin
    {
        private const string postMessage = "Ook een berichtje plaatsen? Stuur een SMSje naar {0}";
        private const string systemOffline = "Momenteel worden nieuwe berichten niet ontvangen";
        private const string costsConditions = "Je betaalt enkel de normale prijs voor het versturen van een SMS";

        #region ScreenPlugin members
        public override PresentationWindow GetWindow()
        {
            return new PresentationWindow();
        }
        #endregion

        #region IPlugin Members

        public override void Initialize()
        {
            base.Initialize();
//            LoadThemeFile("Party");
            VisibleMessages.Add(new Message(""));
        }
        public override void SetPhoneNumber(string phoneNumber)
        {
            base.SetPhoneNumber(phoneNumber);
            PluginWindow.systemText.Text = String.Format(postMessage, PhoneNumber);
        }

        void IPlugin.NewMessage(Message message)
        {
            showMessage(message);
            showSystem();
        }

        void IPlugin.OldMessage(Message message)
        {
            showMessage(message);
            showSystem();
        }

        void IPlugin.NoMessage()
        {
            showMessage(new Message(String.Format(postMessage, PhoneNumber)));
            if (ShowCostsAndConditions)
            {
                PluginWindow.costsText.Text = costsConditions;
                showCosts();
            }
        }

        void IPlugin.SystemOffline()
        {
            PluginWindow.systemText.Text = systemOffline;
            showSystem();
        }

        private void showMessage(Message message)
        {
            PluginWindow.messageText.Text = message.MessageText;
            VisibleMessages.Clear();
            VisibleMessages.Add(message);
        }

        private void showSystem()
        {
            PluginWindow.costsText.Visibility = Visibility.Hidden;
            PluginWindow.systemText.Visibility = Visibility.Visible;
        }

        private void showCosts()
        {
            PluginWindow.systemText.Visibility = Visibility.Hidden;
            PluginWindow.costsText.Visibility = Visibility.Visible;
        }
        #endregion
    }
}
