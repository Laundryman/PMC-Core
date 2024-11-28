using System.Collections.Generic;

namespace diam_planogram.Models.Shop
{
    public class NavModel
    {
        public IEnumerable<CategoryModel> Categories { get; set; }
        public string UserName { get; set; }
        public string CurrentOrderTitle { get; set; }
        public string ClientUrl { get; set; }
        public string PlanogramsUrl { get; set; }
        public bool IsAdminShopper { get; set; }
    }
}