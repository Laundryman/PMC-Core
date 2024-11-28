using System.Collections.Generic;
using dplo.Domain.Entities;

namespace DiamShopSolution.Models.Shop
{
    public class PlanogramModel
    {
        public string Name { get; set; }
        public int PlanogramId { get; set; }
        public IEnumerable<OrderItemInfo> OrderItems { get; set; }
        public bool IsFullPlano { get; set; }
        public string Dimensions { get; set; }
        public string ClusterName { get; set; }
        public string ClusterPartNumber { get; set; }
        public string StandName { get; set; }
        public string StandPartNumber { get; set; }
        public int? OrderPlanogramId { get; set; }
        public int? Quantity { get; set; }
        public int? InitialQuantity { get; set; }

    }
}