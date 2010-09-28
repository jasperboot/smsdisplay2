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
using GSM.AT;
using System.Windows.Data;
using System.Windows.Media;


namespace SMSdisplay.GUI
{
    [ValueConversion(typeof(SerialPortType), typeof(ImageSource))]
    public class ConnectionTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(ImageSource))
                throw new InvalidOperationException("The target must be a ImageSource");
            string imageFile = "serial.png";
            switch ((SerialPortType)value)
            {
                case SerialPortType.RS232:
                    imageFile = "serial.png";
                    break;
                case SerialPortType.USB:
                    imageFile = "usb.png";
                    break;
                case SerialPortType.Modem:
                    imageFile = "modem.png";
                    break;
                case SerialPortType.Phone:
                    imageFile = "phone.png";
                    break;
            }
            return new Uri("ConnectionImages/"+imageFile, UriKind.Relative);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
