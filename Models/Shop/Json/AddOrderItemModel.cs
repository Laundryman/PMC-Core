namespace CoreSystem2024.Models.Shop.Json
{
    public class AddOrderItemModel
    {
        public int PartId { get; set; }
        public int Quantity { get; set; }
        public int? OrderId { get; set; }
    }
}