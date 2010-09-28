/*
 * This source code is taken from the following project (possibly with slight modifications):
 * Library for Decode/Encode SMS PDU (smspdulib)
 * http://www.codeproject.com/KB/windows/smspdulib.aspx
 * 
 * The project is released under the The Code Project Open License (CPOL)
 * http://www.codeproject.com/info/cpol10.aspx
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace GSM
{
	class UnknownSMSTypeException : Exception
	{
		public UnknownSMSTypeException(byte pduType) : 
			base(string.Format("Unknow SMS type. PDU type binary: {0}.", Convert.ToString(pduType, 2))) { }
	}
}
