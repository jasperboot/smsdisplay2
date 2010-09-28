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
using GSM;

namespace GSM.AT.Packets
{
    public class NumberPacket : GenericPacket
    {
        new public static string Command = "AT+CNUM";
        public static string ActionCommand() { return Command; }
        public static string TestCommand() { return Command + "=?"; }

        public NumberPacket(string requestString) : base(requestString) { }

        public string PhoneNumber
        {
            get
            {
                string pn = "";
                foreach (string dataLine in _data)
                {
                    string[] details = Response.GetResponseData(dataLine);
                    if ((details[0] == "\"VOICE\"") || ((details.Length > 4) && (details[4] == "4")))
                    {
                        // alles omzetten naar nationaal nummer (gaat uit van landnummer met 2 digits!!)
                        if (details[2] == Convert.ToString((int)(NumberFormat.internationalFormat),10)) //145
                            pn = "0" + details[1].Trim(new char[] { '"' }).Substring(3);
                        else
                            pn = details[1].Trim(new char[] { '"' });
                    }
                }
                return pn;
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
                        packetMessage = "Current phone number: \t{0}";
                        break;
                    case PacketType.Set:
                        packetMessage = InvalidModeText();
                        break;
                    case PacketType.Read:
                        packetMessage = InvalidModeText();
                        break;
                }
                return String.Format(packetMessage, this.PhoneNumber);
            }
        }
    }
}
