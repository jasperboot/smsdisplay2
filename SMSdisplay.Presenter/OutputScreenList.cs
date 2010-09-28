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
using System.Windows;

namespace SMSdisplay.GUI
{
    public class OutputScreenList : List<OutputScreen>
    {
        public OutputScreen GetOtherThan(OutputScreen notScreen, bool bestTry)
        {
            int availablePosition = 0;
            int notPositionInList = IndexOf(notScreen);
            if (notPositionInList < Count - 1)
            {
                // there is one after notScreen, choose that one
                availablePosition = notPositionInList + 1;
            }
            else if (notPositionInList > 0)
            {
                // there is one before notScreen, choose that one
                availablePosition = notPositionInList - 1;
            }
            else if (!bestTry)
            {
                // there's only no screen, so no luck...
                return null;
            }
            // else 
            // there's only one screen, notScreen is on it, but what else can you do...
            // availablePosition stays 0

            return this[availablePosition];
        }
    }
}
