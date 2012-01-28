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
using SMSdisplay.Plugins;
using SMSdisplay.GUI;
using System.Xml;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace SMSdisplay
{
    public class Presenter : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private volatile bool _sendInfoShown = false;
        private System.Timers.Timer queueTimer;
        private System.Timers.Timer approvalTimer;

        int currentArchiveMessage = 0;
        int lastSendInformation = 0;

        public event EventHandler MessageSwitcherStopped;
        public event EventHandler MessageSwitcherStarted;
        public event EventHandler PluginShown;
        public event EventHandler PluginChanged;
        public event EventHandler PluginHidden;
        public event EventHandler PresentationChanged;
        private delegate void ReceiveEventHandler();

        private Dispatcher Dispatcher;

        public Presenter(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        public void Initialize()
        {
            Status = "Starting... Initializing message queue timer";
            queueTimer = new System.Timers.Timer(2000);
            queueTimer.AutoReset = true;
            queueTimer.Elapsed += new System.Timers.ElapsedEventHandler(queueTimer_Elapsed);
            Status = "Starting... Initializing auto-approval timer";
            approvalTimer = new System.Timers.Timer(1000);
            approvalTimer.AutoReset = true;
            approvalTimer.Elapsed += new System.Timers.ElapsedEventHandler(approvalTimer_Elapsed);
            Status = "Starting... Loading presentation plugins";
            loadPlugins();
            Status = "Starting... Loading presentation plugin themese";
            loadThemes();
            Status = "Starting... Enumerating screens";
            loadScreens();
            Status = "Starting... Restoring message history";
            loadMessages();
            Status = "Presenter initialized";
        }

        public void Close()
        {
            if (MessageSwitching)
            {
                Status = "Closing...  Stopping message switcher";
                MessageSwitching = false;
            }
            Status = "Closing...  Presentation plugins";
            closePlugins();
            Status = "Closing...  Saving messages";
            closeMessages();
        }

        private void loadPlugins()
        {
            Plugins = PluginLoader.LoadPlugins(@"PresentationPlugins");
            initializePlugins();
        }

        private void loadThemes()
        {
            Themes = PluginTheme.GetPluginThemes();
        }

        private void initializePlugins()
        {
            foreach (IPlugin plugin in Plugins)
            {
                plugin.Initialize();
                Console.WriteLine("Presentation plugin initialized: {0} - {1}", plugin.Name, plugin.Description);
            }
        }

        private void closePlugins()
        {
            foreach (IPlugin plugin in Plugins)
            {
                plugin.Cleanup();
            }
        }

        private void loadScreens()
        {
            ScreenList = OutputScreen.AllOutputScreens;
        }

        private void loadMessages()
        {
            loadMessages(ArchivedMessages, "archived.xml");
            loadMessages(ApprovedMessages, "approved.xml");
            loadMessages(NewMessages, "new.xml");
            loadMessages(RejectedMessages, "rejected.xml");
        }

        private void closeMessages()
        {
            saveMessages(ArchivedMessages, "archived.xml");
            saveMessages(ApprovedMessages, "approved.xml");
            saveMessages(NewMessages, "new.xml");
            saveMessages(RejectedMessages, "rejected.xml");
        }

        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                this.RaisePropertyChanged("Status");
            }
        }

        private MessageList shownMessages;
        public MessageList ShownMessages
        {
            get { return shownMessages; }
            private set
            {
                shownMessages = value;
                this.RaisePropertyChanged("ShownMessages");
            }
        }

        public MessageList NewMessages = new MessageList();
        public MessageList ApprovedMessages = new MessageList();
        public MessageList RejectedMessages = new MessageList();
        public MessageList ArchivedMessages = new MessageList();

        public List<IPlugin> Plugins = new List<IPlugin>();
        public List<PluginTheme> Themes = new List<PluginTheme>();
        
        private IPlugin currentPlugin;
        public IPlugin CurrentPlugin
        {
            get
            {
                return currentPlugin;
            }
            set
            {
                if (value != null)
                {
                    // We're handled a new plugin
                    if (currentPlugin == null)
                    {
                        // Plugin was hidden, now show
                        currentPlugin = value;
                        currentPlugin.Show();
                        // connect ShownMessage to new plugin;
                        ShownMessages = currentPlugin.VisibleMessages;
                        Status = String.Format("Presentation '{0}' shown", CurrentPlugin.Name);
                        if (PluginShown != null) PluginShown(this, new EventArgs());
                    }
                    else
                    {
                        // Swapping plugins
                        currentPlugin.Hide();
                        currentPlugin = value;
                        currentPlugin.Show();
                        // connect ShownMessage to new plugin;
                        ShownMessages = currentPlugin.VisibleMessages;
                        if (PluginChanged != null) PluginChanged(this, new EventArgs());
                    }
                }
                else
                {
                    // We have to hide the plugin
                    if (currentPlugin != null)
                    {
                        // Plugin was shown, now hide
                        // stop message switching
                        MessageSwitching = false;
                        Status = String.Format("Presentation '{0}' hidden", CurrentPlugin.Name);
                        // Hide
                        currentPlugin.Hide();
                        currentPlugin = value;
                        // connect ShownMessage to nothing;
                        ShownMessages = null;
                        if (PluginHidden != null) PluginHidden(this, new EventArgs());
                    }
                    // else we didn't have a plugin and we won't get any either
                }
                this.RaisePropertyChanged("CurrentPlugin");
            }
        }

        public OutputScreenList ScreenList;

        private OutputScreen currentScreen;
        public OutputScreen CurrentScreen
        {
            get { return currentScreen; }
            set
            {
                currentScreen = value;
                foreach (IPlugin plugin in Plugins)
                {
                    plugin.SetViewport(currentScreen.Left, currentScreen.Top, currentScreen.Width, currentScreen.Height);
                }
                this.RaisePropertyChanged("CurrentScreen");
            }
        }

        private PluginTheme currentTheme;
        public PluginTheme CurrentTheme
        {
            get { return currentTheme; }
            set
            {
                currentTheme = value;
                foreach (IPlugin plugin in Plugins)
                {
                    plugin.ApplyThemeStyles(currentTheme);
                }
                this.RaisePropertyChanged("CurrentTheme");
            }
        }

        private string phoneNumber;
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set
            {
                phoneNumber = value;
                if (!String.IsNullOrEmpty(phoneNumber))
                {
                    foreach (IPlugin plugin in Plugins)
                    {
                        plugin.SetPhoneNumber(phoneNumber);
                    }
                }
            }
        }
        private bool showPublicOffline = true;
        public bool ShowPublicOffline
        {
            get { return showPublicOffline; }
            set
            {
                showPublicOffline = value;
                this.RaisePropertyChanged("ShowPublicOffline");
            }
        }

        private bool loopArchive = true;
        public bool LoopArchive
        {
            get { return loopArchive; }
            set
            {
                loopArchive = value;
                this.RaisePropertyChanged("LoopArchive");
            }
        }

        private bool showCostsAndConditions;
        public bool ShowCostsAndConditions
        {
            get { return showCostsAndConditions; }
            set
            {
                showCostsAndConditions = value;
                foreach (IPlugin plugin in Plugins)
                {
                    plugin.SetCostsVisibility(showCostsAndConditions);
                }
                this.RaisePropertyChanged("ShowCostsAndConditions");
            }
        }
        private bool isOffline = true;
        public bool IsOffline
        {
            get { return isOffline; }
            set
            {
                isOffline = value;
                if (isOffline && ShowPublicOffline)
                {
                    foreach (IPlugin plugin in Plugins)
                    {
                        plugin.SystemOffline();
                    }
                }
                this.RaisePropertyChanged("IsOffline");
            }
        }

        private int switchingInterval = 15000;
        public int SwitchingInterval
        {
            get { return switchingInterval; }
            set
            {
                switchingInterval = value;
                this.RaisePropertyChanged("SwitchingInterval");
            }
        }
        public double SwitchingIntervalSeconds
        {
            get { return SwitchingInterval / 1000; }
            set
            {
                SwitchingInterval = System.Convert.ToInt32(value * 1000);
            }
        }
        private bool messageSwitching;
        public bool MessageSwitching
        {
            get
            {
                return messageSwitching;
            }
            set
            {
                messageSwitching = value;
                if (value)
                    StartQueueTimer();
                else
                    StopQueueTimer();
                approvalTimer.Enabled = (autoApproval && messageSwitching);
                this.RaisePropertyChanged("MessageSwitching");
            }
        }

        private int approvalInterval = 30000;
        public int ApprovalInterval
        {
            get { return approvalInterval; }
            set
            {
                approvalInterval = value;
                this.RaisePropertyChanged("ApprovalInterval");
            }
        }
        public double ApprovalIntervalSeconds
        {
            get { return ApprovalInterval / 1000; }
            set
            {
                ApprovalInterval = System.Convert.ToInt32(value * 1000);
            }
        }
        private bool autoApproval;
        public bool AutoApproval
        {
            get
            {
                return autoApproval;
            }
            set
            {
                autoApproval = value;
                approvalTimer.Enabled = (autoApproval && messageSwitching);
                Status = String.Format("Auto-approval {0}", value ? "enabled" : "disabled");
                this.RaisePropertyChanged("AutoApproval");
            }
        }

        private int alwaysSendInformationInterval = 5;
        public int AlwaysSendInformationInterval
        {
            get { return alwaysSendInformationInterval; }
            set
            {
                alwaysSendInformationInterval = value;
                this.RaisePropertyChanged("AlwaysSendInformationInterval");
            }
        }
        private bool alwaysSendInformation;
        public bool AlwaysSendInformation
        {
            get
            {
                return alwaysSendInformation;
            }
            set
            {
                alwaysSendInformation = value;
                Status = String.Format("Send information even on new messages {0}", value ? "enabled" : "disabled");
                this.RaisePropertyChanged("AlwaysSendInformation");
            }
        }


        public void NewMessage(Message message)
        {
            NewMessages.AddUnique(message);
        }

        public void ApproveMessage(Message message)
        {
            if (NewMessages.Remove(message))
                ApprovedMessages.Add(message);
        }
        public void ApproveMessages(MessageList messages)
        {
            foreach (Message message in messages)
                ApproveMessage(message);
        }

        public void RejectMessage(Message message)
        {
            if (NewMessages.Remove(message))
                RejectedMessages.Add(message);
        }

        void queueTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Status = "Determining next message";
            if (ApprovedMessages.Count > 0)
            {
                if (!AlwaysSendInformation || (lastSendInformation < AlwaysSendInformationInterval))
                {
                    queueTimer.Interval = switchingInterval;
                    _sendInfoShown = false;
                    Dispatcher.BeginInvoke(new Action(showNextMessage), null);
                }
                else
                {
                    queueTimer.Interval = 4000;
                    _sendInfoShown = true;
                    Dispatcher.BeginInvoke(new Action(showSendInformation), null);
                }
            }
            else
            {
                if ((!_sendInfoShown) || (ArchivedMessages.Count == 0))
                {
                    queueTimer.Interval = 4000;
                    _sendInfoShown = true;
                    Dispatcher.BeginInvoke(new Action(showSendInformation), null);
                }
                else
                {
                    queueTimer.Interval = switchingInterval;
                    _sendInfoShown = false;
                    Dispatcher.BeginInvoke(new Action(showArchivedMessage), null);
                }
            }
        }

        private void showNextMessage()
        {
            if (ApprovedMessages.Count > 0)
            {
                lastSendInformation++;
                Message nextMessage = ApprovedMessages[0];
                if (ApprovedMessages.Remove(nextMessage))
                    ArchivedMessages.Add(nextMessage);
                foreach (IPlugin plugin in Plugins)
                {
                    plugin.NewMessage(nextMessage);
                }
                Status = String.Format("Display new message ({1}s): {0}", nextMessage.MessageTextSingleLine, queueTimer.Interval / 1000);
                if (PresentationChanged != null) PresentationChanged(this, new EventArgs());
            }
        }

        private void showArchivedMessage()
        {
            if (ArchivedMessages.Count > 0)
            {
                int messageToShowIndex;
                if (LoopArchive)
                {
                    currentArchiveMessage = (currentArchiveMessage % ArchivedMessages.Count) + 1;
                    messageToShowIndex = currentArchiveMessage - 1;
                }
                else
                {
                    messageToShowIndex = ArchivedMessages.Count - 1;
                }
                Message curMessage = ArchivedMessages[messageToShowIndex];
                foreach (IPlugin plugin in Plugins)
                {
                    plugin.OldMessage(curMessage);
                }
                Status = String.Format("Older message on single message displays ({1}s): {0}", curMessage.MessageTextSingleLine, queueTimer.Interval / 1000);
                if (PresentationChanged != null) PresentationChanged(this, new EventArgs());
            }
        }

        private void showSendInformation()
        {
            lastSendInformation = 0;
            if (!String.IsNullOrEmpty(PhoneNumber))
            {
                foreach (IPlugin plugin in Plugins)
                {
                    plugin.NoMessage();
                }
                Status = String.Format("Showing send information on single message displays ({0}s)", queueTimer.Interval / 1000);
                if (PresentationChanged != null) PresentationChanged(this, new EventArgs());
            }
            else
            {
                if (ShowPublicOffline)
                {
                    foreach (IPlugin plugin in Plugins)
                    {
                        plugin.SystemOffline();
                    }
                    Status = String.Format("Showing 'System Offline' message...", queueTimer.Interval / 1000);
                    if (PresentationChanged != null) PresentationChanged(this, new EventArgs());
                }
                else
                    Status = String.Format("We're offline, but that's not public....", queueTimer.Interval / 1000);
            }
        }

        private void StartQueueTimer()
        {
            queueTimer.Enabled = true;
            Status = "Message switching started";
            if (MessageSwitcherStarted != null) MessageSwitcherStarted(this, new EventArgs());
        }

        private void StopQueueTimer()
        {
            queueTimer.Enabled = false;
            Status = "Message switching stopped";
            if (MessageSwitcherStopped != null) MessageSwitcherStopped(this, new EventArgs());
        }

        private void approvalTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            approvalTimer.Enabled = false;
            if (autoApproval) // Are will still auto-approving?
            {
                DateTime threshold = DateTime.Now.AddSeconds(-1 * ApprovalIntervalSeconds);
                MessageList AutoApproveList = new MessageList();
                foreach (Message message in NewMessages)
                {
                    if (message.DownloadDate <= threshold)
                    {
                        AutoApproveList.Add(message);
                    }
                }
                Dispatcher.BeginInvoke(new Action<MessageList>(ApproveMessages), AutoApproveList);
            }
            approvalTimer.Enabled = autoApproval;
        }

        private void saveMessages(MessageList messageList, string fileName)
        {
            XmlTextWriter writer = new XmlTextWriter(fileName, null);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteComment(String.Format("Creation date: {0}", DateTime.Now.ToUniversalTime().ToString("u")));
            writer.WriteStartElement("messagelist");
            foreach (Message message in messageList)
            {
                writer.WriteStartElement("message");
                writer.WriteStartElement("datetime");
                writer.WriteString(message.Date.ToString("s"));
                writer.WriteEndElement();
                writer.WriteStartElement("downloaddate");
                writer.WriteString(message.DownloadDate.ToString("s"));
                writer.WriteEndElement();
                writer.WriteStartElement("phonenumber");
                writer.WriteString(message.PhoneNumber);
                writer.WriteEndElement();
                writer.WriteStartElement("text");
                writer.WriteCData(message.MessageText);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }
        #region FileHandlers

        private void loadMessages(MessageList messageList, string fileName)
        {
            XmlTextReader reader = new XmlTextReader(fileName);
            System.IO.FileInfo info = new System.IO.FileInfo(fileName);
            if (info.Exists)
            {
                Message message;
                string text = "";
                string phonenumber = "";
                DateTime datetime = DateTime.Now;
                DateTime downloadDate = DateTime.Now;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case "message":
                                    text = "";
                                    phonenumber = "";
                                    datetime = DateTime.Now;
                                    break;
                                case "text":
                                    text = reader.ReadElementContentAsString();
                                    break;
                                case "phonenumber":
                                    phonenumber = reader.ReadElementContentAsString();
                                    break;
                                case "datetime":
                                    datetime = reader.ReadElementContentAsDateTime();
                                    break;
                                case "downloaddate":
                                    downloadDate = reader.ReadElementContentAsDateTime();
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == "message")
                            {
                                message = new Message(text, phonenumber, datetime, downloadDate);
                                messageList.AddUnique(message);
                            }
                            break;
                    }
                }
            }
        }
        #endregion
    }
}
