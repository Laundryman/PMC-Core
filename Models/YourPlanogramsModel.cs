//using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web.Mvc;
namespace CoreSystem2024.Models
{
    public class YourPlanogramsModel

    {
        public YourPlanogramsModel()
        {
            //this.StandTypes = new HashSet<StandTypeViewModel>();
            //this.Countries = new HashSet<Country>();

        }

        public string ApiUrl { get; set; }
        public int CountryId { get; set; }
        public int BrandId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserRoles { get; set; }
        public int SystemRole { get; set; }
        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Regions { get; set; }
        public List<SelectListItem> StandTypes { get; set; }


    }
}