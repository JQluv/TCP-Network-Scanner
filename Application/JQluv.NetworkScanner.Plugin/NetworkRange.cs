using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace JQluv.NetworkScanner.Plugin
{
    public class NetworkRange
    {
        private IPAddress _gateway;
        private IPAddress _start;
        private IPAddress _subnet;

        public NetworkRange(IPAddress gateway, IPAddress start, IPAddress subnet)
        {
            _gateway = gateway;
            _start = start;
            _subnet = subnet;
        }
    }
}
