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
using GSM.AT.Packets;

namespace GSM.AT
{
    public class PacketReceivedEventArgs : EventArgs
    {
        private GenericPacket _packet;

        public PacketReceivedEventArgs(GenericPacket packet)
        {
            _packet = packet;
        }

        public GenericPacket Packet
        {
            get { return _packet; }
        }
    }

    public class SerialPortList : List<SerialPortInfo>
    {
        public SerialPortInfo PhonePreferred
        {
            get
            {
                // check for phone-types with modem in description
                foreach (SerialPortInfo port in this)
                    if ((port.Type == SerialPortType.Phone) && (port.Description.IndexOf("Data Modem") > -1)) return port;
                // none found, check for phone-types with modem in description
                foreach (SerialPortInfo port in this)
                    if ((port.Type == SerialPortType.Phone) && (port.Description.IndexOf("Modem") > -1)) return port;
                // none found, check for phone-types
                foreach (SerialPortInfo port in this)
                    if ((port.Type == SerialPortType.Phone)) return port;
                // none found, check for modems
                foreach (SerialPortInfo port in this)
                    if ((port.Type == SerialPortType.Modem)) return port;
                // none found, return null
                return null;
            }
        }
    }

    public class SerialPortInfo
    {
/*
        private string _name;
        private string _description;
        private bool _availability;
        private SerialPortType _type;

        public SerialPortInfo(string portName, string description, bool availability, SerialPortType type)
        {
            _name = portName;
            _description = description;
            _availability = availability;
            _type = type;
        }
*/
        public override string ToString()
        {
            return String.Format("{0}: {1} ({2}){3}", Name, Description, Type, Availability ? "" : " (N/A)");
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool Availability { get; set; }
        public SerialPortType Type { get; set; }
    }

    public enum SerialPortType
    {
        RS232 = 0,
        USB = 1,
        Modem = 2,
        Phone = 3
    }

    public class SerialBuffer : List<string>
    {
        public SerialBuffer(int capacity) : base(capacity) { }

        public SerialBuffer Clone()
        {
            SerialBuffer copy = new SerialBuffer(this.Count);
            copy.AddRange(this);
            return copy;
        }
    }   
}