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
using System.Windows.Forms;
using SMSdisplay;
using System.Windows;

namespace SMSdisplay.Plugins
{
    public interface IPlugin
    {
        // NOTE: Please remember to add a new plugin-api-v<n> tag
        //       when changing the Plugin API

        string Name { get; }
        string Description { get; }
        int Version { get; }
        int ApiVersion { get; }

        // Lifecycle methods
        void Initialize();
        void SetPhoneNumber(string phoneNumber);
        void SetCostsVisibility(bool showCostAndConditions);
        void Show();
        void SetViewport(int left, int top, int width, int height);
        void ApplyThemeStyles(PluginTheme theme);
        void Hide();
        void Cleanup();

        // Message passing methods
        void NewMessage(Message message);
        void OldMessage(Message message);
        void NoMessage();
        void SystemOffline();

        MessageList VisibleMessages { get; }
    }

}
