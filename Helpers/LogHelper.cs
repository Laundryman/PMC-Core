using System.Configuration;
using System.Net;
using dplo.Domain;

namespace CoreSystem.Helpers
{
    public static class LogHelper
    {
        private static readonly ILog SystemLog = LogManager.GetLogger("System");
        /// <summary>
        /// Logs an action in the LMAuditLog table
        /// </summary>
        /// <param name="action">action being logged from LogActionEnum</param>
        /// <param name="referrer">referrer of the original action</param>
        /// <param name="planoId">0 or PlanogramId if relevent</param>
        /// <param name="orderId">0 or OrderId if relevant</param>
        /// <returns></returns>
        public static bool LogAction(int action, string referrer, long planoId, int orderId)
        {
            try
            {
                //TODO NEED TO LOG END OF SESSION
                //DiamUserInfo userInfo;
                string domain = ConfigurationManager.AppSettings["apiUrl"];
                string brand = ConfigurationManager.AppSettings["brand"];
                
                Uri myUri = new Uri(domain);
                var logMessage = "";
                switch (action)
                {
                    case (int)LogActionEnum.SubmitPlano:
                        logMessage = "User with id {0} from IP address {1} submitted planogram";
                        myUri = new Uri(domain + "api/v2/log/plano-action/" + action + "/" + logMessage + "/" + planoId);
                        break;
                    case (int)LogActionEnum.ApprovePlano:
                        logMessage = "User with id {0} from IP address {1} approved planogram";
                        myUri = new Uri(domain + "api/v2/log/plano-action/" + action + "/" + logMessage + "/" + planoId);
                        break;
                    case (int)LogActionEnum.EditPlano:
                        logMessage = "User with id {0} from IP address {1} edited planogram";
                        myUri = new Uri(domain + "api/v2/log/plano-action/" + action + "/" + logMessage + "/" + planoId);
                        break;
                    case (int)LogActionEnum.CreateOrder:
                        logMessage = "User with id {0} from IP address {1} created order";
                        myUri = new Uri(domain + "api/v2/log/order-action/" + action + "/" + logMessage + "/" + orderId);
                        break;
                    case (int)LogActionEnum.SubmitOrder:
                        logMessage = "User with id {0} from IP address {1} submitted order";
                        myUri = new Uri(domain + "api/v2/log/order-action/" + action + "/" + logMessage + "/" + orderId);
                        break;
                    case (int)LogActionEnum.EditOrder:
                        logMessage = "User with id {0} from IP address {1} edited order";
                        myUri = new Uri(domain + "api/v2/log/order-action/" + action + "/" + logMessage + "/" + orderId);
                        break;
                    case (int)LogActionEnum.ApproveOrder:
                        logMessage = "User with id {0} from IP address {1} approved order";
                        myUri = new Uri(domain + "api/v2/log/order-action/" + action + "/" + logMessage + "/" + orderId);
                        break;
                    case (int)LogActionEnum.SessionEnd:
                        logMessage = "User logged out with id {0} from IP address {1}";
                        myUri = new Uri(domain + "api/v2/log/sessionend");
                        break;
                }

                var myWebRequest = WebRequest.Create(myUri);
                var myHttpWebRequest = (HttpWebRequest)myWebRequest;
                myHttpWebRequest.PreAuthenticate = true;
                //myHttpWebRequest.Headers.Add("Authorization", "Bearer " + authorization.AccessToken);
                myHttpWebRequest.Referer = referrer;

                myHttpWebRequest.Accept = "application/json";

                var myWebResponse = myWebRequest.GetResponse();
                var responseStream = myWebResponse.GetResponseStream();
                //if (responseStream == null) return false;

                //var myStreamReader = new StreamReader(responseStream, Encoding.Default);
                //var json = myStreamReader.ReadToEnd();

                responseStream.Close();
                myWebResponse.Close();
                return true;
            }
            catch (Exception ex)
            {
                //don't do anything yet
                //probably need to log this in the local log
                SystemLog.InfoFormat("DeleteSession failed log entry Getting UserInfo ");
                return false;

            }
        }
    }
}