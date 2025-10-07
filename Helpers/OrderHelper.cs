using diam_planogram.Models.Shop;
using DiamShopSolution.Models.Shop;
using Microsoft.Extensions.Configuration;
using PMApplication.Entities.OrderAggregate;
using PMApplication.Entities.PartAggregate;
using PMApplication.Interfaces.ServiceInterfaces;

namespace diam_planogram.Helpers
{
    public static class OrderHelper
    {
        public static decimal GetPartItemCost(this PartInfo part)
        {
            if (!part.LaunchDate.IsMinNullOrEmpty()
                && part.LaunchDate >= DateTime.Now
                && part.LaunchPrice != null)
            {
                return part.LaunchPrice.Value;
            }

            return part.UnitCost;
        }


        public static bool IsMinNullOrEmpty(this DateTime? date)
        {
            return date == null
                || date == new DateTime()
                || date == DateTime.MinValue;
        }

        public static OrderModel BuildFullOrder(Order order, IOrderService orderService, IPlanogramService planogramService, IConfiguration config)
        {
            var imageDomain = config["AppSettings:cassette-photo-url"] ?? string.Empty;

            var orderModel = (OrderModel)order;

            // first get all the order items
            var allOrderItems = orderService.GetOrderItemInfos(order.Id).Result;

            orderModel.HasLegacyItems = allOrderItems.Any(x => !x.Shoppable);

            // remove non-shoppable items unless they are in a full plano (otherwise a full-plano comprised of entirely non-shop items would not show here)
            allOrderItems = allOrderItems.Where(x => x.Shoppable || (x.IsFullPlano.HasValue && x.IsFullPlano.Value)).ToList();

            // build the pack shot image url
            foreach (var orderItemInfo in allOrderItems)
            {
                orderItemInfo.PackShotImageSrc = imageDomain + orderItemInfo.PackShotImageSrc;
            }

            // quantity and price exclude full plano items
            var totalQuantity = allOrderItems.Where(x => !x.IsFullPlano.HasValue || !x.IsFullPlano.Value).Sum(x => x.Quantity);
            var totalPrice = allOrderItems.Where(x => !x.IsFullPlano.HasValue || !x.IsFullPlano.Value).Sum(x => x.Price * x.Quantity);

            orderModel.TotalPrice = totalPrice;
            orderModel.TotalQuantity = totalQuantity;

            // get the order items not associated with a plano
            orderModel.IndividualOrderItems = allOrderItems.Where(x => x.PlanogramId == null).OrderByDescending(x => x.DateAdded);



            // get the order items associated with a FULL plano added
            List<OrderItemInfo> fullPlanoOrderItems = allOrderItems.Where(oi => oi.PlanogramId != null
                                                                && oi.IsFullPlano.HasValue && oi.IsFullPlano.Value == true)
                                                                .ToList();

            // make each full plano and add to full planos collection
            if (fullPlanoOrderItems.Any())
            {
                var fullPlanoModels = new List<PlanogramModel>();

                // use the orderPlanograms link table relationship for the full planos only
                var orderPlanogramIds = fullPlanoOrderItems.OrderByDescending(oi => oi.DateAdded).Select(x => x.OrderPlanogramId).Distinct().ToList();

                foreach (var orderPlanogramId in orderPlanogramIds)
                {
                    var orderItems = fullPlanoOrderItems.Where(x => x.OrderPlanogramId == orderPlanogramId).OrderBy(x => x.PartName).ToList();
                    var planogramId = orderItems.First().PlanogramId;
                    var fullPlanoQuantity = orderItems.First().Quantity;
                    var fullPlanoInitialQuantity = orderItems.First().InitialQuantity;
                    if (planogramId == null) continue;
                    var plano = planogramService.GetPlanogram(planogramId.Value).Result;

                    var planoModel = new PlanogramModel
                    {
                        Name = plano.Name,
                        OrderItems = orderItems,
                        IsFullPlano = true,
                        PlanogramId = planogramId.Value,
                        ClusterName = plano.Cluster.Name,
                        Dimensions = $"{plano.Stand.Width}x{plano.Stand.Height}",
                        ClusterPartNumber = plano.Cluster.ClusterPartNumber,
                        Quantity = fullPlanoQuantity,
                        InitialQuantity = fullPlanoInitialQuantity,
                        OrderPlanogramId = orderPlanogramId,
                        StandName = plano.Stand.Name,
                        StandPartNumber = plano.Stand.StandAssemblyNumber
                    };

                    fullPlanoModels.Add(planoModel);

                }

                orderModel.FullPlanograms = fullPlanoModels;
            }

            // get the order items NOT associated with a full plano (should be backwards compatible with before we added the OrderPlanogram link relationship)
            List<OrderItemInfo> partialPlanoOrderItems = allOrderItems
                .Where(oi => oi.PlanogramId != null
                          && (!oi.IsFullPlano.HasValue || !oi.IsFullPlano.Value)).ToList();

            // make each partial plano and add to partial planos collection
            if (partialPlanoOrderItems.Any())
            {

                // here we do not use the orderplanograms link table relationship
                // get the plano IDs for the planograms which have been partially added
                // note that multiple partially added planograms will be combined into one single partially added planogram - separate from the fully added planograms
                var planoIds = partialPlanoOrderItems.OrderByDescending(x => x.DateAdded)
                    .Where(oi => oi.PlanogramId.HasValue)
                    .Select(oi => oi.PlanogramId.Value).Distinct();

                // get all the planos
                var planos = planoIds.Select(p => planogramService.GetPlanogram(p).Result);

                var planoModels = planos.Select(x => new PlanogramModel
                {
                    Name = x.Name,
                    OrderItems = partialPlanoOrderItems.Where(oi => oi.PlanogramId == x.Id),
                    IsFullPlano = false,
                    PlanogramId = x.Id,
                    ClusterName = x.Cluster.Name,
                    ClusterPartNumber = x.Cluster.ClusterPartNumber,
                    Dimensions = $"{x.Stand.Width}x{x.Stand.Height}"

                }).ToList();

                orderModel.PartialPlanograms = planoModels;
            }


            return orderModel;
        }


    }


}