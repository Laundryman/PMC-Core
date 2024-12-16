using DiamShopSolution.Models.Shop;
using dplo.Domain.Entities;

namespace diam_planogram.Models.Shop
{
    public class OrderModel
    {
        public int OrderId { get; set; }
        public string OrderTitle { get; set; }
        public int OrderStatus { get; set; }
        public DateTime OrderCreated { get; set; }
        public DateTime OrderUpdated { get; set; }
        //public int OrderCreatedBy { get; set; }
        //public int OrderUpdatedBy { get; set; }
        //public string OrderCreatedByName { get; set; }
        //public string OrderUpdatedByName { get; set; }
        //public int CountryId { get; set; }
        //public int RegionId { get; set; }
        public DateTime? OrderSubmitted { get; set; }

        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }

        public bool HasLegacyItems { get; set; }
        public bool HasFullPlanograms => FullPlanograms?.Any() ?? false;
        public bool HasPartialItems => (IndividualOrderItems?.Any() ?? false) || (PartialPlanograms?.Any() ?? false);

        public int FullPlanogramsQuantity => FullPlanograms?.Sum(x => x.Quantity) ?? 0;

        public IEnumerable<OrderItemInfo> IndividualOrderItems { get; set; }
        public IEnumerable<PlanogramModel> PartialPlanograms { get; set; }
        public IEnumerable<PlanogramModel> FullPlanograms { get; set; }

        public static explicit operator OrderModel(Order order)
        {
            OrderModel o = new OrderModel();
            o.OrderId = order.OrderId;
            o.OrderTitle = order.OrderTitle;
            o.OrderStatus = order.OrderStatus;
            o.OrderCreated = order.OrderCreated;
            o.OrderUpdated = order.OrderUpdated;
            o.OrderSubmitted = order.OrderSubmitted;

            return o;
        }
    }
}