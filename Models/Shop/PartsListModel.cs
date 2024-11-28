using System.Collections.Generic;

namespace diam_planogram.Models.Shop
{
    public class PartsListModel
    {
        public int? categoryId { get; set; }
        public string pageTitle { get; set; }
        public IEnumerable<PartModel> parts { get; set; }
        public int? standTypeId { get; set; }
    }
}