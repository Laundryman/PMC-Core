using dplo.Domain.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace CoreSystem2024.Helpers
{
    public class EmailHelper
    {

        #region Services, managers
        private ILogger<EmailHelper> _logger;
        private IConfiguration _configuration;

        public EmailHelper(ILogger<EmailHelper> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        #endregion

        #region SendEmails

        // MB - I've reused this method for the contact form, as suggested by NR, so maybe we can rename it to be more generic

        /// <summary>
        /// Email Send triggered by Submitting a planogram
        /// </summary>
        /// <param name="emailToSend">The completed email object to send</param>
        /// <param name="accessToken">The granted access token for oauth2</param>
        /// <returns>Email object with success noted</returns>
        public async Task<Email> PlanogramSubmittedEmail(Email emailToSend)
        {
            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];

            var uri = "api/email/send";
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here

            try
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    //request.Headers.Authorization = authorization;
                    var content = JsonConvert.SerializeObject(emailToSend);
                    //request.Headers.Add("Content-Type", "application/json; charset=utf-8");
                    request.Content = new StringContent(content, Encoding.UTF8, "application/json");
                    using (HttpClient httpClient = new HttpClient())
                    {
                        var response = await httpClient.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            return (new Email() { EmailSendSuccess = true });
                        }
                        else
                        {
                            return new Email() { EmailSendSuccess = false };
                        }

                    }
                }
            }
            catch (Exception x)
            {
                throw;
            }
            finally
            {
                //should log something
            }
        }




        public async Task<Email> SendEmail(Email emailToSend)
        {
            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];


            var uri = "api/email/send";
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here

            try
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    //request.Headers.Authorization = authorization;
                    var content = JsonConvert.SerializeObject(emailToSend);
                    //request.Headers.Add("Content-Type", "application/json; charset=utf-8");
                    request.Content = new StringContent(content, Encoding.UTF8, "application/json");
                    using (HttpClient httpClient = new HttpClient())
                    {
                        var response = await httpClient.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            return (new Email() { EmailSendSuccess = true });
                        }
                        else
                        {
                            //SystemLog.ErrorFormat("EmailHelper SendEmail Fail - " + response.Content);
                            _logger.LogError("EmailHelper SendEmail Fail - " + response.Content);
                            return new Email() { EmailSendSuccess = false };
                        }

                    }
                }
            }
            catch (Exception x)
            {
                throw;
            }
            finally
            {
                //should log something
            }
        }



        #endregion


    }
}