using System;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;
using PacketDotNet;
using PacketDotNet.Tcp;
using PacketDotNet.Utils;
using SharpPcap;
using SharpPcap.AirPcap;
using SharpPcap.LibPcap;

namespace JQluv.NetworkScanner.Plugin
{
    public class ScanEngine
    {
        private Dictionary<DeviceType, ushort> ports = new Dictionary<DeviceType, ushort>();
        private PhysicalAddress MyMac = null;
        private Stopwatch EthPacketTimeout = new Stopwatch();
        LibPcapLiveDevice ScanDevice = null;
        public ScanEngine()
        {
            ports.Add(DeviceType.Windows, 135);
            ports.Add(DeviceType.Android, 5228);
            ports.Add(DeviceType.OSX_10_8, 312);
            ports.Add(DeviceType.Linux, 22);
        }

        public bool SetupWinScanDevice(string to_addr)
        {
            bool found = false;
            var devices = SharpPcap.WinPcap.WinPcapDeviceList.Instance;
            if (devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                ScanDevice = null;
            }
            IPAddress toAddre = IPAddress.Parse(to_addr);
            int i = 0;
            foreach (var dev in devices)
            {
                if (dev.Addresses[1].Addr.ipAddress == null) continue;
                Console.WriteLine("{0}) {1} {2} {3}", i, dev.Name, dev.Description, dev.Addresses[1].Addr.ipAddress.ToString());
                i++;
                IPAddress IntAddr = null;
                for (int addrI = 0; addrI < dev.Addresses.Count; addrI++)
                {
                    if (dev.Addresses[addrI].Addr.ipAddress == null) continue;
                    if (dev.Addresses[addrI].Addr.ipAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) continue;
                    IPAddress.TryParse(dev.Addresses[addrI].Addr.ipAddress.ToString(), out IntAddr);
                    if (IPAddressExtensions.IsInSameSubnet(toAddre, IntAddr, dev.Addresses[addrI].Netmask.ipAddress))
                    {
                        found = true;
                        ScanDevice = dev;
                        dev.Open();
                        MyMac = dev.MacAddress;
                        dev.Close();
                        break;
                    }
                }
                if (found)
                    break;
            }
            return found;
        }

        public bool SetupScanDevice(string to_addr)
        {
            bool found = false;
            var devices = SharpPcap.WinPcap.WinPcapDeviceList.Instance;
            if (devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                ScanDevice = null;
            }
            IPAddress toAddre = IPAddress.Parse(to_addr);
            int i = 0;
            foreach (var dev in devices)
            {
                Console.WriteLine("{0}) {1} {2} {3}", i, dev.Name, dev.Description, dev.Addresses[1].Addr.ipAddress.ToString());
                i++;
                IPAddress IntAddr = null;
                for (int addrI = 0; addrI < dev.Addresses.Count; addrI++)
                {
                    if (dev.Addresses[addrI].Addr.ipAddress == null) continue;
                    if (dev.Addresses[addrI].Addr.ipAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) continue;
                    IPAddress.TryParse(dev.Addresses[addrI].Addr.ipAddress.ToString(), out IntAddr);
                    if (IPAddressExtensions.IsInSameSubnet(toAddre, IntAddr, dev.Addresses[addrI].Netmask.ipAddress))
                    {
                        found = true;
                        ScanDevice = dev;
                        dev.Open();
                        MyMac = dev.MacAddress;
                        dev.Close();
                        break;
                    }
                }
                if (found)
                    break;
            }
            return found;
        }

        public PhysicalAddress SendArpPacket(string to_addr)
        {
            System.Net.IPAddress ip;
            System.Net.IPAddress.TryParse(to_addr, out ip);
            ARP arper = new ARP(ScanDevice);
            var resolvedMacAddress = arper.Resolve(ip);
            if(resolvedMacAddress == null)
                return null;
            else
                return resolvedMacAddress;
        }

        public NetworkDevice[] SendArpPackets(string to_addr)
        {
            List<NetworkDevice> devices = new List<NetworkDevice>();
            System.Net.IPAddress ip;
            System.Net.IPAddress.TryParse(to_addr, out ip);
            byte[,] range = IPAddressExtensions.ParseRange(to_addr + "-255");
            ARP arper = new ARP(ScanDevice);
            foreach (IPAddress addr in IPAddressExtensions.Enumerate(range))
            {
                var resolvedMacAddress = arper.Resolve(addr);
                if (resolvedMacAddress == null)
                    continue;
                else
                {
                    devices.Add(new NetworkDevice() { machinesIP = addr, macAddress = resolvedMacAddress });
                }
            }
            return devices.ToArray();
        }

        public PhysicalAddress GetMyMac()
        {
            ScanDevice.Open();
            PhysicalAddress addr = ScanDevice.MacAddress;
            ScanDevice.Close();
            return addr;
        }

