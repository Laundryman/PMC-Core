using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CoreSystem2024.CMSModelBuilderModels
{
    public partial class YourPlanograms
    {
        public string ApiUrl { get; set; }
        public int CountryId { get; set; }
        public int RegionId { get; set; }
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
