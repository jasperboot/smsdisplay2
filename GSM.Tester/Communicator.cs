using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO.Ports;
using GSM;
using GSM.AT;
using GSM.AT.Packets;
using GSM.Messages;


namespace GSM.Tester
{
    public class Communicator
    {
        #region Public Methods

        public void Connect()
        {

            _comPortInUse = false;
            _phoneInitialized = false;
            _checkForMessages = true;
            _wantConnection = true;
            _silentModeSupport = false;
            _originalSilentValue = SilentModeType.Unknown;
            _messageTimer = 0;

            WriteTxDbg(String.Format("Connecting to phone ({0})...", _portName));
            comPort = new SerialPort(_portName);
            comPort.BaudRate = 19200;
            comPort.NewLine = "\r\n";
            comPort.ReadTimeout = 8000;
            comPort.WriteBufferSize = 8192;
            comPort.DataReceived += new SerialDataReceivedEventHandler(processNewData);
            if (!comPort.IsOpen) comPort.Open();

            phoneTimer = new System.Timers.Timer(700);
            phoneTimer.Elapsed += new System.Timers.ElapsedEventHandler(transmitterEvent);
            phoneTimer.Start();
        }

        public void Disconnect()
        {
            WriteTxDbg("Disconnecting from phone...");
            _wantConnection = false;
            phoneTimer.AutoReset = false;
            phoneTimer.Interval = 500;
        }

        public void SendCommand(string command)
        {
            sendCommand(command);
        }
        #endregion

        #region Events

        public event EventHandler<DebugData> NewRxDebugData;
        public event EventHandler<DebugData> NewTxDebugData;

        #endregion

        #region Properties

        public bool Connection
        {
            get
            {
                return (comPort != null && comPort.IsOpen);
            }
            set
            {
                if (value) Connect();
                else Disconnect();
            }
        }

        public bool CheckForMessages
        {
            get { return _checkForMessages; }
            set { _checkForMessages = value; }
        }
        #endregion

        #region Private Methods
        
            #region Message Tx Loop

        private void transmitterEvent(object sender, EventArgs e)
        {
            if (!_phoneInitialized) initializeConnection();
            if (_wantConnection)
            {
                checkGsmStatus();
                if ((_checkForMessages) && (_messageTimer == 1))
                {
                    checkMessages();
                    removeMessages();
                }
            }
            else
            {
                closeConnection();
            }
            _messageTimer = (_messageTimer + 1) % 2;
        }

        private void initializeConnection()
        {
            _phoneInitialized = true;
            phoneTimer.Interval = 10000;
            WriteTxDbg("Initialize phone...");
            sendCommand(IdentPacket.ActionCommand());
            sendCommand(ModelPacket.ActionCommand());
            sendCommand(NumberPacket.ActionCommand());
            sendCommand(SilentPacket.TestCommand());
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
        }

        private void checkMessages()
        {
            WriteTxDbg("Checking for new messages...");
            sendCommand(MessageListPacket.ActionCommand(MessageType.Unread));
        }

        private void removeMessages()
        {
            WriteTxDbg("Removing read messages...");
            sendCommand(MessageDeletePacket.ActionCommand(0,DelFlagType.SingleMessage));
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
                sendClosingCommand(SilentPacket.SetCommand(_originalSilentValue == SilentModeType.On ? true : false));
            }
        }

        private void closeConnection()
        {
            restoreSilentMode();
            comPort.Close();
            comPort.Dispose();
            WriteTxDbg("Disconnected...");
        }

        private void sendCommand(string command)
        {
            while (_comPortInUse) Thread.Sleep(sendWait);
            if (_wantConnection && Connection)
            {
                if (command.StartsWith("AT+")) _comPortInUse = true; //only block port if we're sure we'll get into the process AT buffer loop... hacky ;-)
                comPort.WriteLine(command);
            }
        }

