namespace diam_planogram.Models.Shop
{
    public class OrderListModel
    {
        public string UserName { get; set; }

        public IEnumerable<OrderModel> OrdersInProgress { get; set; }
        public IEnumerable<OrderModel> OrdersSubmitted { get; set; }
        public IEnumerable<OrderModel> OrdersReceived { get; set; }
        public IEnumerable<OrderModel> OrdersCompleted { get; set; }
        public IEnumerable<OrderModel> OrdersCancelled { get; set; }

    }
}