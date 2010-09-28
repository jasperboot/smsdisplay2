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
using System.Text;

namespace GSM.Messages
{
    public class MessagePartComparer : IComparer<TextMessage>
    {
            int IComparer<TextMessage>.Compare(TextMessage m1, TextMessage m2)
            {
                //ShortMessage m1 = (ShortMessage)a;
                //ShortMessage m2 = (ShortMessage)b;
                if ((!m1.InParts) || (!m2.InParts)) return 0;
                if (m1.InPartsID != m2.InPartsID) return 0;
                if (m1.Part > m2.Part)
                    return 1;
                if (m1.Part < m2.Part)
                    return -1;
                else
                    return 0;
            }

            public static IComparer<TextMessage> sortMessageParts()
            {
                return new MessagePartComparer();
            }

    }
}
