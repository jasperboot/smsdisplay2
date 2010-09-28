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
using System.Text;

namespace GSM.AT.Packets
{
    public class SilentPacket : GenericPacket
    {
        new public static string Command = "AT+CSIL";
        public static string SetCommand(bool mode) { return Command + "=" + (mode ? "1" : "0"); }
        public static string ReadCommand() { return Command + "?"; }
        public static string TestCommand() { return Command + "=?"; }

        public SilentPacket(string requestString) : base(requestString) { }

        public bool SilentMode
        {
            get
            {
                bool sm = false;
                if (this.Type == PacketType.Read)
                {
                    foreach (string dataLine in _data)
                    {
                        string[] details = Response.GetResponseData(dataLine);
                        sm = (details[0] == "1");
                    }

                }
                if (this.Type == PacketType.Set)
                {
                    string[] details = RequestString.Split(new char[] { '=' });
                    if (details.Length > 1) sm = (details[1] == "1");
                }
                return sm;
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
                        packetMessage = "New silent mode set: \t{0}";
                        break;
                    case PacketType.Read:
                        packetMessage = "Current silent mode: \t{0}";
                        break;
                }
                return String.Format(packetMessage, (SilentMode ? "On" : "Off"));
            }
        }
    }
}
