/*
 * This source code is taken from the following project (possibly with slight modifications):
 * Library for Decode/Encode SMS PDU (smspdulib)
 * http://www.codeproject.com/KB/windows/smspdulib.aspx
 * 
 * The project is released under the The Code Project Open License (CPOL)
 * http://www.codeproject.com/info/cpol10.aspx
 */

using System;

namespace GSM.Messages
{

	public class StatusReport : BaseSMS
	{
		#region Members
		protected DateTime _reportTimeStamp;
		protected ReportStatus _reportStatus;
		protected byte _messageReference;
		protected string _phoneNumber;
		protected DateTime _serviceCenterTimeStamp;
		#endregion

		#region Public Properties
		public byte MessageReference { get { return _messageReference; } }
		public string PhoneNumber { get { return _phoneNumber; } }
		public DateTime ServiceCenterTimeStamp { get { return _serviceCenterTimeStamp; } }
		public DateTime ReportTimeStamp { get { return _reportTimeStamp; } }
		public ReportStatus ReportStatus { get { return _reportStatus; } }

		public override SMSType Type { get { return SMSType.StatusReport; } }
		#endregion

		#region Public Statics
		public static void Fetch(StatusReport statusReport, ref string source)
		{
			BaseSMS.Fetch(statusReport, ref source);

			statusReport._messageReference = PopByte(ref source);
			statusReport._phoneNumber = PopPhoneNumber(ref source);
			statusReport._serviceCenterTimeStamp = PopDate(ref source);
			statusReport._reportTimeStamp = PopDate(ref source);
			statusReport._reportStatus = (ReportStatus) PopByte(ref source);
		}
		#endregion
	}

    public enum ReportStatus
    {
        NoResponseFromSME = 0x62,
        NotSend = 0x60,
        Success = 0
    }

}

