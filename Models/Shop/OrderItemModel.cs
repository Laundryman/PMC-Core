
using PMApplication.Entities.OrderAggregate;

namespace diam_planogram.Models.Shop
{
    public class OrderItemModel
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public DateTime DateAdded { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; }
        public string PartNumber { get; set; }
        public decimal Price { get; set; }
        public int? PlanogramId { get; set; }

        //public static explicit operator OrderItemModel(OrderItem orderItem)
        //{
        //    var o = new OrderItemModel();

        //    o.OrderItemId = orderItem.Id;
        //    o.OrderId = orderItem.OrderId;
        //    o.Quantity = orderItem.Quantity;
        //    o.DateAdded = orderItem.DateAdded;
        //    o.PartId = orderItem.PartId;
        //    o.PartName = orderItem.PartName;
        //    o.PartNumber = orderItem.PartNumber;
        //    o.Price = orderItem.Price;
        //    o.PlanogramId = orderItem.PlanogramId;

        //    return o;
        //}
    }
}