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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Collections;

namespace GSM.Forms
{
    public partial class MonitorForm : Form
    {
        private Communicator GSMcon = new Communicator();

        public MonitorForm()
        {
            InitializeComponent();
        }

        private void WriteRxDbg(string data)
        {
            atRx.AppendText(String.Format("{0}\r\n",data));
        }

        private void WriteTxDbg(string data)
        {
            atTx.AppendText(String.Format("{0}: {1}\r\n",System.DateTime.Now.ToString("s").Replace('T',' '),data));
        }


        private void Form1_Load(object sender, EventArgs e)
        {
        }

        public void newRxData(object sender, NewDebugDataArgs data)
        {

            if (!atRx.IsDisposed)
            {
                if (this.IsHandleCreated)
                {
                    atRx.BeginInvoke(new EventHandler(delegate
                    {
                        WriteRxDbg(data.Message);
                    }));
                }
            }
        }

        public void newTxData(object sender, NewDebugDataArgs data)
        {

            if (!atTx.IsDisposed)
            {
                if (this.IsHandleCreated)
                {
                    atTx.BeginInvoke(new EventHandler(delegate
                    {
                        WriteTxDbg(data.Message);
                    }));
                }
            }
        }

    }
}
