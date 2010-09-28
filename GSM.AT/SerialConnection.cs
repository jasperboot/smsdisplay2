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
using GSM.AT.Packets;
using System.Management;
using System.Text.RegularExpressions;

namespace GSM.AT
{
    
    public class SerialConnection
    {
        public SerialConnection(string portName)
        {
            _portName = portName;
        }

        public SerialConnection()
        {
        }

        #region constants
        private const int sendWait = 20;
        #endregion

        #region fields
        private SerialPort comPort;
        private SerialBuffer atBuffer = new SerialBuffer(6);

        private string _portName = "COM4";
        private volatile bool _comPortInUse = false;
        #endregion

        #region Public Methods

        public void Connect()
        {

            _comPortInUse = false;

            comPort = new SerialPort(_portName);
            comPort.BaudRate = 19200;
            comPort.NewLine = "\r\n";
            comPort.ReadTimeout = 8000;
            comPort.ReadBufferSize = 32768;
            comPort.WriteBufferSize = 8192;
            comPort.DataReceived += new SerialDataReceivedEventHandler(processNewData);
            bool errorOccurred = false;
            try
            {
                if (!comPort.IsOpen) comPort.Open();
                GC.SuppressFinalize(comPort.BaseStream);
            }
            catch (Exception e)
            {
                errorOccurred = true;
                throw (e);
            }
            if (!errorOccurred) OnOpened();
        }

        public void Disconnect()
        {
            lock (comPort)
            {
                if (comPort.IsOpen)
                {
                    try
                    {
                        comPort.Close();
                    }
                    catch (UnauthorizedAccessException e) { }
                    catch (ObjectDisposedException e)
                    {   // Stupid thingy that when unplugging USB, a safehandle error is being generated (object already disposed)
                        // This is a Microsoft bug and we can only catch it in an Application form... :-(
                    }
                    catch { }
                }
            }
            OnClosed();
        }

        public void SendCommand(string command)
        {
            sendCommand(command);
        }
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

        #endregion

        #region Events

        public event EventHandler<PacketReceivedEventArgs> PacketReceived;
        public event EventHandler Closed;
        public event EventHandler Opened;

        #endregion

        #region Private Methods

            #region sendCommand

        private void sendCommand(string command)
        {
            while (_comPortInUse) Thread.Sleep(sendWait);
            if (Connection)
            {
                _comPortInUse = true;
                try
                {
                    comPort.WriteLine(command);
                }
                catch
                {
                    _comPortInUse = false;
                    Disconnect();
                }
            }
        }

            #endregion

            #region Message Rx Loop

        private void processNewData(object sender, SerialDataReceivedEventArgs e)
        {
            string readLine;
            bool bufferFinal = false;
            while (Connection && (!bufferFinal || (comPort.BytesToRead > 0)))
            {
                try
                {
                    readLine = comPort.ReadLine();
                    // Empty lines and unsollicited RING return codes are ommited
                    if ((readLine != "") && (readLine != "RING")) atBuffer.Add(readLine);
                    switch (readLine)
                    {
                        case "OK":              // Operation completed, succesfully
                        case "ERROR":           // Operation completed, with errors
                            bufferFinal = true;
                            break;

                        default:
                            if (readLine.StartsWith("+CMS ERROR"))  // Operation completed, with errors
                            {
                                bufferFinal = true;
                            }
                            break;

                    }
                }
                catch (TimeoutException)
                {
                    // The command sent to the phone wasn't recognised by it as AT command, so we won't
                    // even get an error response. Release the COMport for sending...
                    _comPortInUse = false;
                    break;
                }
                if (bufferFinal) processAtBuffer();
            }
        }

        private void processAtBuffer()
        {
            SerialBuffer buf = atBuffer.Clone();
            atBuffer.Clear();
            _comPortInUse = false;
            while ((buf.Count > 0) && (!buf[0].EndsWith("\r")))
            {
                Console.WriteLine(string.Format("Dropping line noise: {0}", buf[0]));
                buf.RemoveAt(0);
            }
            if (buf.Count > 0)
            {
                GenericPacket packet = GenericPacket.FromData(buf);
                
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object threadContext) {
                        OnPacketReceived(packet);
                    }));
                
                /*
                Thread test = new Thread(new ThreadStart(delegate() {
                        OnPacketReceived(packet);
                }));
                test.Start();
                */
                // OnPacketReceived(packet);
//                Thread.Sleep(1);
            }
        }
            #endregion

            #region Event Raisers

        private void OnPacketReceived(GenericPacket packet)
        {
            if (this.PacketReceived != null) PacketReceived(this, new PacketReceivedEventArgs(packet));
        }

        private void OnOpened()
        {
            if (this.Opened != null) Opened(this, new EventArgs());
        }

        private void OnClosed()
        {
            if (this.Closed != null) Closed(this, new EventArgs());
        }

            #endregion

        #endregion

        #region Public Statics

        public static string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }

        private static SerialPortList getSerialPorts()
        {
            var portList = new SerialPortList();
            ManagementObjectSearcher searcher =
              new ManagementObjectSearcher("Select DeviceID, Description, PNPDeviceID, MaxBaudRate from Win32_SerialPort");
            foreach (ManagementObject device in searcher.Get())
            {
                string portName = (device.GetPropertyValue("DeviceID") as String);
                string description = (device.GetPropertyValue("Description") as String);
                description = Regex.Replace(description, @"Device \d+ +(USB) WMC",@"$1");
                string pnpID = (device.GetPropertyValue("PNPDeviceID") as String);
                SerialPortType deviceType = (Regex.IsMatch(pnpID, "^USB") ? SerialPortType.USB : SerialPortType.RS232);
                if (description.IndexOf("Modem") > -1) deviceType = SerialPortType.Modem;
                bool availabilty = (device.GetPropertyValue("MaxBaudRate") != null);
                var port = new SerialPortInfo();
                port.Name = portName; 
                port.Description = description;
                port.Availability = availabilty;
                port.Type = deviceType;
                portList.Add(port);
            }
            return portList;
        }

        public static SerialPortList GetPortList()
        {
            SerialPortList portList = getSerialPorts();
            for (int i = 0; i < portList.Count; i++)
            {
                SerialPortInfo port = portList[i];
                string portName = port.Name;
                try
                {
                    SerialPort connection = new SerialPort(portName);
                    connection.BaudRate = 19200;
                    connection.NewLine = "\r\n";
                    connection.ReadTimeout = 500;
                    connection.Open();
                    if (connection.IsOpen)
                    {
                        GC.SuppressFinalize(connection.BaseStream); // Dirty hack for WPF application
                        System.IO.Stream baseStream = connection.BaseStream;
                        connection.WriteLine(AT.Packets.MessageListPacket.TestCommand());
                        string readLine = connection.ReadLine();
                        while ((readLine != "OK") && (readLine != "ERROR"))
                        {
                            readLine = connection.ReadLine();
                        }
                        if (readLine == "OK") port.Type = SerialPortType.Phone;
                        connection.Close();
                        baseStream.Dispose();
                    }
                }
                catch (UnauthorizedAccessException e)
                {
                    if (port.Availability) {
                        Console.WriteLine(String.Format("Port {0} was told to be available, but it isn't", port.Name));
                        port.Availability = false;
                    }
                }
                catch (System.IO.IOException e) { } // Port doesn't understand I/O, keep IsPhone=false
                catch (TimeoutException e) { }  // Port hasn't got a modem interface, keep IsPhone=false
                finally
                {
                    Console.WriteLine(port);
                }
            }
            return portList;
        }

        #endregion
    }
}
