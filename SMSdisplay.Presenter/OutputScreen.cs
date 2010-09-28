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
using System.Windows.Forms;
using System.Drawing;

namespace SMSdisplay.GUI
{
    public class OutputScreen
    {
        private int _number;
        private Screen _screen;
        
        public OutputScreen(int number, Screen screen)
        {
            _number = number;
            _screen = screen;
        }

        public string Name
        { get { return String.Format("Screen {0}", _number); } }

        public string Resolution
        { get { return String.Format("{0} x {1}", _screen.Bounds.Width, _screen.Bounds.Height); } }

        public string BitsPerPixel
        { get { return String.Format("{0} bpp", _screen.BitsPerPixel); } }

        public bool IsPrimary
        { get { return _screen.Primary; } }

        public int Left   { get { return _screen.Bounds.Left; } } // +(_number == 2 ? Width : 0)
        public int Top    { get { return _screen.Bounds.Top;  } }
        public int Right  { get { return _screen.Bounds.Right; } }
        public int Bottom { get { return _screen.Bounds.Bottom; } }
        public int Width  { get { return _screen.Bounds.Width; } } // -2
        public int Height { get { return _screen.Bounds.Height; } }

        public static OutputScreenList AllOutputScreens
        {
            get
            {
                OutputScreenList screens = new OutputScreenList();
                OutputScreen screen;
                int i = 1;
                foreach (System.Windows.Forms.Screen scr in System.Windows.Forms.Screen.AllScreens)
                {
                    screen = new OutputScreen(i, scr);
                    screens.Add(screen);
                    i++;
                    //screen = new OutputScreen(i, scr);
                    //screens.Add(screen);
                    //i++;
                    Console.WriteLine("Screen found: {0}x{1} @ {2},{3}", screen.Width, screen.Height, screen.Left, screen.Top);
                }
                return screens;
            }
        }
    }
}
