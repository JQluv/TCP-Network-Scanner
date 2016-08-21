using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JQluv.NetworkScanner.Plugin;
using JQluv.NetworkScanner.Plugin.JQluvITAMAPI;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections.Generic;

namespace JQluv.NetworkScanner.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        private TestContext testContextInstance;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private const string TestIP = "192.168.1.25";
        private const string FromIP = "192.168.1.254";
        private const string ToIP = "192.168.1.1";

        JQluvWebHandler handler = new JQluvWebHandler();
        ScanEngine scanner = new ScanEngine();
        [TestMethod]
        public void RunAccountLoginTest()
        {
            AccountHandlerAPI account = new AccountHandlerAPI(handler);
            account.AuthenticateSession("gloveless", "password");
        }
        [TestMethod]
        public void EquipmentByMacTest()
        {
            AccountHandlerAPI account = new AccountHandlerAPI(handler);
            account.AuthenticateSession("gloveless", "password");
            EquipmentHandlerAPI equip = new EquipmentHandlerAPI(handler);
            equip.EquipmentRecordExists("001122334455");
        }

        [TestMethod]
        public void ScanEngine_InitClass()
        {
            scanner = new ScanEngine();
        }

        [TestMethod]
        public void ScanEngine_FindScanDevice()
        {
            scanner.SetupScanDevice(TestIP);
        }

        [TestMethod]
        public void ScanEngine_FindVPNScanDevice()
        {
            scanner.SetupWinScanDevice(TestIP);
        }

        [TestMethod]
        public void ScanEngine_SendArp()
        {
            scanner.SetupScanDevice(TestIP);
            scanner.SendArpPacket(TestIP);
        }

        [TestMethod]
        public void ScanEngine_SendArpRange()
        {
            scanner.SetupWinScanDevice(TestIP);
            NetworkDevice[] devices = scanner.SendArpPackets(TestIP);
            foreach(NetworkDevice nd in devices) {
                Console.WriteLine(nd.machinesIP);
            }
        }

        [TestMethod]
        public void ScanEngine_SendWinArp()
        {
            scanner.SetupWinScanDevice(TestIP);
            scanner.SendArpPacket(TestIP);
        }

        [TestMethod]
        public void ScanEngine_TestForWMI()
        {
            scanner.SetupWinScanDevice(TestIP);
            NetworkDevice[] devices = scanner.SendArpPackets(TestIP);
            //var device = scanner.GetMyMac();
            foreach (NetworkDevice dev in devices)
            {
                //scanner.SendTcpSyn(dev.macAddress, IPAddress.Parse(FromIP), dev.machinesIP, 2245, 80);
            }
        }

        [TestMethod]
        public void ScanEngine_TestForSingleWMI()
        {
            scanner.SetupWinScanDevice(TestIP);
            var device = scanner.SendArpPacket(ToIP);
            scanner.SendTcpSyn(device, IPAddress.Parse(FromIP), IPAddress.Parse(ToIP), 46735, 22);
        }

        [TestMethod]
        public void ScanEngine_ScanForDeviceTypes()
        {
            scanner.SetupWinScanDevice(TestIP);
            NetworkDevice[] devices = scanner.SendArpPackets(TestIP);
            foreach (NetworkDevice dev in devices)
            {
                List<DeviceType> deviceTypes = scanner.GetDeviceType(dev.macAddress, IPAddress.Parse(FromIP), dev.machinesIP, 46735);
                foreach(DeviceType item in deviceTypes)
                    TestContext.WriteLine("Machine: " + dev.machinesIP + ", Device Type: " + item);
            }
        }

        [TestMethod]
        public void Equipment_RecordExists()
        {
            scanner.SetupWinScanDevice(TestIP);
            var macAddr = scanner.SendArpPacket(TestIP);
            if (macAddr == null)
                return;
            AccountHandlerAPI account = new AccountHandlerAPI(handler);
            account.AuthenticateSession("gloveless", "password");
            EquipmentHandlerAPI equip = new EquipmentHandlerAPI(handler);
            equip.EquipmentRecordExists("" + macAddr);
        }

        [TestMethod]
        public void Equipment_InsertOrReplace()
        {
            scanner.SetupWinScanDevice(TestIP);
            var macAddr = scanner.SendArpPacket(TestIP);
            if (macAddr == null)
                return;
            AccountHandlerAPI account = new AccountHandlerAPI(handler);
            account.AuthenticateSession("gloveless", "password");
            EquipmentHandlerAPI equip = new EquipmentHandlerAPI(handler);
        }
    }
}