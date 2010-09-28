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
using System.Reflection;

/*
 *  AT-commands:
 *  ATQ:    Hide result codes
 *  CNUM:   Get phone number
 *  CBC:    Get battery info
 *  COPS:   Get operator info
 *  CSIL:   Set silent mode
 *  ESKL:   Key-lock mode (Ericsson)
 *  CSQ:    Get signal strength
 *  CPMS:   Set preferred message store ("ME" for phone "SM" for SIM) (gelijk memory bekijken en eventueel berichten verwijderen)
 *  CMGL:   List messages
 *  CCLK:   Date/time
 *  #PBDYN: Samsung's CNUM - response: #PBDYN: NNN-NNN-NNNN
 *  
 */

namespace GSM.AT.Packets
{
    public enum PacketType
    {
        Action = 0,
        Read = 1,
        Set = 2,
        Test = 3
    }

    public class GenericPacket
    {
        private PacketType _packetType;

        public GenericPacket(string requestString)
        {
            this._requestString = requestString;
            this._requestCmd = requestString.Split(new char[] { '=' })[0];
            if (requestString.IndexOf("=?") > -1)
                _packetType = PacketType.Test;
            else if (requestString.IndexOf("?") > -1)
                _packetType = PacketType.Read;
            else if (requestString.IndexOf("=") > -1)
                _packetType = PacketType.Set;
            else
                _packetType = PacketType.Action;
        }
        
        #region Public Statics

        public static string Command = "";

        public static GenericPacket FromData(List<string> contents)
        {
            GenericPacket packet;
            string requestString = contents[0].TrimEnd(new char[] { '\r' });
            string requestCmd = requestString.Split(new char[] { '=' })[0].Split(new char[] { '?' })[0];
            if (contents.Count > 1) contents.RemoveAt(0);
            string resultText = contents[contents.Count - 1];
            contents.RemoveAt(contents.Count - 1);
            // contents now only contains real data payload

            switch (requestCmd)
            {
                case "ATZ":
                    packet = new ResetPacket(requestString);
                    break;
                case "AT+GMI":
                    packet = new IdentPacket(requestString);
                    break;
                case "AT+GMM":
                    packet = new ModelPacket(requestString);
                    break;
                case "AT+CNUM":
                    packet = new NumberPacket(requestString);
                    break;
                case "AT#PBDYN":
                    packet = new SamsungNumberPacket(requestString);
                    break;
                case "AT+CPMS":
                    packet = new PrefMessageStorePacket(requestString);
                    break;
                case "AT+CSIL":
                    packet = new SilentPacket(requestString);
                    break;
                case "AT+CSQ":
                    packet = new SignalPacket(requestString);
                    break;
                case "AT+CBC":
                    packet = new BatteryPacket(requestString);
                    break;
                case "AT+COPS":
                    packet = new OperatorPacket(requestString);
                    break;
                case "AT+CMGL":
                    packet = new MessageListPacket(requestString);
                    break;
                case "AT+CMGR":
                    packet = new MessageReadPacket(requestString);
                    break;
                case "AT+CMGD":
                    packet = new MessageDeletePacket(requestString);
                    break;
                default:
                    packet = new GenericPacket(requestString);
                    break;
            }
            packet._data = contents;
            packet._resultText = resultText;
            return packet;
        }

        #region Response statics
        public static class Response
        {
            public static string GetResponseHeader(string responseLine)
            {
                string[] responseParts = responseLine.Split(new string[] { ":" },StringSplitOptions.None);
                if (responseParts.Length == 1) return ""; // malformed packet, without command reply (like AT+CGMI on SE phones)
                return responseParts[0];
            }

            public static string GetResponseDataString(string responseLine)
            {
                string[] parts=responseLine.Split(new string[] { ":" }, StringSplitOptions.None);
                if (parts.Length == 1) return parts[0]; // malformed packet, without command reply (like AT+CGMI on SE phones)
                string responseData = parts[1].TrimStart(new char[] { ' ' });
                return responseData;
            }
            public static string[] GetResponseData(string responseLine)
            {
                return GetResponseDataString(responseLine).Split(new char[] { ',' });
            }
        }
        #endregion

        #endregion

        #region Members
        private string _requestString;
        private string _requestCmd;
        protected List<string> _data;
        protected string _resultText;
        #endregion

        #region Properties
        
        public PacketType Type
        {
            get { return _packetType; }
        }

        public List<string> ResponseData
        {
            get { return _data; }
        }

        public string ResultText
        {
            get { return _resultText; }
        }

        public string RequestString
        {
            get { return _requestString; }
        }

        public string RequestCmd
        {
            get { return _requestCmd; }
        }

        public bool Result
        {
            get {
                return (_resultText == "OK");
            }
        }

        public bool Supported
        {
            get
            {
                if (Type == PacketType.Test) return Result;
                else return (_resultText != "ERROR");
            }
        }

        public string SupportedText()
        {
            return String.Format("{0} support:\t{1}", RequestCmd, Supported);

        }

        public string InvalidModeText()
        {
            return String.Format("Invalid cmd mode: \t{0}", this.RequestString);
        }


        public virtual string DebugText
        {
            get
            {
                if (Type == PacketType.Test)
                    return SupportedText();
                else
                {
                    string packetMsg = "Unknown packet ({0}): \t{1}";
                    return string.Format(packetMsg, this.ResultText, this.RequestString);
                }
            }
        }
        #endregion
    }

}
