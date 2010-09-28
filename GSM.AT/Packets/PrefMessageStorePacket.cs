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
using GSM.Messages;

namespace GSM.AT.Packets
{
    public class PrefMessageStorePacket : GenericPacket
    {
        new public static string Command = "AT+CPMS";
        public static string SetCommand(string messageStore)
        {
            string ac = String.Format("{0}=\"{1}\"", Command, messageStore);
            return ac;
        }
        public static string ReadCommand() { return Command + "?"; }
        public static string TestCommand() { return Command + "=?"; }

        public PrefMessageStorePacket(string requestString) : base(requestString) { }

        public string CurrentStore
        {
            get
            {
                int item = 1; // 1-based

                string cs = "";
                if (Type == PacketType.Read)
                {

                    foreach (string dataLine in _data)
                    {
                        string[] details = Response.GetResponseData(dataLine);
                        if (details.Length >= item) cs = details[item - 1].Trim(new char[] {'"'});
                    }
                }
                return cs;
            }
        }

        public int MessageCount
        {
            get
            {
                int item = 1; // 1-based
                if (Type == PacketType.Read) item++;

                int mc = -1;
                foreach (string dataLine in _data)
                {
                    string[] details = Response.GetResponseData(dataLine);
                    if (details.Length >= item)
                    {
                        if (! Int32.TryParse(details[item - 1], out mc)) mc = -1;
                    }
                }
                return mc;
            }
        }

        public int MessageCapacity
        {
            get
            {
                int item = 2; // 1-based
                if (Type == PacketType.Read) item++;

                int mc = -1;
                foreach (string dataLine in _data)
                {
                    string[] details = Response.GetResponseData(dataLine);
                    if (details.Length >= item)
                    {
                        if (! Int32.TryParse(details[item - 1], out mc)) mc = -1;
                    }
                }
                return mc;
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
                        packetMessage = "Messages in store: \t{0}/{1} ({2})";
                        break;
                    case PacketType.Read:
                        packetMessage = "Messages in \"{3}\": \t{0}/{1} ({2})";
                        break;
                }
                int messageCount = this.MessageCount;
                int messageCapacity = this.MessageCapacity;
                int messageFill;
                if ((messageCount != -1) && (messageCapacity != -1))
                    messageFill = (int)Math.Round((double)messageCount / (double)messageCapacity * 100);
                else
                    messageFill = -1;
                return String.Format(packetMessage, messageCount, messageCapacity, (messageFill != -1) ? messageFill.ToString() + "% full": "uncomplete data", CurrentStore);
            }
        }    
    }
}
