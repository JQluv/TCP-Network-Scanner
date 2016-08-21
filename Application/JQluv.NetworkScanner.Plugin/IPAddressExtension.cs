using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace JQluv.NetworkScanner.Plugin
{
    public static class IPAddressExtensions
    {
        public static IPAddress GetBroadcastAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
            }
            return new IPAddress(broadcastAddress);
        }

        public static bool IsInSameSubnet(this IPAddress address2, IPAddress address, IPAddress subnetMask)
        {
            IPAddress network1 = address.GetNetworkAddress(subnetMask);
            IPAddress network2 = address2.GetNetworkAddress(subnetMask);

            return network1.Equals(network2);
        }
        public static byte[,] ParseRange(string str)
        {
            if (string.IsNullOrEmpty(str)) throw new ArgumentException("str");

            string[] partStr = str.Split('.');
            if (partStr.Length != 4) throw new FormatException();

            byte[,] range = new byte[4, 2];
            for (int i = 0; i < 4; i++)
            {
                string[] rangeStr = partStr[i].Split('-');
                if (rangeStr.Length > 2) throw new FormatException();

                range[i, 0] = byte.Parse(rangeStr[0]);
                range[i, 1] = byte.Parse(rangeStr[Math.Min(rangeStr.Length - 1, 1)]);

                // Remove this to allow ranges to wrap around.
                // For example: 254-4 = 254, 255, 0, 1, 2, 3, 4
                if (range[i, 1] < range[i, 0]) throw new FormatException();
            }

            return range;
        }

        public static IEnumerable<IPAddress> Enumerate(byte[,] range)
        {
            if (range.GetLength(0) != 4) throw new ArgumentException("range");
            if (range.GetLength(1) != 2) throw new ArgumentException("range");

            for (byte a = range[0, 0]; a != (byte)(range[0, 1] + 1); a++)
            {
                for (byte b = range[1, 0]; b != (byte)(range[1, 1] + 1); b++)
                {
                    for (byte c = range[2, 0]; c != (byte)(range[2, 1] + 1); c++)
                    {
                        for (byte d = range[3, 0]; d != (byte)(range[3, 1] + 1); d++)
                        {
                            yield return new IPAddress(new byte[] { a, b, c, d });
                        }
                    }
                }
            }
        }
    }
}
