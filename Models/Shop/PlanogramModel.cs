using PMApplication.Entities.OrderAggregate;

namespace DiamShopSolution.Models.Shop
{
    public class PlanogramModel
    {
        public string Name { get; set; }
        public long PlanogramId { get; set; }
        public IEnumerable<OrderItemInfo> OrderItems { get; set; }
        public bool IsFullPlano { get; set; }
        public string Dimensions { get; set; }
        public string ClusterName { get; set; }
        public string ClusterPartNumber { get; set; }
        public string StandName { get; set; }
        public string StandPartNumber { get; set; }
        public long? OrderPlanogramId { get; set; }
        public int? Quantity { get; set; }
        public int? InitialQuantity { get; set; }

    }
}