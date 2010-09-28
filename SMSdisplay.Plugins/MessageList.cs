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
using System.Collections.ObjectModel;

namespace SMSdisplay.Plugins
{
    public class MessageList : ObservableCollection<Message>
    {
        public void AddUnique(Message item)
        {
            if (item != null)
            {
                if (!String.IsNullOrEmpty(item.MessageText))
                {
                    if (!this.Any(new Func<Message, bool>(delegate(Message message)
                        {
                            return (message.Date == item.Date) && (message.PhoneNumber == item.PhoneNumber) && (message.MessageText == item.MessageText);
                        })))
                    {
                        this.Add(item);
                    }
                }
            }
        }
    }
}
