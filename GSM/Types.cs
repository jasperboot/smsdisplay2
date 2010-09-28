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

namespace GSM
{
    /* From GSM specs */
    public enum NumberFormat
    {
        internationalFormat = 0x91,
        unknownFormat = 0x81,
        nationalFormat = 0xA1,
        networkFormat = 0xB1,
        subscriberFormat = 0xC1,
        alphanumFormat = 0xD0,
        abbrFormat = 0xE1,
        errorFormat = 0x80

    }

    /* Internal enums */
    public enum ConnectionType
    {
        GSM = 0,
        G3 = 1
    }

    public enum SilentModeType
    {
        Unknown = -1,
        Off = 0,
        On = 1
    }
}
