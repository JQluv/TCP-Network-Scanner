using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JQluv.NetworkScanner.Plugin.JQluvITAMAPI
{
    public class AccountHandlerAPI
    {
        private JQluvWebHandler _requestor = null;
        public AccountHandlerAPI(JQluvWebHandler requestor)
        {
            _requestor = requestor;
        }

        public bool AuthenticateSession(string username, string password) {
            string request_url = String.Format("username={0}&pass={1}&RQ={2}&Method={3}", username, password, "ACCTM", "Login");
            string return_data = _requestor.SubmitData(JQluvWebHandler.MainUrl + "/includes/itasm.networking.php", request_url);
            if (return_data == "1")
                return true;
            return false;
        }

        public bool ClearSession() {
            _requestor.SubmitData(JQluvWebHandler.MainUrl + "/logout", "");
            return true;
        }
    }
}
