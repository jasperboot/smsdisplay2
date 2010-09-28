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
using System.Threading;
using System.IO.Ports;
using GSM;
using GSM.AT;
using GSM.AT.Packets;
using GSM.Messages;

namespace GSM
{
    public class Communicator
    {
        public Communicator(string portName)
        {
            _portName = portName;
        }

        public Communicator()
        {
        }

        #region fields
        private System.Timers.Timer phoneTimer;
        private SerialConnection phoneConnection;
        private MessageAssembler messageAssembler = new MessageAssembler();

        private string _portName = "COM4";
        private bool _phoneInitialized = false;
        private int _messageTimer = 0;

        private bool _silentModeSupport = false;
        private SilentModeType _originalSilentValue = SilentModeType.Unknown;
        private object sendLock = new object();
        #endregion

        #region property fields
        private bool _checkForMessages = true;
        private bool _deleteReadMessages = false;

        private string _ident;
        private string _model;
        private string _number;
        private int _signalStrength = -1;
        private int _batteryCharge = -1;
        private bool _batteryCharging = false;
        private string _networkOperator;
        private int _storeCapacity = -1;
        private int _storeCount = -1;
        private ConnectionType _networkConnection;
        private SilentModeType _silentMode = SilentModeType.Unknown;
        #endregion

        #region Public Methods

        public void Connect()
        {

            _phoneInitialized = false;
            _checkForMessages = true;
            _silentModeSupport = false;
            _originalSilentValue = SilentModeType.Unknown;
            _messageTimer = 0;


            WriteTxDbg(String.Format("Connecting to phone ({0})...", _portName));
            phoneConnection = new SerialConnection(_portName);
            phoneConnection.PacketReceived += new EventHandler<PacketReceivedEventArgs>(processNewPacket);
            phoneConnection.Opened += new EventHandler(serialConnectionOpened);
            phoneConnection.Closed += new EventHandler(serialConnectionClosed);
            phoneConnection.Connect();

            phoneTimer = new System.Timers.Timer(700);
            phoneTimer.Elapsed += new System.Timers.ElapsedEventHandler(transmitterEvent);
            phoneTimer.Start();
        }

        public void Disconnect()
        {
            if (Connection)
            {
                WriteTxDbg("Disconnecting from phone...");
                phoneTimer.Stop();
                restoreSilentMode();
                sendCommand(ResetPacket.ActionCommand());
            }
        }

        #endregion

        #region Events

        public event EventHandler Closed;
        public event EventHandler Opened;
        public event EventHandler<NewDebugDataArgs> NewRxDebugData;
        public event EventHandler<NewDebugDataArgs> NewTxDebugData;
        public event EventHandler<IdentChangeEventArgs> IdentChange;
        public event EventHandler<ModelChangeEventArgs> ModelChange;
        public event EventHandler<NumberChangeEventArgs> NumberChange;
        public event EventHandler<SignalStrengthChangeEventArgs> SignalStrengthChange;
        public event EventHandler<BatteryChargeChangeEventArgs> BatteryChargeChange;
        public event EventHandler<BatteryChargingChangeEventArgs> BatteryChargingChange;
        public event EventHandler<NetworkOperatorChangeEventArgs> NetworkOperatorChange;
        public event EventHandler<NetworkConnectionChangeEventArgs> NetworkConnectionChange;
        public event EventHandler<SilentModeChangeEventArgs> SilentModeChange;
        public event EventHandler<NewMessageEventArgs> NewMessage;
        public event EventHandler<StoreCapacityChangeEventArgs> StoreCapacityChange;
        public event EventHandler<StoreCountChangeEventArgs> StoreCountChange;

        #endregion

        #region Properties

        public bool Connection
        {
            get
            {
                return (phoneConnection != null && phoneConnection.Connection);
            }
            set
            {
                if (value) Connect();
                else Disconnect();
            }
        }

        public string PortName
        {
            get { return _portName; }
            set
            {
                if (!Connection) _portName = value;
                else
                {
                    throw (new System.IO.IOException("Unable to change port name while serial connection is open"));
                }
            }
        }

        public bool CheckForMessages
        {
            get { return _checkForMessages; }
            set { _checkForMessages = value; }
        }

        public bool DeleteReadMessages
        {
            get { return _deleteReadMessages; }
            set { _deleteReadMessages = value; }
        }

        public string Ident
        {
            get { return _ident; }
            private set
            {
                if (_ident != value)
                {
                    _ident = value;
                    OnIdentChange(value);
                }
            }
        }

        public string Model
        {
            get { return _model; }
            private set
            {
                if (_model != value)
                {
                    _model = value;
                    OnModelChange(value);
                }
            }
        }

        public string Number
        {
            get { return _number; }
            private set
            {
                if (_number != value)
                {
                    _number = value;
                    OnNumberChange(value);
                }
            }
        }

