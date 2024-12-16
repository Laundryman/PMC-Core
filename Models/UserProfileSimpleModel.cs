namespace CoreSystem2024.Models
{
    public class UserProfileSimpleModel
    {

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? CountryId { get; set; }
        public string CountryName { get; set; }
    }
}
