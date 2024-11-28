using System.Configuration;
using System.Net;
using System.Text;
using dplo.Service;
using Newtonsoft.Json;

namespace CoreSystem.Controllers
{

    public class EditSectionPlanogramApiController : UmbracoApiController
    {

        #region Services, managers

        public IDependencyResolver _dpRes = DependencyResolver.Current;


        public IStandService StandService
        {
            get { return _dpRes.GetService<IStandService>(); }
        }
        public IPlanogramService PlanogramService
        {
            get { return _dpRes.GetService<IPlanogramService>(); }
        }
        public IBrandService BrandService
        {
            get { return _dpRes.GetService<IBrandService>(); }
        }
        public ICountryService CountryService
        {
            get { return _dpRes.GetService<ICountryService>(); }
        }
        //public ICategoryService CategoryService
        //{
        //    get { return _dpRes.GetService<ICategoryService>(); }
        //}
        //public IProductService ProductService
        //{
        //    get { return _dpRes.GetService<IProductService>(); }
        //}
        #endregion


        #region LocalApiCalls


        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetPlanogramSections(int planogramId)
        {
            try { 
                //we need to re-auth using the reauth process
                ////AuthHelper.ReAuth(Authorization, client);

                var response = await GetPlanogramSectionsCall(planogramId);

                //Get the json data from the result
                var sections = new PlanogramSectionsModel();
                //var response =
                if (response.IsSuccessStatusCode)
                {
                    var sectionsJson = response.Content.ReadAsStringAsync();
                    sections = JsonConvert.DeserializeObject<PlanogramSectionsModel>(sectionsJson.Result);
                }
                else
                {
                    //Something has gone wrong, handle it here
                }
                return Request.CreateResponse(HttpStatusCode.OK, sections);

            }
            catch (Exception ex)
            {
                // Do Something to show not valid
                return null;
            }
        }

        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> CreatePlanogramSection([FromBody] NewSectionViewModel section)
        {

            try
            {
                //we need to re-auth using the reauth process
                //AuthHelper.ReAuth(Authorization, client);


                var response = await CreatePlanogramSectionCall(section);

                //Get the json data from the result
                var sections = new PlanogramSectionsModel();
                //var response =
                if (response.IsSuccessStatusCode)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, sections);
                }
                else
                {
                    //Something has gone wrong, handle it here
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, response.Content);

                }

            }
            catch (Exception ex)
            {
                // Do Something to show not valid
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }


        }

        public async Task<HttpResponseMessage> ReleasePlanogramSection(int planogramSectionId)
        {

            try
            {
                //we need to re-auth using the reauth process
                //AuthHelper.ReAuth(Authorization, client);


                var response = await DeletePlanogramSectionCall(planogramSectionId);

                if (response.IsSuccessStatusCode)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    //Something has gone wrong, handle it here
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, response.Content);
                }

            }
            catch (Exception ex)
            {
                // Do Something to show not valid
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

        }


        public async Task<HttpResponseMessage> EditPlanogramSection(string virtualPlanogramId)
        {



            try
            {
                string domain = ConfigurationManager.AppSettings["apiUrl"];
                //we need to re-auth using the reauth process
                //var accessToken = //AuthHelper.ReAuth(Authorization, client);
                var response = await EditPlanogramSectionCall(virtualPlanogramId);

                //Get the json data from the result
                var section = new PlanogramSectionViewModel();
                //var response =
                if (response.IsSuccessStatusCode)
                {
                    var sectionJson = response.Content.ReadAsStringAsync();
                    section = JsonConvert.DeserializeObject<PlanogramSectionViewModel>(sectionJson.Result);
                    section.ApiUrl = domain;
                    section.AccessToken = accessToken;
                }
                else
                {
                    //Something has gone wrong, handle it here
                }
                return Request.CreateResponse(HttpStatusCode.OK, section);

            }
            catch (Exception ex)
            {
                // Do Something to show not valid
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        #endregion


        #region remote api calls

        private async Task<HttpResponseMessage> GetPlanogramSectionsCall(int planogramId)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];
            //Confirm the authorization so we can call the api 
            //var currentAuth = AuthHelper.GetAuth(HttpContext.Current.Request);
            //var accessToken = currentAuth.AccessToken;
            //var country = CountryService.GetCountryByThreeLetterISO(UserInfo["countryThreeLetterISO"]);


            var uri = "api/v2/planogram/sections/get/" + brand + "/" + planogramId ;
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    try
                    {
                        var response = await httpClient.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            return response;
                        }
                        else
                        {
                            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        }

                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }
        }

        private async Task<HttpResponseMessage> CreatePlanogramSectionCall(NewSectionViewModel section)
        {

            try
            {
                string domain = ConfigurationManager.AppSettings["apiUrl"];
                string brand = ConfigurationManager.AppSettings["brand"];
                //Confirm the authorization so we can call the api 
                //var currentAuth = AuthHelper.GetAuth(HttpContext.Current.Request);
                //var accessToken = currentAuth.AccessToken;
                //var country = CountryService.GetCountryByThreeLetterISO(UserInfo["countryThreeLetterISO"]);

                //var getPartURL = $("#apiURL").val() + "api/clusters/get/" + $('#brandId').val() + "/" + standId + "?token=" + _authCode + "&callback=?";

                var uri = "api/v2/planogram/section/create";
                var url = string.Format("{0}{1}", domain, uri);
                //maybe log something here

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    //request.Headers.Authorization = authorization;
                    var content = JsonConvert.SerializeObject(section);
                    //request.Headers.Add("Content-Type", "application/json; charset=utf-8");
                    request.Content = new StringContent(content, Encoding.UTF8, "application/json");
                    using (HttpClient httpClient = new HttpClient())
                    {
                        var response = await httpClient.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            return (new HttpResponseMessage(HttpStatusCode.OK));
                        }
                        else
                        {
                            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

        }

        private async Task<HttpResponseMessage> DeletePlanogramSectionCall(int planogramSectionId)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];
            //Confirm the authorization so we can call the api 
            //var currentAuth = AuthHelper.GetAuth(HttpContext.Current.Request);
            //var accessToken = currentAuth.AccessToken;
            //var country = CountryService.GetCountryByThreeLetterISO(UserInfo["countryThreeLetterISO"]);


            var uri = "api/v2/planogram/section/delete/" + planogramSectionId ;
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    return await httpClient.GetAsync(url);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

        }
        private async Task<HttpResponseMessage> EditPlanogramSectionCall(string virtualPlanogramId)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];
            //Confirm the authorization so we can call the api 
            //var currentAuth = AuthHelper.GetAuth(HttpContext.Current.Request);
            //var accessToken = currentAuth.AccessToken;

            var uri = "api/v2/planosection/get/" + virtualPlanogramId ;
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        return response;
                    }
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    }

                }
            }


        }

        #endregion

    }
}
