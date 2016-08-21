
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace JQluv.NetworkScanner.Plugin
{
    public class EquipmentHandlerAPI
    {
        private JQluvWebHandler _requestor = null;
        public EquipmentHandlerAPI(JQluvWebHandler requestor)
        {
            _requestor = requestor;
        }

        public string NewEquipmentRecord(string mac, string ip, string subnet)
        {
            string request_url = String.Format("RQ={0}&Method={1}&eq_mac_address={2}&eq_ip_addr={3}&eq_subnet={4}", "EQUIP", "NewNetEquip", mac, ip, subnet);
            string data = _requestor.SubmitData(JQluvWebHandler.MainUrl + "/includes/itasm.networking.php", request_url);
            var ReturnedRows = Json.Decode(data);
            if (ReturnedRows.length > 0)
                return null;
            return data;

        }

        public bool EquipmentRecordExists(string MacAddress)
        {
            string request_url = String.Format("RQ={0}&Method={1}&eq_mac_address={2}", "EQUIP", "GetByMac", MacAddress);
            string data = _requestor.SubmitData(JQluvWebHandler.MainUrl + "/includes/itasm.networking.php", request_url);
            var ReturnedRows = Json.Decode(data);
            if(ReturnedRows.length > 0)
                return true;
            return false;
        }
    }
}
