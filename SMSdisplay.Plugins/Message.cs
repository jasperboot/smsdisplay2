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
using System.ComponentModel;

namespace SMSdisplay.Plugins
{
    public class Message : INotifyPropertyChanged
    {
        private string _messageText = "";
        private string _phoneNumber = "";
        private DateTime _date = DateTime.Now;
        private DateTime _downloadDate = DateTime.Now;

        public Message()
        {
//            this._messageText = "Systeem in werking";
        }

        public Message(string messageText, string phoneNumber, DateTime date, DateTime downloadDate)
        {
            this._messageText = messageText;
            this._phoneNumber = phoneNumber;
            this._date = date;
            this._downloadDate = downloadDate;
        }

        public Message(string messageText, string phoneNumber, DateTime date)
        {
            this._messageText = messageText;
            this._phoneNumber = phoneNumber;
            this._date = date;
        }

        public Message(string messageText, string phoneNumber)
        {
            this._messageText = messageText;
            this._phoneNumber = phoneNumber;
        }

        public Message(string messageText)
        {
            this._messageText = messageText;
        }

        public string MessageText
        {
            get { return _messageText; }
            set
            {
                _messageText = value;
                this.RaisePropertyChanged("MessageText");
            }
        }

        public string MessageTextSingleLine
        {
            get { return _messageText.Replace("\n", " / ");  }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
        }

        public DateTime Date
        {
            get { return _date; }
        }

        public DateTime DownloadDate
        {
            get { return _downloadDate; }
            set
            {
                _downloadDate = value;
                RaisePropertyChanged("DownloadDate");
            }
        }

        public string DateText
        {
            get { return _date.ToString("yyyy-MM-dd - HH:mm"); }
        }

        public override string ToString()
        {
            return this.MessageText;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}