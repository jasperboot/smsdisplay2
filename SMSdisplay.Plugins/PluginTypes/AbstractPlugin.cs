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

namespace SMSdisplay.Plugins.PluginTypes
{
    public abstract class AbstractPlugin
    {
        public string Name
        {
            get
            {
                return (this.GetType().GetCustomAttributes(typeof(PluginAttribute), false)[0] as PluginAttribute).Name;
            }
        }

        public string Description
        {
            get
            {
                return (this.GetType().GetCustomAttributes(typeof(PluginAttribute), false)[0] as PluginAttribute).Description;
            }
        }

        public int Version
        {
            get
            {
                return (this.GetType().GetCustomAttributes(typeof(PluginAttribute), false)[0] as PluginAttribute).Version;
            }
        }

        protected string PhoneNumber
        {
            get; set; 
        }

        protected bool ShowCostsAndConditions
        {
            get; set;
        }

        public MessageList VisibleMessages
        {
            get;
            protected set;
        }

        public virtual void SetPhoneNumber(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }

        public virtual void SetCostsVisibility(bool showCostsAndConditions)
        {
            ShowCostsAndConditions = showCostsAndConditions;
        }

    }
}
