/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 * Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Internet packets include IPv4, IPv6, IGMP etc, see
    /// http://en.wikipedia.org/wiki/Internet_Layer
    /// </summary>
    [Serializable]
    public class InternetPacket : Packet
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InternetPacket()
        { }
    }
}
