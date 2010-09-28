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
    public class SMS : Messages.TextMessageList
    {
        #region Constructors
        public SMS() : base()
        {
        }

        public SMS(Messages.TextMessage message) : base()
        {
            this.Add(message);
        }
        #endregion

        #region Methods
        new public void Sort()
        {
            base.Sort(Messages.MessagePartComparer.sortMessageParts());
        }
        #endregion

        #region Properties

        public bool InParts
        {
            get
            {
                if (this.Count < 1) return false;
                return this[0].InParts;
            }
        }

        public int Parts
        {
            get
            {
                if (this.Count < 1) return 0;
                return this[0].TotalParts;
            }
        }

        public int InPartsID
        {
            get
            {
                if (this.Count < 1) return 0;
                return this[0].InPartsID;
            }
        }

        public string MultiPartID
        {
            get
            {
                if (this.Count < 1) return null;
                return this[0].MultiPartID;
            }
        }

        public bool Complete
        {
            get
            {
                if (this.Count < 1) return false;
                if (!this[0].InParts) return true;
                return (this[0].TotalParts == this.Count);
            }
        }

        public string Message
        {
            get
            {
                string text = string.Empty;
                this.Sort();
                foreach (Messages.TextMessage message in this)
                {
                    text += message.Message;
                }
                return text;
            }
        }

        public string PhoneNumber
        {
            get
            {
                if (this.Count < 1) return "";
                return this[0].PhoneNumber;
            }
        }

        public DateTime TimeStamp
        {
            get
            {
                if (this.Count < 1) return DateTime.Now;
                return this[0].ServiceCenterTimeStamp;
            }
        }
        #endregion
    }
}