        public int SignalStrength 
        {
            get { return _signalStrength; }
            private set
            {
                if (_signalStrength != value)
                {
                    _signalStrength = value;
                    OnSignalStrengthChange(value);
                }
            }
        }

        public int BatteryCharge
        {
            get { return _batteryCharge; }
            private set
            {
                if (_batteryCharge != value)
                {
                    _batteryCharge = value;
                    OnBatteryChargeChange(value);
                }
            }
        }

        public bool BatteryCharging
        {
            get { return _batteryCharging; }
            private set
            {
                if (_batteryCharging != value)
                {
                    _batteryCharging = value;
                    OnBatteryChargingChange(value);
                }
            }
        }

        public string NetworkOperator
        {
            get { return _networkOperator; }
            private set
            {
                if (_networkOperator != value)
                {
                    _networkOperator = value;
                    OnNetworkOperatorChange(value);
                }
            }
        }

        public ConnectionType NetworkConnection
        {
            get { return _networkConnection; }
            private set
            {
                if (_networkConnection != value)
                {
                    _networkConnection = value;
                    OnNetworkConnectionChange(value);
                }
            }
        }

        public SilentModeType SilentMode
        {
            get { return _silentMode; }
            private set
            {
                if (_silentMode != value)
                {
                    _silentMode = value;
                    OnSilentModeChange(value);
                }
            }
        }

        public int StoreCapacity
        {
            get { return _storeCapacity; }
            private set
            {
                if (_storeCapacity != value)
                {
                    _storeCapacity = value;
                    OnStoreCapacityChange(value);
                }
            }
        }

        public int StoreCount
        {
            get { return _storeCount; }
            private set
            {
                if (_storeCount != value)
                {
                    _storeCount = value;
                    OnStoreCountChange(value);
                }
            }
        }

        #endregion

        #region Private Methods

            #region Event Handlers

        private void serialConnectionOpened(object sender, EventArgs e)
        {
            OnOpened();
        }

        private void serialConnectionClosed(object sender, EventArgs e)
        {
            OnClosed();
        }
            #endregion

            #region Message Tx Loop

        private void transmitterEvent(object sender, EventArgs e)
        {
            if (!_phoneInitialized)
            {
                initializeConnection();
                checkReadMessages();
            }
            checkGsmStatus();
            if ((_checkForMessages) && (_messageTimer == 1))
            {
                checkMessages();
                if (_deleteReadMessages) removeMessages();
            }
            _messageTimer = (_messageTimer + 1) % 2;
            WriteTxDbg("Idle");
        }

        private void initializeConnection()
        {
            _phoneInitialized = true;
            phoneTimer.Interval = 10000;
            WriteTxDbg("Initialize phone...");
            sendCommand(IdentPacket.ActionCommand());
            Thread.Sleep(100);
            sendCommand(ModelPacket.ActionCommand());
            Thread.Sleep(100);
            sendCommand(NumberPacket.TestCommand());
            sendCommand(SamsungNumberPacket.TestCommand());
            sendCommand(SilentPacket.TestCommand());
            sendCommand(NumberPacket.ActionCommand());
            sendCommand(PrefMessageStorePacket.SetCommand(MessageStore.Phone));
            saveSilentMode();
        }

        private void checkGsmStatus()
        {
            WriteTxDbg("Requesting status...");
            sendCommand(OperatorPacket.ReadCommand());
            sendCommand(BatteryPacket.ActionCommand());
            sendCommand(SignalPacket.ActionCommand());
            if (_silentModeSupport) sendCommand(SilentPacket.ReadCommand());
            if (String.IsNullOrEmpty(Number))
            {
                switch (Ident.ToLowerInvariant())
                {
                    case "samsung":
                        sendCommand(SamsungNumberPacket.ReadCommand());
                        break;
                    default:
                        sendCommand(NumberPacket.ActionCommand());
                        break;
                }
            }
            sendCommand(PrefMessageStorePacket.ReadCommand());
        }

        private void checkReadMessages()
        {
            WriteTxDbg("Checking for read messages...");
            sendCommand(MessageListPacket.ActionCommand(MessageType.Read));
        }

        private void checkMessages()
        {
            WriteTxDbg("Checking for new messages...");
            sendCommand(MessageListPacket.ActionCommand(MessageType.Unread));
        }

        private void removeMessages()
        {
            WriteTxDbg("Removing read messages...");
            sendCommand(MessageDeletePacket.ActionCommand(0,DelFlagType.Read));
            WriteTxDbg("Requesting new inbox status...");
            sendCommand(PrefMessageStorePacket.ReadCommand());
        }

        private void saveSilentMode()
        {
            if (_silentModeSupport)
            {
                WriteTxDbg("* Saving silent mode setting...");
                sendCommand(SilentPacket.ReadCommand());
                sendCommand(SilentPacket.SetCommand(true));
            }
        }

