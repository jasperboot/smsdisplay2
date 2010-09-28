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

namespace GSM.AT.Packets
{
    public class OperatorPacket : GenericPacket
    {
        new public static string Command = "AT+COPS";
        public static string SetCommand(int mode) { return Command + "=" + mode; }
        public static string ReadCommand() { return Command + "?"; }
        public static string TestCommand() { return Command + "=?"; }

        public OperatorPacket(string requestString) : base(requestString) { }

        public string Operator
        {
            get
            {
                string op = "";
                foreach (string dataLine in _data)
                {
                    string[] details = Response.GetResponseData(dataLine);
                    if (details.Length > 2) op = details[2].Trim(new char[] { '"' });
                }
                return op;
            }
        }

        public ConnectionType Connection
        {
            get
            {
                ConnectionType ct = ConnectionType.GSM;
                foreach (string dataLine in _data)
                {
                    string[] details = Response.GetResponseData(dataLine);
                    if (details.Length >= 4) {
                        int ctValue;
                        if ((Int32.TryParse(details[3], out ctValue)) && (ctValue == 2)) ct = ConnectionType.G3;
                    }
                }
                return ct;
            }
        }

        public override string DebugText
        {
            get
            {
                if (Type == PacketType.Test) return base.DebugText;
                string packetMessage = "";
                switch (this.Type)
                {
                    case PacketType.Action:
                        packetMessage = InvalidModeText();
                        break;
                    case PacketType.Set:
                        packetMessage = InvalidModeText(); // Not supported by us yet
                        break;
                    case PacketType.Read:
                        packetMessage = "Network operator: \t{0} ({1})";
                        break;
                }
                string netop = this.Operator;
                if (netop == "") netop = "No network";
                string nettype = (this.Connection == ConnectionType.G3 ? "3G/UMTS" : "GSM");
                return String.Format(packetMessage, netop, nettype);
            }
        }
    }
}
