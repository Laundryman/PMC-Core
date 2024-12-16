namespace CoreSystem2024.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public int BrandId { get; set; }
        public string OrderTitle { get; set; }
        public int OrderStatus { get; set; }
        public DateTime OrderCreated { get; set; }
        public DateTime OrderUpdated { get; set; }
        public int OrderCreatedBy { get; set; }
        public int OrderUpdatedBy { get; set; }
        public string OrderCreatedByName { get; set; }
        public string OrderUpdatedByName { get; set; }
        public int CountryId { get; set; }
        public int RegionId { get; set; }

    }
}