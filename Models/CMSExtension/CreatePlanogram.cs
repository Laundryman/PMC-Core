using Dplo.ViewModels;

namespace CoreSystem2024.CMSModelBuilderModels
{
    public partial class CreatePlanogram
    {

        public string ApiUrl { get; set; }
        public int CountryId { get; set; }
        public int BrandId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserRoles { get; set; }
        public int SystemRole { get; set; }
        public string ImageServerUrl { get; set; }
        public ICollection<StandTypeViewModel> StandTypes { get; set; }

    }
}