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
namespace GSM
{
    public class NewDebugDataArgs : EventArgs
    {
        private string msg;

        public NewDebugDataArgs(string messageData)
        {
            msg = messageData;
        }

        public string Message
        {
            get { return msg; }
        }
    }

    public class IdentChangeEventArgs : EventArgs
    {
        private string _ident;

        public IdentChangeEventArgs(string ident)
        {
            _ident = ident;
        }

        public string Ident { get { return _ident; } }
    }

    public class ModelChangeEventArgs : EventArgs
    {
        private string _model;

        public ModelChangeEventArgs(string model)
        {
            _model = model;
        }

        public string Model { get { return _model; } }
    }

    public class NumberChangeEventArgs : EventArgs
    {
        private string _number;

        public NumberChangeEventArgs(string number)
        {
            _number = number;
        }

        public string Number { get { return _number; } }
    }

    public class SignalStrengthChangeEventArgs : EventArgs
    {
        private int _signalStrength;

        public SignalStrengthChangeEventArgs(int signalStrength)
        {
            _signalStrength = signalStrength;
        }

        public int SignalStrength { get { return _signalStrength; } }
    }

    public class BatteryChargeChangeEventArgs : EventArgs
    {
        private int _batteryCharge;

        public BatteryChargeChangeEventArgs(int batteryCharge)
        {
            _batteryCharge = batteryCharge;
        }

        public int BatteryCharge { get { return _batteryCharge; } }
    }

    public class BatteryChargingChangeEventArgs : EventArgs
    {
        private bool _batteryCharging;

        public BatteryChargingChangeEventArgs(bool batteryCharging)
        {
            _batteryCharging = batteryCharging;
        }

        public bool BatteryCharging { get { return _batteryCharging; } }
    }

    public class NetworkOperatorChangeEventArgs : EventArgs
    {
        private string _networkOperator;

        public NetworkOperatorChangeEventArgs(string networkOperator)
        {
            _networkOperator = networkOperator;
        }

        public string NetworkOperator { get { return _networkOperator; } }
    }

    public class NetworkConnectionChangeEventArgs : EventArgs
    {
        private ConnectionType _networkConnection;

        public NetworkConnectionChangeEventArgs(ConnectionType networkConnection)
        {
            _networkConnection = networkConnection;
        }

        public ConnectionType NetworkConnection { get { return _networkConnection; } }
    }

    public class SilentModeChangeEventArgs : EventArgs
    {
        private SilentModeType _silentMode = SilentModeType.Unknown;

        public SilentModeChangeEventArgs(SilentModeType silentMode)
        {
            _silentMode = silentMode;
        }

        public SilentModeType SilentMode { get { return _silentMode; } }
    }

    public class NewMessageEventArgs : EventArgs
    {
        private string _message;
        private string _phoneNumber;
        private DateTime _timeStamp;

        public NewMessageEventArgs(string message, string phoneNumber, DateTime timeStamp)
        {
            _message = message;
            _phoneNumber = phoneNumber;
            _timeStamp = timeStamp;
        }

        public string Message { get { return _message; } }
        public string PhoneNumber { get { return _phoneNumber; } }
        public DateTime TimeStamp { get { return _timeStamp; } } 
    }

    public class StoreCapacityChangeEventArgs : EventArgs
    {
        private int _storeCapacity;

        public StoreCapacityChangeEventArgs(int storeCapacity)
        {
            _storeCapacity = storeCapacity;
        }

        public int StoreCapacity { get { return _storeCapacity; } }
    }

    public class StoreCountChangeEventArgs : EventArgs
    {
        private int _storeCount;

        public StoreCountChangeEventArgs(int storeCount)
        {
            _storeCount = storeCount;
        }

        public int StoreCount { get { return _storeCount; } }
    }

}