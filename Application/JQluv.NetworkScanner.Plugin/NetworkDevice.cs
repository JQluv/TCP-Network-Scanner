using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;

namespace JQluv.NetworkScanner.Plugin
{
    public class NetworkDevice
    {
        public IPAddress machinesIP = null;
        public PhysicalAddress macAddress = null;
    }
}
