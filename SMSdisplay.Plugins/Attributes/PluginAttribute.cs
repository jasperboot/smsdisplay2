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

namespace SMSdisplay.Plugins
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginAttribute : System.Attribute
    {
        public PluginAttribute(string name, string description)
            : base()
        {
            Name = name;
            Description = description;
            Version = 1;
        }

        public override string ToString()
        {
            return Name;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Version { get; set; }
    }
}