        public List<DeviceType> GetDeviceType(PhysicalAddress theirMac, IPAddress fromIP, IPAddress toIP, ushort fromPort)
        {
            List<DeviceType> DeviceDescriptorPorts = new List<DeviceType>();
            foreach (KeyValuePair<DeviceType, ushort> entry in ports)
            {
                Packet response = SendTcpSyn(theirMac, fromIP, toIP, fromPort, entry.Value);
                if (response == null) continue;
                IPv4Packet recIPv4 = ((IPv4Packet)response.PayloadPacket);
                TcpPacket RecTcp = (TcpPacket)response.PayloadPacket.PayloadPacket;
                if (RecTcp.Syn == true && RecTcp.Ack == true && RecTcp.Rst == false)
                {
                    DeviceDescriptorPorts.Add(entry.Key);
                    break;
                }
            }
            return DeviceDescriptorPorts;
        }

        static int lLen = EthernetFields.HeaderLength;

        public Packet SendTcpSyn(PhysicalAddress theirMac, IPAddress fromIP, IPAddress toIP, ushort fromPort, ushort toPort)
        {
            EthPacketTimeout.Reset();
            EthernetPacket packet = new EthernetPacket(MyMac, theirMac, EthernetPacketType.IpV4);

            byte[] asd = new byte[32];
            int pos = 20;
            asd[pos++] = 0x02;
            asd[pos++] = 0x04;
            asd[pos++] = 0x05;
            asd[pos++] = 0xb4;
            asd[pos++] = 0x01;
            asd[pos++] = 0x03;
            asd[pos++] = 0x03;
            asd[pos++] = 0x08;
            asd[pos++] = 0x01;
            asd[pos++] = 0x01;
            asd[pos++] = 0x04;
            asd[pos++] = 0x02;
            ByteArraySegment bas = new ByteArraySegment(asd);
            
            IPv4Packet ipPacket = (IPv4Packet)IPv4Packet.RandomPacket(IpVersion.IPv4);
            TcpPacket tcpPacket = new TcpPacket(bas);
            ipPacket.TimeToLive = 128;
            ipPacket.Protocol = IPProtocolType.TCP;
            ipPacket.FragmentFlags = 0x20;
            ipPacket.Version = IpVersion.IPv4;
            ipPacket.DestinationAddress = toIP;
            ipPacket.SourceAddress = fromIP;
            ipPacket.PayloadPacket = tcpPacket;
            ipPacket.FragmentFlags = 0x02;
            ipPacket.UpdateIPChecksum();

            tcpPacket.ParentPacket = ipPacket;
            tcpPacket.SourcePort = fromPort;
            tcpPacket.DestinationPort = toPort;
            tcpPacket.Syn = true;
            tcpPacket.Ack = false;
            tcpPacket.WindowSize = 8192;
            tcpPacket.AcknowledgmentNumber = 0;
            tcpPacket.SequenceNumber = 0;
            tcpPacket.DataOffset = TcpPacket.HeaderMinimumLength + 4;
            tcpPacket.UpdateTCPChecksum();
            packet.PayloadPacket = ipPacket;
            ipPacket.ParentPacket = packet;

            ScanDevice.Open();
            ScanDevice.SendPacket(packet);
            Packet packet_reply = FindResponse(packet, EthPacketTimeout);
            ScanDevice.Close();
            return packet_reply;
        }

        public EthernetPacket FindResponse(EthernetPacket sentPacket, Stopwatch watch)
        {
            watch.Start();
            RawCapture reply;
            while ((reply = ScanDevice.GetNextPacket()) == null) ;
            watch.Stop();
            if (watch.ElapsedMilliseconds > 120)
                return null;
            EthernetPacket recPacket = (EthernetPacket)EthernetPacket.ParsePacket(reply.LinkLayerType, reply.Data);
            if (recPacket.PayloadPacket == null)
                return FindResponse(sentPacket, watch);
            if (recPacket.PayloadPacket.PayloadPacket is TcpPacket && recPacket.PayloadPacket is IPv4Packet)
            {
                IPv4Packet sentIp4 = ((IPv4Packet)sentPacket.PayloadPacket);
                TcpPacket sentTcp = (TcpPacket)sentPacket.PayloadPacket.PayloadPacket;

                IPv4Packet recIPv4 = ((IPv4Packet)recPacket.PayloadPacket);
                TcpPacket RecTcp = (TcpPacket)recPacket.PayloadPacket.PayloadPacket;
                if (recPacket.SourceHwAddress.Equals(sentPacket.DestinationHwAddress) && RecTcp.DestinationPort == sentTcp.SourcePort)
                {
                    return (EthernetPacket)recPacket;
                }
                else
                    return FindResponse(sentPacket, watch);
            }
            else
                return FindResponse(sentPacket, watch);

        }
    }
}
