
using diam_planogram.Models.Shop;
using PMApplication.Dtos;
using PMApplication.Entities;
using PMApplication.Entities.PartAggregate;
using PMApplication.Entities.StandAggregate;

namespace CoreSystem2024.CMSModelBuilderModels
{
    public partial class Catalogue
    {
        public string ApiUrl { get; set; }
        public string ServerUrl { get; set; }
        public int CountryId { get; set; }
        public int BrandId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public virtual List<Product> Products { get; set; }
        public virtual List<CategoryDto> ParentCategories { get; set; }
        public virtual List<CategoryDto> ChildCategories { get; set; }
        public virtual List<StandType> StandTypes { get; set; }
        public virtual List<Part> Parts { get; set; }


    }
}
