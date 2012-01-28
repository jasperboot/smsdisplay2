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

#define DEBUG
#define CONSOLEDEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GSM;
using GSM.AT;
using System.Windows.Threading;
using System.Threading;
using SMSdisplay.Plugins;
using System.Reflection;
using System.Collections.ObjectModel;
using GSM.Forms;
using AppBuildInfo = SMSdisplay.GUI.BuildInfo;
using SMSdisplay.GUI.BuildInfo;

namespace SMSdisplay.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Communicator communicator;
        private Presenter presenter;
        private PhoneStatus phoneStatus;

        private bool _wasScreensaverActive;
        private delegate void UpdatePortListDelegate(SerialPortList portList);

        // Enable for debug form
        //MonitorForm monfrm;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Window_Loaded);
            Closing += new System.ComponentModel.CancelEventHandler(Window_Closing);

            presenter = new Presenter(Dispatcher);

            applicationStatus.DataContext = presenter;
            presenter.Status = "Starting... Loading main window";

            phoneStatus = new PhoneStatus();
            phoneControl.DataContext = phoneStatus;
            connectionStatus.DataContext = phoneStatus;

            communicator = new Communicator();
            (Application.Current as App).CatchSerialPortErrors();

            messageControl.DataContext = presenter;
            optionControl.DataContext = presenter;

            lvNewMessages.DataContext = presenter.NewMessages;
            lvApprovedMessages.DataContext = presenter.ApprovedMessages;
            lvRejectedMessages.DataContext = presenter.RejectedMessages;
            lvArchivedMessages.DataContext = presenter.ArchivedMessages;
            lvShownMessages.DataContext = presenter.ShownMessages;

        }

        private void debugData(object sender, NewDebugDataArgs args)
        {
            Console.WriteLine(args.Message);
        }

        private void disableScreensaver()
        {
            _wasScreensaverActive = Util.ScreenSaver.GetScreenSaverActive();
            Util.ScreenSaver.SetScreenSaverActive(0);
        }

        private void restoreScreensaverState()
        {
            if (_wasScreensaverActive) Util.ScreenSaver.SetScreenSaverActive(1);
        }

        private void closeConnection()
        {
            if ((communicator != null) && (communicator.Connection)) communicator.Disconnect();
        }

        private void closeConnectionOnExit()
        {
            closeConnection();
            while ((communicator != null) && (communicator.Connection))
            {
                Thread.Sleep(10);
            }
        }

        private void updatePortList(SerialPortList portList)
        {
            serialPorts.DataContext = portList;
            int itemCount = portList.Count;
            SerialPortInfo preferred = portList.PhonePreferred;
            if (preferred != null)
            {
                serialPorts.SelectedItem = preferred;
            }
            else
            {
                if ((serialPorts.SelectedIndex == -1) && (itemCount > 0)) serialPorts.SelectedIndex = itemCount - 1;
            }
        }

        private void updatePluginList()
        {
            plugins.DataContext = presenter.Plugins;
            if ((plugins.SelectedIndex == -1) && (presenter.Plugins.Count > 0)) plugins.SelectedIndex = 0;

            themes.DataContext = presenter.Themes;
            if ((themes.SelectedIndex == -1) && (presenter.Themes.Count > 0)) themes.SelectedIndex = 0;
        }

        private void updateScreenList()
        {
            screens.DataContext = presenter.ScreenList;
            OutputScreen myScreen = getMyScreen(presenter.ScreenList);
            if (myScreen != null)
            {
                screens.SelectedItem = presenter.ScreenList.GetOtherThan(myScreen, true);
            }
            else
            {
                int itemCount = presenter.ScreenList.Count;
                if (itemCount > 0) screens.SelectedIndex = itemCount - 1;
            }
        }

        private OutputScreen getMyScreen(OutputScreenList screenList)
        {
            int MyCenterX = Convert.ToInt32(Left + (Width / 2));
            int MyCenterY = Convert.ToInt32(Top + (Height / 2));
            foreach (OutputScreen screen in screenList)
            {
                if ((MyCenterX >= screen.Left) && (MyCenterX <= screen.Right))
                {
                    if ((MyCenterY >= screen.Top) && (MyCenterX <= screen.Bottom))
                    {
                        // Found the screen we're on
                        return screen;
                    }
                }
            }
            return null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Enable for debug form
            //monfrm = new MonitorForm();
            //communicator.NewRxDebugData += monfrm.newRxData;
            //communicator.NewTxDebugData += monfrm.newTxData;
            presenter.Status = "Starting... Registering events";
            #if CONSOLEDEBUG
            communicator.NewRxDebugData += new EventHandler<NewDebugDataArgs>(debugData);
            communicator.NewTxDebugData += new EventHandler<NewDebugDataArgs>(debugData);
            #endif
            communicator.NewTxDebugData += new EventHandler<NewDebugDataArgs>(phoneActionToStatus);
            communicator.Opened += new EventHandler(phoneConnected);
            communicator.Closed += new EventHandler(phoneDisconnected);
            serialPorts.SelectionChanged += new SelectionChangedEventHandler(serialPorts_SelectionChanged);
            btnConnect.Click += new RoutedEventHandler(connect);
            btnDisconnect.Click += new RoutedEventHandler(disconnect);
            /*
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object threadContext)
            {
                SerialPortList portList = Communicator.GetPorts();
                Dispatcher.BeginInvoke(new UpdatePortListDelegate(updatePortList),portList);
            }));
            */
            communicator.IdentChange += new EventHandler<IdentChangeEventArgs>(communicator_IdentChange);
            communicator.ModelChange += new EventHandler<ModelChangeEventArgs>(communicator_ModelChange);
            communicator.NumberChange += new EventHandler<NumberChangeEventArgs>(communicator_NumberChange);
            communicator.BatteryChargeChange += new EventHandler<BatteryChargeChangeEventArgs>(communicator_BatteryChargeChange);
            communicator.BatteryChargingChange += new EventHandler<BatteryChargingChangeEventArgs>(communicator_BatteryChargingChange);
            communicator.NetworkOperatorChange += new EventHandler<NetworkOperatorChangeEventArgs>(communicator_NetworkOperatorChange);
            communicator.SignalStrengthChange += new EventHandler<SignalStrengthChangeEventArgs>(communicator_SignalStrengthChange);
            communicator.NetworkConnectionChange += new EventHandler<NetworkConnectionChangeEventArgs>(communicator_NetworkConnectionChange);
            communicator.SilentModeChange += new EventHandler<SilentModeChangeEventArgs>(communicator_SilentModeChange);
            communicator.NewMessage += new EventHandler<NewMessageEventArgs>(communicator_NewMessage);
            communicator.StoreCapacityChange += new EventHandler<StoreCapacityChangeEventArgs>(communicator_StoreCapacityChange);
            communicator.StoreCountChange += new EventHandler<StoreCountChangeEventArgs>(communicator_StoreCountChange);

            presenter.MessageSwitcherStarted += new EventHandler(presenter_MessageSwitcherStarted);
            presenter.MessageSwitcherStopped += new EventHandler(presenter_MessageSwitcherStopped);
            presenter.PluginShown += new EventHandler(presenter_PluginShown);
            presenter.PluginChanged += new EventHandler(presenter_PluginChanged);
            presenter.PluginHidden += new EventHandler(presenter_PluginHidden);
            presenter.PresentationChanged += new EventHandler(presenter_PresentationChanged);

            presenter.Status = "Initializing presenter...";
            presenter.Initialize();
            updatePluginList();
            updateScreenList();

            presenter.Status = "Starting... Enumerating serial ports";
            SerialPortList portList = Communicator.GetPorts();
            updatePortList(portList);

            presenter.Status = "Starting... Disabling screensaver";
            disableScreensaver();

            presenter.Status = "Showing (debug) monitor form";
            // Enable for debug form
            //monfrm.Show();

            presenter.Status = "Processing release information";
            ReleaseCopyright.Text = ApplicationCopyright;
            ReleaseText.Text = ApplicationRelease;
            ReleaseText.Visibility = Visibility.Visible;
            ReleaseBuild.Text = ApplicationBuild;
            ReleaseBuild.Visibility = Visibility.Visible;

            presenter.Status = "Cleaning up...";
            btnConnect.IsEnabled = !communicator.Connection;
            presenter.Status = "Running";
        }

        public string ApplicationCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                    return null;
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright.Replace("Copyright ", "");
            }
        }

        public string ApplicationRelease
        {
            get
            {
                return String.Format("Release {0} - {1}", BuildVersion.Short, BuildVersion.ReleaseType);
            }
        }

        public string ApplicationBuild
        {
            get
            {
                return String.Format("{0} ({1})", BuildVersion.BuildType, BuildVersion.Build);
            }
        }

        void communicator_IdentChange(object sender, IdentChangeEventArgs e)
        {
            phoneStatus.Ident = e.Ident;
        }

        void communicator_ModelChange(object sender, ModelChangeEventArgs e)
        {
            phoneStatus.Model = e.Model;
        }

        void communicator_NumberChange(object sender, NumberChangeEventArgs e)
        {
            string number = e.Number;
            if ((!String.IsNullOrEmpty(number)) && (number.StartsWith("06"))) number = "06-" + number.Substring(2);
            phoneStatus.Number = number;
            Dispatcher.BeginInvoke(new EventHandler(delegate(object sender2, EventArgs e2)
            {
                presenter.PhoneNumber = number;
            }), this, new EventArgs());
        }

        void communicator_SilentModeChange(object sender, SilentModeChangeEventArgs e)
        {
            switch (e.SilentMode)
            {
                case SilentModeType.On:
                    Dispatcher.BeginInvoke(new EventHandler(delegate(object sender2, EventArgs e2)
                    {
                        silentModeImage.Source = new BitmapImage(new Uri("Resources/StatusImages/silentmode_on.png", UriKind.Relative));
                    }), this, new EventArgs());
                    break;
                case SilentModeType.Off:
                    Dispatcher.BeginInvoke(new EventHandler(delegate(object sender2, EventArgs e2)
                    {
                        silentModeImage.Source = new BitmapImage(new Uri("Resources/StatusImages/silentmode_off.png", UriKind.Relative));
                    }), this, new EventArgs());
                    break;
                default:
                    Dispatcher.BeginInvoke(new EventHandler(delegate(object sender2, EventArgs e2)
                    {
                        silentModeImage.Source = new BitmapImage(new Uri("Resources/StatusImages/silentmode_unknown.png", UriKind.Relative));
                    }), this, new EventArgs());
                    break;
            }
        }

        void communicator_NetworkConnectionChange(object sender, NetworkConnectionChangeEventArgs e)
        {
            if (e.NetworkConnection == ConnectionType.G3)
            {
                phoneStatus.ConnectionType = "3G/UMTS";
                Dispatcher.BeginInvoke(new EventHandler(delegate(object sender2, EventArgs e2)
                {
                    signalType.Visibility = Visibility.Visible;
                }), this, new EventArgs());
            } else {
                phoneStatus.ConnectionType = "GSM";
                Dispatcher.BeginInvoke(new EventHandler(delegate(object sender2, EventArgs e2)
                {
                    signalType.Visibility = Visibility.Hidden;
                }), this, new EventArgs());
            }
        }

        void communicator_BatteryChargingChange(object sender, BatteryChargingChangeEventArgs e)
        {
            bool charging = e.BatteryCharging;
            if (charging)
            {
                Dispatcher.BeginInvoke(new EventHandler(delegate(object sender2, EventArgs e2)
                { 
                    chargeImage.Source = new BitmapImage(new Uri("Resources/StatusImages/battery_charging.png", UriKind.Relative));
                }), this, new EventArgs());
            }
            else
            {
                if (communicator.Connection)
                {
                    Dispatcher.BeginInvoke(new EventHandler(delegate(object sender2, EventArgs e2)
                    {
                        chargeImage.Source = new BitmapImage(new Uri("Resources/StatusImages/battery.png", UriKind.Relative));
                    }), this, new EventArgs());
                }
                else
                {
                    Dispatcher.BeginInvoke(new EventHandler(delegate(object sender2, EventArgs e2)
                    {
                        chargeImage.Source = new BitmapImage(new Uri("Resources/StatusImages/battery_unknown.png", UriKind.Relative));
                    }), this, new EventArgs());
                }
            }
        }

        void communicator_SignalStrengthChange(object sender, SignalStrengthChangeEventArgs e)
        {
            phoneStatus.Signal = e.SignalStrength;
        }

        void communicator_NetworkOperatorChange(object sender, NetworkOperatorChangeEventArgs e)
        {
            string network = e.NetworkOperator;
            phoneStatus.Network = network;
            if ((network != "No network") && (network != ""))
            {
                Dispatcher.BeginInvoke(new EventHandler(delegate(object sender2, EventArgs e2)
                {
                    networkImage.Source = new BitmapImage(new Uri("Resources/StatusImages/network.png", UriKind.Relative));
                }), this, new EventArgs());
            }
            else
            {
                Dispatcher.BeginInvoke(new EventHandler(delegate(object sender2, EventArgs e2)
                {
                    networkImage.Source = new BitmapImage(new Uri("Resources/StatusImages/network_none.png", UriKind.Relative));
                }), this, new EventArgs());
            }
        }

        void communicator_BatteryChargeChange(object sender, BatteryChargeChangeEventArgs e)
        {
            phoneStatus.Battery = e.BatteryCharge;
        }

        void communicator_NewMessage(object sender, NewMessageEventArgs e)
        {
            Message newMessage = new Message(e.Message, e.PhoneNumber, e.TimeStamp);
            Dispatcher.BeginInvoke(new EventHandler(delegate(object sender2, EventArgs e2)
            {
                presenter.NewMessage(newMessage);
            }), this, new EventArgs());
        }

        void communicator_StoreCapacityChange(object sender, StoreCapacityChangeEventArgs e)
        {
            int capacity = e.StoreCapacity;
            if (capacity > -1)
            {
                phoneStatus.InboxCapacity = capacity;
                Dispatcher.BeginInvoke(new EventHandler(delegate(object sender2, EventArgs e2)
                {
                    inboxImage.Source = new BitmapImage(new Uri("Resources/StatusImages/mailbox.png", UriKind.Relative));
                }), this, new EventArgs());
            }
            else
            {
                phoneStatus.InboxCapacity = 100;
                Dispatcher.BeginInvoke(new EventHandler(delegate(object sender2, EventArgs e2)
                {
                    inboxImage.Source = new BitmapImage(new Uri("Resources/StatusImages/mailbox_unknown.png", UriKind.Relative));
                }), this, new EventArgs());
            }
        }

        void communicator_StoreCountChange(object sender, StoreCountChangeEventArgs e)
        {
            int count = e.StoreCount;
            if (count > -1)
            {
                phoneStatus.InboxCount = count;
            }
            else
            {
                phoneStatus.InboxCount = 0;
            }
        }

        void presenter_MessageSwitcherStarted(object sender, EventArgs e)
        {
            btnStartQueue.IsEnabled = false;
            btnStopQueue.IsEnabled = true;
            cbPluginLock.IsChecked = true;
            updatePluginLockedState();
        }

        void presenter_MessageSwitcherStopped(object sender, EventArgs e)
        {
            btnStopQueue.IsEnabled = false;
            btnStartQueue.IsEnabled = true;
            cbPluginLock.IsChecked = false;
            updatePluginLockedState();
        }

        void connect(object sender, RoutedEventArgs e)
        {
            btnConnect.IsEnabled = false;
            serialPorts.IsEnabled = false;
            phoneStatus.StatusMessage = "Connecting...";
            try
            { communicator.Connect(); }
            catch 
            {
                btnConnect.IsEnabled = true;
                serialPorts.IsEnabled = true;
                connectionStatus.Background = Brushes.IndianRed;
                phoneStatus.StatusMessage = String.Format("Failed to connect to {0}", communicator.PortName);
            }
        }

        void disconnect(object sender, RoutedEventArgs e)
        {
            btnDisconnect.IsEnabled = false;
            communicator.Disconnect();
        }

        void serialPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (serialPorts.SelectedIndex > -1) communicator.PortName = (serialPorts.SelectedItem as SerialPortInfo).Name;
            phoneStatus.StatusMessage = "Disconnected";
            connectionStatus.Background = Brushes.Transparent;
        }

        private void plugins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (plugins.SelectedIndex > -1)
            {
                btnToggleDisplay.IsEnabled = true; // after startup, enable button as soon as list gets populated
                if (presenter.CurrentPlugin != null)
                {
                    presenter.CurrentPlugin = (plugins.SelectedItem as IPlugin);
                }
            }
        }

        private void themes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (themes.SelectedIndex > -1)
            {
                presenter.CurrentTheme = (themes.SelectedItem as PluginTheme);
            }
        }

        void phoneActionToStatus(object sender, NewDebugDataArgs e)
        {
            phoneStatus.StatusMessage = e.Message;
        }

        void phoneConnected(object sender, EventArgs e)
        {
            phoneStatus.StatusMessage = String.Format("Connected to {0}", communicator.PortName);
            connectionState.Source = new BitmapImage(new Uri("Resources/phone_connected.png", UriKind.Relative));
            btnDisconnect.IsEnabled = true;
            presenter.IsOffline = false;
        }

        void phoneDisconnected(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new EventHandler(phoneDisconnected_UI), this, e);
        }

        void phoneDisconnected_UI(object sender, EventArgs e)
        {
            btnDisconnect.IsEnabled = false;
            phoneStatus.StatusMessage = "Disconnected";
            connectionState.Source = new BitmapImage(new Uri("Resources/phone_unconnected.png", UriKind.Relative));
            
            communicator.DeleteReadMessages = false;
            cbRemoveRead.IsChecked = false;
            
            presenter.IsOffline = true;

            btnConnect.IsEnabled = true;
            serialPorts.IsEnabled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            presenter.Close();
            restoreScreensaverState();
            closeConnectionOnExit();
        }

        private void btnToggleDisplay_Click(object sender, RoutedEventArgs e)
        {
            if (presenter.CurrentPlugin == null)
            {
                if ((plugins.SelectedIndex > -1))
                {
                    presenter.CurrentPlugin = (plugins.SelectedItem as IPlugin);
                }
            } 
            else
            {
                presenter.CurrentPlugin = null;
            }
        }

        void presenter_PluginShown(object sender, EventArgs e)
        {
            btnToggleDisplay.Content = "_Hide";
            // set lock on Screen config
            cbScreenLock.IsChecked = true;
            updateScreenLockedState();
            // update screen visibility state
            screenState.Source = new BitmapImage(new Uri("Resources/display_on.png", UriKind.Relative));
            // enable switcher control
            SwitcherControl.IsEnabled = true;
            lvShownMessages.DataContext = presenter.ShownMessages;
        }

        void presenter_PluginChanged(object sender, EventArgs e)
        {
            //
            lvShownMessages.DataContext = presenter.ShownMessages;
        }

        void presenter_PluginHidden(object sender, EventArgs e)
        {
            btnToggleDisplay.Content = "S_how";
            // disable switcher control
            SwitcherControl.IsEnabled = false;
            // update screen visibility state
            screenState.Source = new BitmapImage(new Uri("Resources/display_off.png", UriKind.Relative));
            cbScreenLock.IsChecked = false;
            // remove lock from ScreenConfig
            screens.IsEnabled = true;
            updateScreenLockedState();
            lvShownMessages.DataContext = presenter.ShownMessages;
        }

        void presenter_PresentationChanged(object sender, EventArgs e)
        {
            if (lvShownMessages.Items.Count > 0)
                (lvShownMessages.ItemContainerGenerator.ContainerFromIndex(lvShownMessages.Items.Count - 1) as ContentPresenter).BringIntoView();
        }

        private void updateScreenLockedState()
        {
            screens.IsEnabled = !(cbScreenLock.IsChecked ?? false);
        }

        private void cbScreenLock_Clicked(object sender, RoutedEventArgs e)
        {
            updateScreenLockedState();
        }
        
        private void updatePluginLockedState()
        {
            bool isEnabled = !(cbPluginLock.IsChecked ?? false);
            plugins.IsEnabled = isEnabled;
            themes.IsEnabled = isEnabled;
            btnToggleDisplay.IsEnabled = isEnabled;
        }
        private void cbPluginLock_Clicked(object sender, RoutedEventArgs e)
        {
            updatePluginLockedState();
        }

        private void screens_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updatePluginViewports();
        }

        private void updatePluginViewports()
        {
            presenter.CurrentScreen = (screens.SelectedItem as OutputScreen);
        }

        private void addManualMessage()
        {
            if (manualMessage.Text != "")
            {
                Message newMessage = new Message(manualMessage.Text.Replace("\\n","\n"), "[manual]");
                if (cbManualApproved.IsChecked ?? false)
                {
                    presenter.ApprovedMessages.Insert(0, newMessage);
                }
                else
                {
                    presenter.NewMessages.Add(newMessage);
                    lvNewMessages.SelectedItem = newMessage;
                }
            }
            manualMessage.SelectAll();
        }

        private void btnInsertManual_Click(object sender, RoutedEventArgs e)
        {
            addManualMessage();
        }

        private void manualMessage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                manualMessage.AppendText("\\n");
                manualMessage.CaretIndex += manualMessage.Text.Length;
            }
            int mcc = manualMessage.Text.Length;
            charCount.Text = String.Format("({0})", mcc);
            btnInsertManual.IsEnabled = (mcc > 0);
            if (e.Key == Key.Enter) addManualMessage();
        }

        private void lvNewMessages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvNewMessages.SelectedIndex > -1)
            {
                btnApprove.IsEnabled = true;
                btnReject.IsEnabled = true;
            }
            else
            {
                btnApprove.IsEnabled = false;
                btnReject.IsEnabled = false;
            }
        }

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            if (lvNewMessages.SelectedIndex > -1)
            {
                while (lvNewMessages.SelectedItems.Count > 0)
                {
                    Message message = (lvNewMessages.SelectedItems[0] as Message);
                    presenter.ApproveMessage(message);
                }
                if (lvNewMessages.Items.Count > 0) lvNewMessages.SelectedIndex = 0;
            }
        }

        private void btnReject_Click(object sender, RoutedEventArgs e)
        {
            if (lvNewMessages.SelectedIndex > -1)
            {
                while (lvNewMessages.SelectedItems.Count > 0)
                {
                    Message message = (lvNewMessages.SelectedItems[0] as Message);
                    presenter.RejectMessage(message);
                    if (lvNewMessages.SelectedItems.Count == 0) lvRejectedMessages.ScrollIntoView(message);
                }
                if (lvNewMessages.Items.Count > 0) lvNewMessages.SelectedIndex = 0;
            }
        }

        private void lvArchivedMessages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (lvArchivedMessages.SelectedIndex > -1)
            //{
            //    btnReinsert.IsEnabled = true;
            //}
            //else
            //{
            //    btnReinsert.IsEnabled = false;
            //}
        }

        //private void btnReinsert_Click(object sender, RoutedEventArgs e)
        //{
        //    if (lvArchivedMessages.SelectedIndex > -1)
        //    {
        //        foreach (Message message in lvArchivedMessages.SelectedItems)
        //        {
        //            newMessages.Add(message);
        //        }
        //    }
        //}

        private void btnStartQueue_Click(object sender, RoutedEventArgs e)
        {
            presenter.MessageSwitching = true;
        }

        private void btnStopQueue_Click(object sender, RoutedEventArgs e)
        {
            presenter.MessageSwitching = false;
        }

        private void cbRemoveRead_Click(object sender, RoutedEventArgs e)
        {
            communicator.DeleteReadMessages = (cbRemoveRead.IsChecked ?? false);
        }

        private void messageList_KeyUp(object sender, KeyEventArgs e)
        {
            ListView listView = (sender as ListView);
            if (listView != null)
            {
                int selectedIndex = listView.SelectedIndex;
                MessageList messageList = (listView.ItemsSource as MessageList);
                if (messageList != null)
                {
                    Message selectedMessage = (listView.SelectedItem as Message);
                    if (selectedMessage != null)
                    {
                        if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl))
                        {
                            switch (e.Key)
                            {
                                case Key.Up:
                                    if (selectedIndex > 0)
                                        messageList.Move(selectedIndex, selectedIndex - 1);
                                    break;
                                case Key.Down:
                                    if (selectedIndex < messageList.Count - 1)
                                        messageList.Move(selectedIndex, selectedIndex + 1);
                                    break;
                            }
                            listView.ScrollIntoView(selectedMessage);
                            e.Handled = true;
                        }
                        else
                        {
                            switch (e.Key)
                            {
                                case Key.Delete:
                                    if (messageList != presenter.RejectedMessages)
                                    {
                                        if (messageList.Remove(selectedMessage))
                                        {
                                            presenter.RejectedMessages.Add(selectedMessage);
                                            if (messageList.Count > 0)
                                                listView.SelectedIndex = (selectedIndex < messageList.Count) ? selectedIndex : selectedIndex - 1;
                                        }
                                    }
                                    e.Handled = true;
                                    break;
                                case Key.Insert:
                                    if (messageList != presenter.ApprovedMessages)
                                    {
                                        if (messageList.Remove(selectedMessage))
                                        {
                                            presenter.ApprovedMessages.Add(selectedMessage);
                                            if (messageList.Count > 0)
                                                listView.SelectedIndex = (selectedIndex < messageList.Count) ? selectedIndex : selectedIndex - 1;
                                        }
                                    }
                                    e.Handled = true;
                                    break;
                            }
                        }
                    }
                }
            }
        }

    }
}
