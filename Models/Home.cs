namespace CoreSystem2024.CMSModelBuilderModels
{
    public partial class Home
    {

        public string ApiUrl { get; set; }
        public int CountryId { get; set; }
        public int BrandId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserRoles { get; set; }
        public string CountryName { get; set; }
        public string CountryFlag { get; set; }
        public int SystemRole { get; set; }
        public string UserName { get; set; }
        //public virtual ICollection<StandTypeViewModel> StandTypes { get; set; }
        //public virtual ICollection<Part> Parts { get; set; }

        //public DateTime? OrderWindowClosing { get; set; }
        //public DateTime? OrderWindowOpening { get; set; }

        public string OrderWindowClosing { get; set; }
        public string OrderWindowOpening { get; set; }

        public string OrderWindowCalendar { get; set; }

    }
}