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
    public class SMContainingPacket : GenericPacket
    {
        public SMContainingPacket(string requestString) : base(requestString) { }

        public TextMessageList Messages
        {
            get
            {
                TextMessageList ms = new TextMessageList();
                int count = this.ResponseData.Count;
                if (count < 2) return ms;
                for (int i = 1; i < count; i += 2)
                {
                    string msgData = this.ResponseData[i];
                    TextMessage sms = null;
                    SMSType smsType = BaseSMS.GetSMSType(msgData);

                    if (smsType == SMSType.Normal)
                    {
                            sms = new TextMessage();
                            TextMessage.Fetch(sms, ref msgData);
                            ms.Add(sms);
                    }
                }
                return ms;

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
                        packetMessage = "New unread SMS: \t{0}"; // is a Set Action with =0 as default param (unread messages)
                        break;
                    case PacketType.Set:
                        packetMessage = "New unread SMS: \t{0}"; // Actually an action-type command with parameter
                        break;
                    case PacketType.Read:
                        packetMessage = InvalidModeText();
                        break;
                }
                return String.Format(packetMessage, this.Messages.Count);
            }
        }
    }
}
