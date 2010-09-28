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

namespace GSM.Tester
{
    public partial class GSMcomDbg : Form
    {
        private Communicator GSMcon = new Communicator();

        public GSMcomDbg()
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
            GSMcon = new Communicator();
            GSMcon.NewRxDebugData += new EventHandler<DebugData>(newRxData);
            GSMcon.NewTxDebugData += new EventHandler<DebugData>(newTxData);
            GSMcon.Connect();
        }

        private void newRxData(object sender, DebugData data)
        {

            if (!atRx.IsDisposed)
            {
                atRx.Invoke(new EventHandler(delegate
                {
                    WriteRxDbg(data.Message);
                }));
            }
        }

        private void newTxData(object sender, DebugData data)
        {

            if (!atTx.IsDisposed)
            {
                atTx.Invoke(new EventHandler(delegate
                {
                    WriteTxDbg(data.Message);
                }));
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            GSMcon.SendCommand(textBox1.Text);
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            GSMcon.Connection = !GSMcon.Connection;
        }

        private void GSMcomDbg_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
