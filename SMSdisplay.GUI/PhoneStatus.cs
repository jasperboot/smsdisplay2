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
using System.ComponentModel;

namespace SMSdisplay.GUI
{
    public class PhoneStatus : INotifyPropertyChanged
    {

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private string _ident = "";
        private string _model = "";
        private string _number = "";
        private string _network = "";
        private string _type = "";
        private int _signal = 0;
        private int _battery = 0;
        private int _inbox = 0;
        private int _inboxMax = 100;
        private string _statusMessage = "";

        public string Ident
        {
            get { return _ident; }
            set { _ident = value; RaisePropertyChanged("Ident"); }
        }

        public string Model
        {
            get { return _model; }
            set { _model = value; RaisePropertyChanged("Model"); }
        }

        public string Number
        {
            get { return _number; }
            set { _number = value; RaisePropertyChanged("Number"); }
        }

        public string Network
        {
            get { return _network; }
            set { _network = value; RaisePropertyChanged("Network"); }
        }
        
        public string ConnectionType
        {
            get { return _type; }
            set { _type = value; RaisePropertyChanged("ConnectionType"); }
        }

        public int Signal
        {
            get { return _signal; }
            set { _signal = value; RaisePropertyChanged("Signal"); }
        }

        public int Battery
        {
            get { return _battery; }
            set { _battery = value; RaisePropertyChanged("Battery"); }
        }

        public int InboxCount
        {
            get { return _inbox; }
            set { _inbox = value; RaisePropertyChanged("InboxCount"); }
        }

        public int InboxCapacity
        {
            get { return _inboxMax; }
            set { _inboxMax = value; RaisePropertyChanged("InboxCapacity"); }
        }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set { _statusMessage = value; RaisePropertyChanged("StatusMessage"); }
        }
    }

}
