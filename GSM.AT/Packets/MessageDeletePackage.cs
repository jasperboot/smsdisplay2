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
using GSM.Messages;

namespace GSM.AT.Packets
{
    public enum DelFlagType
    {
        Unknown = -1,
        SingleMessage = 0,
        Read = 1,
        ReadSent = 2,
        NotUnread = 3,
        All = 4
    }

    public class MessageDeletePacket : SMContainingPacket
    {
        new public static string Command = "AT+CMGD";
        public static string ActionCommand(int index, DelFlagType delFlag)
        {
            string ac = String.Format("{0}={1},{2}",Command, index, ((int)delFlag).ToString());
            return ac;
        }
        public static string TestCommand() { return Command + "=?"; }

        public MessageDeletePacket(string requestString) : base(requestString) { }

        public int MessageID
        {
            get
            {
                int id = 0;

                if (this.Type == PacketType.Set)
                {
                    string[] reqString = RequestString.Split(new char[] { '=' });
                    if (reqString.Length > 1)
                    {
                        string[] parms = GenericPacket.Response.GetResponseData(reqString[1]);
                        if (parms.Length >= 1)
                        {
                            Int32.TryParse(parms[0], out id);
                        }
                    }
                }
                return id;
            }
        }

        public DelFlagType DelFlag
        {
            get
            {
                DelFlagType df = DelFlagType.Unknown;

                if (this.Type == PacketType.Set)
                {
                    string[] reqString = RequestString.Split(new char[] { '=' });
                    if (reqString.Length > 1)
                    {
                        string[] parms = GenericPacket.Response.GetResponseData(reqString[1]);
                        if (parms.Length >= 2)
                        {
                            int dfValue;
                            if (! Int32.TryParse(parms[1], out dfValue)) dfValue = -1;
                            df = (DelFlagType)dfValue;
                        }
                        else if (parms.Length == 1)
                        {
                            df = DelFlagType.SingleMessage;
                        }
                    } 
                }
                return df;
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
                    case PacketType.Set:                                    // Actually an action-type command with parameters
                        if (DelFlag == DelFlagType.SingleMessage)
                            packetMessage = "SMS delete req. ID: \t{0} (single ID: {1})";
                        else
                            packetMessage = "SMS delete req.: \t{0} (by status: {2})";
                        break;
                    case PacketType.Read:
                        packetMessage = InvalidModeText();
                        break;
                }
                return String.Format(packetMessage, ResultText, this.MessageID, this.DelFlag);
            }
        }
    }
}