        private void restoreSilentMode()
        {
            if (_silentModeSupport && Connection && (_originalSilentValue != SilentModeType.Unknown))
            {
                WriteTxDbg("* Restoring silent mode setting...");
                sendCommand(SilentPacket.SetCommand(_originalSilentValue == SilentModeType.On ? true : false));
            }
        }

        private void sendCommand(string command)
        {
//            Monitor.Enter(sendLock);
            if (Connection)
                phoneConnection.SendCommand(command);
//            Monitor.Exit(sendLock);
        }

            #endregion

            #region Message Receiver

        private void processNewPacket(object sender, PacketReceivedEventArgs e)
        {
            GenericPacket packet = e.Packet;
            // Lowlevel debug
            // processUnknownPacket(packet);
            WritePacketDbg(packet.DebugText);
            switch (packet.GetType().Name)
            {
                case "ResetPacket":
                    phoneConnection.Disconnect();
                    WriteTxDbg("Disconnected...");
                    Ident = "";
                    Model = "";
                    Number = "";
                    SignalStrength = -1;
                    BatteryCharge = -1;
                    BatteryCharging = false;
                    NetworkOperator = "";
                    NetworkConnection = ConnectionType.GSM;
                    SilentMode = SilentModeType.Unknown;
                    StoreCapacity = -1;
                    StoreCount = -1;
                    break;
                case "IdentPacket":
                    if (packet.Type == PacketType.Action)
                    {
                        Ident = (packet as IdentPacket).Identification;
                    }
                    break;
                case "ModelPacket":
                    if (packet.Type == PacketType.Action)
                    {
                        Model = (packet as ModelPacket).Model;
                    }
                    break;
                case "NumberPacket":
                    if (packet.Type == PacketType.Action)
                    {
                        string number = (packet as NumberPacket).PhoneNumber;
                        if (!String.IsNullOrEmpty(number)) Number = number;
                    }
                    break;
                case "SamsungNumberPacket":
                    if (packet.Type == PacketType.Read)
                    {
                        string number = (packet as SamsungNumberPacket).PhoneNumber;
                        if (!String.IsNullOrEmpty(number)) Number = number;
                    }
                    break;
                case "PrefMessageStorePacket":
                    if (packet.Type == PacketType.Read)
                    {
                        if ((packet as PrefMessageStorePacket).CurrentStore == MessageStore.Phone)
                        {
                            StoreCapacity = (packet as PrefMessageStorePacket).MessageCapacity;
                            StoreCount = (packet as PrefMessageStorePacket).MessageCount;
                        }
                    }
                    break;
                case "SilentPacket":
                    _silentModeSupport = packet.Supported;
                    if (packet.Type == PacketType.Read)
                    {
                        if (_originalSilentValue == SilentModeType.Unknown) _originalSilentValue = ((packet as SilentPacket).SilentMode ? SilentModeType.On : SilentModeType.Off);
                        SilentMode = (packet as SilentPacket).SilentMode ? SilentModeType.On : SilentModeType.Off;
                    }
                    break;
                case "SignalPacket":
                    if (packet.Type == PacketType.Action) SignalStrength = (packet as SignalPacket).SignalQuality;
                    break;
                case "BatteryPacket":
                    if (packet.Type == PacketType.Action)
                    {
                        BatteryCharge = (packet as BatteryPacket).BatteryCharge;
                        BatteryCharging = (packet as BatteryPacket).Charging;
                    }
                    break;
                case "OperatorPacket":
                    if (packet.Type == PacketType.Read)
                    {
                        NetworkOperator = (packet as OperatorPacket).Operator;
                        NetworkConnection = (packet as OperatorPacket).Connection;
                    }
                    break;
                case "MessageReadPacket":
                case "MessageListPacket":
                    processShortMessages((packet as SMContainingPacket).Messages);
                    break;
                case "MessageDeletePacket":
                    break;
                default:
                    processUnknownPacket(packet);
                    break;
            }
        }

            #endregion

            #region Specific packet Rx handling

        private void processUnknownPacket(GSM.AT.Packets.GenericPacket packet)
        {
            string debugHeader = "  Response header: \t{0}";
            string debugData = "  Clean data text: \t{0}";
            string debugRawData = "  Raw unparsed data: \t{0}";
            string rawData, responseHeader, responseData;
            for (int i = 0; i < packet.ResponseData.Count; i++)
            {
                rawData = packet.ResponseData[i];
                responseHeader = GenericPacket.Response.GetResponseHeader(rawData);
                responseHeader = (responseHeader == "") ? "<no response type returned>" : responseHeader;
                responseData = GenericPacket.Response.GetResponseDataString(rawData) ?? "";
                WritePacketDbg(String.Format(debugHeader, responseHeader));
                WritePacketDbg(String.Format(debugData, responseData));
                WritePacketDbg(String.Format(debugRawData, rawData));
            }
        }

