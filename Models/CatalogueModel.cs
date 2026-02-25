
using diam_planogram.Models.Shop;
using PMApplication.Entities;
using PMApplication.Entities.PartAggregate;
using PMApplication.Entities.StandAggregate;
using PMApplication.Entities.ProductAggregate;

namespace CoreSystem2024.Models
{
    public class Catalogue
    {
        public Catalogue()
        {
            this.Products = new HashSet<Product>();
            this.ChildCategories = new HashSet<CategoryModel>();
            this.ParentCategories = new List<CategoryModel>();
            this.StandTypes = new HashSet<StandType>();
            this.Parts = new HashSet<Part>();

        }

        public string ApiUrl { get; set; }
        public int CountryId { get; set; }
        public int BrandId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual List<CategoryModel> ParentCategories { get; set; }
        public virtual ICollection<CategoryModel> ChildCategories { get; set; }
        public virtual ICollection<StandType> StandTypes { get; set; }
        public virtual ICollection<Part> Parts { get; set; }


    }
}