        private void sendClosingCommand(string command)
        {
            while (_comPortInUse) Thread.Sleep(sendWait);
            if (Connection)
            {
                _comPortInUse = true;
                comPort.WriteLine(command);
            }
        }
            #endregion

            #region Message Rx Loop

        private void processNewData(object sender, SerialDataReceivedEventArgs e)
        {
            string readLine;
            while (comPort.BytesToRead > 0)
            {
                //try
                //{
                readLine = comPort.ReadLine();
                // Empty lines and unsollicited RING return codes are ommited
                if ((readLine != "") && (readLine != "RING")) atBuffer.Add(readLine);
                switch (readLine)
                {
                    case "OK":              // Operation completed, succesfully
                    case "ERROR":
                        processAtBuffer();  // Operation completed, with errors

                        break;
                    default:
                        if (readLine.StartsWith("+CMS ERROR"))  // Operation completed, with errors
                        {
                            processAtBuffer();
                        }
                        break;
                }
                //}
                //catch {}
                _bytesToRead = comPort.BytesToRead;
            }
        }

        private void processAtBuffer()
        {
            _comPortInUse = false;
            while ((atBuffer.Count > 0) && (!atBuffer[0].EndsWith("\r")))
            {
                Console.WriteLine(string.Format("Dropping line noise: {0}", atBuffer[0]));
                atBuffer.RemoveAt(0);
            }
            GenericPacket packet = GenericPacket.FromData(atBuffer);
            WritePacketDbg(packet.DebugText);
            switch (packet.GetType().Name)
            {
                case "ModelPacket":
                    break;
                case "IdentPacket":
                    break;
                case "NumberPacket":
                    break;
                case "PrefMessageStorePacket":
                    break;
                case "SilentPacket":
                    _silentModeSupport = packet.Supported;
                    if ((_originalSilentValue == SilentModeType.Unknown) && packet.Type == PacketType.Read)
                        _originalSilentValue = ((packet as SilentPacket).SilentMode ? SilentModeType.On : SilentModeType.Off);
                    break;
                case "SignalPacket":
                    break;
                case "BatteryPacket":
                    break;
                case "OperatorPacket":
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
            atBuffer.Clear();
        }
        #endregion

            #region Specific packet Rx handling

        private void processUnknownPacket(GenericPacket packet)
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
                responseData = GenericPacket.Response.GetResponseDataString(rawData);
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
                string debugText = "  Message text: \t{0}";
                string debugMulti = "  Multipart ID: \t{1} ({0} parts)";
                foreach (SMS message in completeMessages)
                {
                    WritePacketDbg(String.Format(debugMessage, i));
                    WritePacketDbg(String.Format(debugSender, message.PhoneNumber));
                    WritePacketDbg(String.Format(debugText, message.Message));
                    if (message.InParts)
                        WritePacketDbg(String.Format(debugMulti, message.Parts, message.InPartsID));
                    i++;
                }
            }
        }
        #endregion

            #region Event Raisers

        private void WritePacketDbg(string data)
        {
            if (this.NewRxDebugData != null) NewRxDebugData(this, new DebugData(data));
        }

        private void WriteTxDbg(string data)
        {
            if (this.NewTxDebugData != null) NewTxDebugData(this, new DebugData(data));
        }

            #endregion

        #endregion

        #region constants
        private const int sendWait = 100;
        #endregion

        #region internal property fields
        private string _portName = "COM4";
        private int _bytesToRead = 0;
        #endregion

        #region internal vars
        private bool _comPortInUse = false;
        private bool _phoneInitialized = false;
        private bool _checkForMessages = true;
        private bool _wantConnection = false;
        private bool _silentModeSupport = false;
        private SilentModeType _originalSilentValue = SilentModeType.Unknown;
        private int _messageTimer = 0;
        #endregion

        #region members
        private System.Timers.Timer phoneTimer;
        private SerialPort comPort;
        private MessageAssembler messageAssembler = new MessageAssembler();
        private List<string> atBuffer = new List<string>(6);
        #endregion

    }
}
