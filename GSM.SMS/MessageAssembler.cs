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

namespace GSM
{
    public class MessageAssembler : List<SMS>
    {
        public void AddTextMessage(Messages.TextMessage newMessage)
        {
            if (!newMessage.InParts) // complete single message
            {
                SMS completeMessage = new SMS(newMessage);
                this.Add(completeMessage);
            }
            else
            {
                bool existed = false;
                foreach (SMS sms in this)
                {
                    if (sms.MultiPartID == newMessage.MultiPartID) // another part of an existing message
                    {
                        existed = true;
                        bool newPart = true;
                        // check if we haven't received the part already in one way or the other
                        foreach (Messages.TextMessage existingPart in sms)
                        {
                            if (newMessage.Part == existingPart.Part) newPart = false;
                        }
                        if (newPart) sms.Add(newMessage); // else we have the part already
                        break;
                    }
                }
                if (! existed) // part of a new message
                {
                    SMS completeMessage = new SMS(newMessage);
                    this.Add(completeMessage);
                }
            }
        }

        public bool CompleteMessagesWaiting
        {
            get
            {
                foreach (SMS sms in this)
                {
                    if (sms.Complete) return true;
                }
                return false;
            }
        }

        public int CompleteMessagesCount
        {
            get
            {
                int count = 0;
                foreach (SMS sms in this)
                {
                    if (sms.Complete) count++;
                }
                return count;
            }
        }

        public int IncompleteMessagesCount
        {
            get
            {
                int count = 0;
                foreach (SMS sms in this)
                {
                    if (! sms.Complete) count++;
                }
                return count;
            }
        }

        public SmsList CompleteMessages
        {
            get
            {
                SmsList smsList = new SmsList();
                foreach (SMS sms in this)
                {
                    if (sms.Complete) smsList.Add(sms);
                }
                return smsList;
            }
        }

        public void ClearCompleteMessages()
        {
            if (this.Count > 0)
            {
                for (int i = this.Count - 1; i >= 0; i--)
                {
                    if (this[i].Complete) this.RemoveAt(i);
                }
            }
        }

        public SmsList PullCompleteMessages()
        {
            SmsList smsList = new SmsList();
            smsList = CompleteMessages;
            this.ClearCompleteMessages();
            return smsList;
        }
    }
}
