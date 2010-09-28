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
using System.Windows.Forms;
using System.Threading;
using GSM;
using GSM.Forms;

namespace GSM.Debugger
{
    static class Program
    {
        private static Communicator communicator;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            communicator = new Communicator();
            communicator.PortName = "COM4";
            Communicator.GetPorts();
            
            MonitorForm monitorForm = new MonitorForm();
            communicator.NewRxDebugData += new EventHandler<NewDebugDataArgs>(monitorForm.newRxData);
            communicator.NewTxDebugData += new EventHandler<NewDebugDataArgs>(monitorForm.newTxData);
            Application.ApplicationExit += new EventHandler(closeConnections);
            communicator.Connect();
            Application.Run(monitorForm);
        }

        private static void closeConnections(object sender, EventArgs e)
        {
            if (communicator.Connection) communicator.Disconnect();
            while (communicator.Connection)
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
        }

    }
}
