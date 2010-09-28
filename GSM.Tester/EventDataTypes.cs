using System;
namespace GSM.Tester
{
    public class DebugData : EventArgs
    {
        private string msg;

        public DebugData(string messageData)
        {
            msg = messageData;
        }

        public string Message
        {
            get { return msg; }
            set { msg = value; }
        }
    }

}