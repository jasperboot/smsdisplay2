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

namespace GSM.AT.Packets
{
    public class ResetPacket : GenericPacket
    {
        new public static string Command = "ATZ";
        public static string ActionCommand() { return Command; }

        public ResetPacket(string requestString) : base(requestString) { }

        public override string DebugText
        {
            get
            {
                if (Type == PacketType.Test) return base.DebugText;
                string packetMessage = "";
                switch (this.Type)
                {
                    case PacketType.Action:
                        packetMessage = "Connection reset command executed";
                        break;
                    case PacketType.Set:
                        packetMessage = InvalidModeText();
                        break;
                    case PacketType.Read:
                        packetMessage = InvalidModeText();
                        break;
                }
                return packetMessage;
            }
        }
    }
}