        private void processShortMessages(TextMessageList shortMessages)
        {
            if (shortMessages.Count > 0)
            {
                foreach (TextMessage shortMessage in shortMessages)
                {
                    messageAssembler.AddTextMessage(shortMessage);
                }
            }
            int newMessageCount = messageAssembler.CompleteMessagesCount;
            WritePacketDbg(string.Format("New complete messages: \t{0} (excl. {1} incomplete)", (newMessageCount > 0) ? newMessageCount.ToString() : "None", messageAssembler.IncompleteMessagesCount));
            if (messageAssembler.CompleteMessagesWaiting)
            {
                int i = 1;
                SmsList completeMessages = messageAssembler.PullCompleteMessages();
                string debugMessage = "- Message {0}:";
                string debugSender = "  Sender phone number: \t{0}";
                string debugTimeStamp = "  DateTimestamp: \t{0}";
                string debugText = "  Message text: \t{0}";
                string debugMulti = "  Multipart ID: \t{1} ({0} parts)";
                foreach (SMS message in completeMessages)
                {
                    WritePacketDbg(String.Format(debugMessage, i));
                    WritePacketDbg(String.Format(debugSender, message.PhoneNumber));
                    WritePacketDbg(String.Format(debugTimeStamp, message.TimeStamp.ToString("u")));
                    WritePacketDbg(String.Format(debugText, message.Message));
                    if (message.InParts)
                        WritePacketDbg(String.Format(debugMulti, message.Parts, message.InPartsID));
                    OnNewMessage(message.Message, message.PhoneNumber, message.TimeStamp);
                    i++;
                }
            }
        }
        #endregion

            #region Event Raisers

        private void OnOpened()
        {
                if (this.Opened != null) Opened(this, new EventArgs());
        }

        private void OnClosed()
        {
            if (this.Closed != null) Closed(this, new EventArgs());
        }

        private void WritePacketDbg(string data)
        {
            if (this.NewRxDebugData != null) NewRxDebugData(this, new NewDebugDataArgs(data));
        }

        private void WriteTxDbg(string data)
        {
            if (this.NewTxDebugData != null) NewTxDebugData(this, new NewDebugDataArgs(data));
        }

        private void OnSignalStrengthChange(int signalStrength)
        {
            if (this.SignalStrengthChange != null) SignalStrengthChange(this, new SignalStrengthChangeEventArgs(signalStrength));
        }

        private void OnIdentChange(string ident)
        {
            if (this.IdentChange != null) IdentChange(this, new IdentChangeEventArgs(ident));
        }

        private void OnModelChange(string model)
        {
            if (this.ModelChange != null) ModelChange(this, new ModelChangeEventArgs(model));
        }

        private void OnNumberChange(string number)
        {
            if (this.NumberChange != null) NumberChange(this, new NumberChangeEventArgs(number));
        }

        private void OnBatteryChargeChange(int batteryCharge)
        {
            if (this.BatteryChargeChange != null) BatteryChargeChange(this, new BatteryChargeChangeEventArgs(batteryCharge));
        }

        private void OnBatteryChargingChange(bool batteryCharging)
        {
            if (this.BatteryChargingChange != null) BatteryChargingChange(this, new BatteryChargingChangeEventArgs(batteryCharging));
        }

        private void OnNetworkOperatorChange(string networkOperator)
        {
            if (this.NetworkOperatorChange != null) NetworkOperatorChange(this, new NetworkOperatorChangeEventArgs(networkOperator));
        }

        private void OnNetworkConnectionChange(ConnectionType networkConnection)
        {
            if (this.NetworkConnectionChange != null) NetworkConnectionChange(this, new NetworkConnectionChangeEventArgs(networkConnection));
        }

        private void OnSilentModeChange(SilentModeType silentMode)
        {
            if (this.SilentModeChange != null) SilentModeChange(this, new SilentModeChangeEventArgs(silentMode));
        }

        private void OnNewMessage(string message, string phoneNumber, DateTime timeStamp)
        {
            if (this.NewMessage != null) NewMessage(this, new NewMessageEventArgs(message, phoneNumber, timeStamp));
        }

        private void OnStoreCapacityChange(int storeCapacity)
        {
            if (this.StoreCapacityChange != null) StoreCapacityChange(this, new StoreCapacityChangeEventArgs(storeCapacity));
        }

        private void OnStoreCountChange(int storeCount)
        {
            if (this.StoreCountChange != null) StoreCountChange(this, new StoreCountChangeEventArgs(storeCount));
        }

            #endregion

        #endregion

        #region Public Statics

        public static SerialPortList GetPorts()
        {
            return SerialConnection.GetPortList();
        }

        #endregion
    }
}
