namespace CoreSystem2024.Models.Shop.Json
{
    public class UpdateOrderPlanogramQuantityModel
    {
        public int OrderPlanogramId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
    }
}