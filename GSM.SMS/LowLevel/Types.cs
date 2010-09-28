/*
 * This source code is taken from the following project (possibly with slight modifications):
 * Library for Decode/Encode SMS PDU (smspdulib)
 * http://www.codeproject.com/KB/windows/smspdulib.aspx
 * 
 * The project is released under the The Code Project Open License (CPOL)
 * http://www.codeproject.com/info/cpol10.aspx
 */

using System;
using System.Collections.Generic;

namespace GSM.Messages
{
    public class TextMessageList : List<Messages.TextMessage>
    {
    }
    
    public static class Gsm7String
    {
        private static char GSM2UnicodeLookup(byte gsm)
        {
            UInt16[] lookupTable = new UInt16[128] {
            #region Lookup table
                64,
                163,
                36,
                165,
                232,
                233,
                249,
                236,
                242,
                199,
                10,
                216,
                248,
                13,
                197,
                229,
                916,
                95,
                934,
                915,
                923,
                937,
                928,
                936,
                931,
                920,
                926,
                27,
                198,
                230,
                223,
                201,
                32,
                33,
                34,
                35,
                164,
                37,
                38,
                39,
                40,
                41,
                42,
                43,
                44,
                45,
                46,
                47,
                48,
                49,
                50,
                51,
                52,
                53,
                54,
                55,
                56,
                57,
                58,
                59,
                60,
                61,
                62,
                63,
                161,
                65,
                66,
                67,
                68,
                69,
                70,
                71,
                72,
                73,
                74,
                75,
                76,
                77,
                78,
                79,
                80,
                81,
                82,
                83,
                84,
                85,
                86,
                87,
                88,
                89,
                90,
                196,
                214,
                209,
                220,
                167,
                191,
                97,
                98,
                99,
                100,
                101,
                102,
                103,
                104,
                105,
                106,
                107,
                108,
                109,
                110,
                111,
                112,
                113,
                114,
                115,
                116,
                117,
                118,
                119,
                120,
                121,
                122,
                228,
                246,
                241,
                252,
                224
            #endregion
            };

            return (char)lookupTable[(UInt16)gsm];
        }

        public static string ToUnicode(List<Byte> gsmBytes)
        {
            string result = string.Empty;
            char unicode = ' ';
            bool escaped = false;
            foreach (byte gsm in gsmBytes)
            {
                if (!escaped)
                {
                    switch (gsm)
                    {
                        case 0x1B:
                            escaped = true;
                            break;
                        default:
                            unicode = GSM2UnicodeLookup(gsm);
                            break;
                    }
                }
                else
                {
                    escaped = false;
                    switch (gsm)
                    {
                        #region Known escaped characters
                        case 0x0A: // FormFeed
                            unicode = (char)0x000C;
                            break;
                        case 0x14:
                            unicode = '^'; break;
                        case 0x28:
                            unicode = '{'; break;
                        case 0x29:
                            unicode = '}'; break;
                        case 0x2F:
                            unicode = '\\'; break;
                        case 0x3C:
                            unicode = '['; break;
                        case 0x3D:
                            unicode = '~'; break;
                        case 0x3E:
                            unicode = ']'; break;
                        case 0x40:
                            unicode = '|'; break;
                        case 0x65: // Euro-sign
                            unicode = (char)0x20AC;
                            break;
                        #endregion

                        default:
                            unicode = ' '; break; // unknown escaped character (prob. from lang pane);
                    }
                }
                if (!escaped) result += unicode;
            }
            return result;
        }
    }
}
