
using PMApplication.Dtos;

namespace CoreSystem2024.Models
{
    public class CreateStandModel

    {
        public CreateStandModel()
        {
            this.StandTypes = new HashSet<StandTypeDto>();
            //this.Parts = new HashSet<Part>();

        }

        public string ApiUrl { get; set; }
        public int CountryId { get; set; }
        public int BrandId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserRoles { get; set; }
        public int SystemRole { get; set; }
        public virtual ICollection<StandTypeDto> StandTypes { get; set; }
        //public virtual ICollection<Part> Parts { get; set; }



    }
}