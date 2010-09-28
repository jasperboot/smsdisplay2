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
    public class SignalPacket : GenericPacket
    {
        new public static string Command = "AT+CSQ";
        public static string ActionCommand() { return Command; }
        public static string TestCommand() { return Command + "=?"; }

        public SignalPacket(string requestString) : base(requestString) { }

        public int SignalQuality
        {
            get
            {
                int sq = -1;
                foreach (string dataLine in _data)
                {
                    string[] details = Response.GetResponseData(dataLine);
                    if (!Int32.TryParse(details[0], out sq)) sq = -1;
                    if (sq == 99)
                        sq = -1;
                    else
                        sq = Convert.ToInt32((Math.Sqrt(sq) / Math.Sqrt(31)) * 100); // sq = Convert.ToInt32(sq / 0.31);
                    if (sq > 100) sq = 100;
                }
                return sq;
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
                        packetMessage = "Network signal: \t{0}";
                        break;
                    case PacketType.Set:
                        packetMessage = InvalidModeText();
                        break;
                    case PacketType.Read:
                        packetMessage = InvalidModeText();
                        break;
                }
                return String.Format(packetMessage, (this.SignalQuality > -1) ? this.SignalQuality+ "%" : "Unknown");
            }
        }


    }
}
