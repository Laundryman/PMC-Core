using CoreSystem2024.Helpers;
using CoreSystem2024.Models.Shop.Json;
using CoreSystemII.Config;
using diam_planogram.Helpers;
using diam_planogram.Models.Shop;
using dplo.Domain.Entities;
using dplo.Helpers;
using dplo.Service;
using dplo_shop.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Security;
using RolesHelper = CoreSystem2024.Helpers.RolesHelper;

namespace CoreSystem2024.Controllers.shop
{
    public class OrdersApiController : BaseApiController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMemberManager _memberManager;
        private readonly IOrderService _orderService;
        private readonly IPlanogramService _planogramService;
        private readonly IConfiguration _config;
        private readonly ILogger<OrdersApiController> _logger;
        private readonly ICountryService _countryService;
        private readonly ICatalogueService _catalogueService;
        private readonly EmailHelper _emailHelper;


        #region orders
        //[Route("api/orders")]
        public OrdersApiController(
            ICountryService countryService,
            IOrderService orderService,
            ILogger<OrdersApiController> logger,
            IMemberManager memberManager,
            IWebHostEnvironment webHostEnvironment, 
            IConfiguration config, 
            IPlanogramService planogramService, ICatalogueService catalogueService, EmailHelper emailHelper) : base(config)
        {
            _logger = logger;
            _countryService = countryService;
            _memberManager = memberManager;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
            _orderService = orderService;
            _planogramService = planogramService;
            _catalogueService = catalogueService;
            _emailHelper = emailHelper;
        }
        [Route("/umbraco/api/ordersapi/getorders")]

        public async Task<IActionResult> GetOrders()
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var userCountry = _countryService.GetCountry(userInfo.DiamCountryId);
            RolesHelper.Initialize(_config);

            List<OrderInfo> orders;

            if (RolesHelper.IsAdminUser(userInfo.Roles)) // admin - get all
            {
                orders = _orderService.GetFilteredOrders(BrandId, null, null, "OrderUpdated", "desc", null, null, null,
                    null).ToList();
            }
            else if (RolesHelper.IsClientValidator(userInfo.Roles)) // regional manager
            {
                var region = userCountry.Regions.FirstOrDefault(x => x.BrandId == BrandId);

                if (region == null)
                    throw new Exception("Failed to look up region for this user's country/brandId: " +
                                        userInfo.DiamCountryId + " / " + BrandId);

                orders = _orderService.GetFilteredOrders(BrandId, null, null, "OrderUpdated", "desc", null, null,
                    null, region.RegionId).ToList();
            }
            else // normal user
            {
                orders = _orderService.GetFilteredOrders(BrandId, null, null, "OrderUpdated", "desc", null, null,
                    userCountry.CountryId, null).ToList();
            }

            var orderModels = orders.Select(x => new OrderModel
            {
                OrderStatus = x.OrderStatus,
                OrderId = x.OrderId,
                OrderTitle = x.OrderTitle,
                TotalQuantity = x.TotalResults,
                OrderCreated = x.OrderCreated,
                OrderUpdated = x.OrderUpdated
            }).ToList();

            var ordersInProgress = orderModels.Where(o => o.OrderStatus == (int)OrderStatusEnum.Open);
            var ordersSubmitted = orderModels.Where(o => o.OrderStatus == (int)OrderStatusEnum.Submitted
                                                         || o.OrderStatus == (int)OrderStatusEnum.Received);
            var ordersCompleted = orderModels.Where(o => o.OrderStatus == (int)OrderStatusEnum.Approved);
            // var ordersReceived = orders.Where(o => o.OrderStatus == (int)OrderStatusEnum.Received);
            // var ordersCancelled = orders.Where(o => o.OrderStatus == (int)OrderStatusEnum.Cancelled);


            var model = new OrderListModel
            {
                OrdersInProgress = ordersInProgress,
                OrdersSubmitted = ordersSubmitted,
                OrdersCompleted = ordersCompleted,
                // OrdersReceived = ordersReceived,
                //  OrdersCancelled = ordersCancelled
            };

            var response = new ApiResponseModel { data = model };

            return Ok(response);
        }


        //[Route("api/orders/getactive")]
        [Route("/umbraco/api/ordersapi/GetActiveOrder")]

        public async Task<IActionResult> GetActiveOrder()
        {



            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var aOrderId = AuthHelper.GetActiveOrderId(HttpContext, User);

            var order = _orderService.GetOrder(aOrderId);

            if (order == null || order.OrderStatus != (int)OrderStatusEnum.Open)
            {
                AuthHelper.SetActiveOrderId(HttpContext.Request, 0);
                var response = new ApiResponseModel { data = null };
                return Ok(response);
            }
            else
            {
                var activeOrder = new ActiveOrder
                {
                    ActiveOrderId = order.OrderId.ToString(),
                    Title = order.OrderTitle
                };

                var response = new ApiResponseModel { data = activeOrder };
                return Ok(response);

            }
        }


        //[Route("api/orders/setactive/{id:int}")]
        [Route("/umbraco/api/ordersapi/SetActiveOrder/{id}")]
        public IActionResult SetActiveOrder(int id)
        {


            var aOrderId = AuthHelper.SetActiveOrderId(HttpContext.Request, id);

            var order = _orderService.GetOrder(aOrderId);
            var activeOrder = new ActiveOrder
            {
                ActiveOrderId = order.OrderId.ToString(),
                Title = order.OrderTitle
            };

            var response = new ApiResponseModel { data = activeOrder };
            return Ok(response);
        }

        [HttpPost]
        [Route("/umbraco/api/ordersapi/RenameOrder")]
        public async Task<IActionResult> RenameOrder(RenameOrderModel model)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            if (RolesHelper.IsAdminShopper(userInfo.Roles))
            {
                var order = _orderService.GetOrder(model.OrderId);

                order.OrderTitle = model.OrderTitle;

                _orderService.SaveOrder();

                var orderModel = OrderHelper.BuildFullOrder(order, _orderService, _planogramService);

                var response = new ApiResponseModel { data = orderModel };

                return Ok(response);

                return Ok(response);
            }
            else
            {
                throw new ArgumentException("This function is only available to admin shoppers.");
            }


            //try
            //{

            //}
            //catch (Exception ex)
            //{
            //    var response = new ApiResponseModel {error = new ErrorResponse(ex.Message)};
            //    return Ok(response);
            //}
        }



        [HttpPost]
        //[Route("api/order/create/{orderTitle}")]
        [Route("/umbraco/api/ordersapi/CreateOrder")]
        public async Task<IActionResult> CreateOrder(CreateOrderModel model)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var userCountry = _countryService.GetCountry(userInfo.DiamCountryId);
            if (!RolesHelper.IsAdminShopper(userInfo.Roles))
            {
                throw new UnauthorizedAccessException("Only admin shoppers can create orders.");
            }

            if (string.IsNullOrWhiteSpace(model.OrderTitle))
                throw new ArgumentNullException(nameof(model.OrderTitle));

            try
            {
                var order = new Order
                {
                    OrderTitle = model.OrderTitle,
                    BrandId = BrandId, 
                    OrderCreatedBy = userInfo.Id,
                    OrderUpdatedBy = userInfo.Id,
                    OrderCreated = DateTime.Now,
                    OrderUpdated = DateTime.Now,
                    OrderStatus = 1,
                    CountryId = userCountry.CountryId,
                    OrderCreatedByName = userInfo.GivenName + " " + userInfo.Surname,
                    OrderUpdatedByName = userInfo.GivenName + " " + userInfo.Surname,
                    RegionId = userCountry.Regions.First(x => x.BrandId == BrandId).RegionId
                };

                _orderService.CreateOrder(order);
                var url = new System.Uri(Request.GetDisplayUrl());
                //LogHelper.LogAction((int)LogActionEnum.CreateOrder,
                //    url.GetLeftPart(UriPartial.Authority), 0, order.OrderId);
                var response = new ApiResponseModel { data = order };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        //[Route("api/order/{id:int}/{planoView:bool}")]
        [Route("/umbraco/api/ordersapi/GetOrder")]
        public IActionResult GetOrder(int id, bool planoView)
        {




            var response = new ApiResponseModel();

            var order = _orderService.GetOrder(id);

            if (order == null)
            {
                response.data = string.Empty;
            }
            else
            {
                // TODO: this is currently the same data returned for both views
                if (planoView)
                {
                    var orderModel = OrderHelper.BuildFullOrder(order, _orderService, _planogramService);

                    response.data = orderModel;
                }
                else
                {
                    var orderModel = OrderHelper.BuildFullOrder(order, _orderService, _planogramService);

                    response.data = orderModel;
                }
            }

            return Ok(response);

        }


        //[Route("api/orders/submit/{id:int}")]
        [HttpPost]
        [Route("/umbraco/api/ordersapi/SubmitOrder")]
        //[MvcAuthorize]
        public async Task<IActionResult> SubmitOrder(int id)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            // TODO: check the current user has rights to submit this order

            var order = _orderService.GetOrder(id);

            if (order.OrderStatus != (int)OrderStatusEnum.Open)
                throw new ArgumentException("Only open orders can be submitted.");

            if (order.OrderItems == null || order.OrderItems.Count == 0)
                throw new ArgumentException("This order has no items.");

            var orderItemInfos = _orderService.GetOrderItemInfos(order.OrderId).ToList();

            foreach (var orderItem in order.OrderItems)
            {
                var orderItemInfo = orderItemInfos.FirstOrDefault(x => x.OrderItemId == orderItem.OrderItemId);
                if (orderItemInfo == null)
                    throw new ArgumentException("There was an error with order item id: " + orderItem.OrderItemId +
                                                ", part name: " + orderItem.PartName);

                orderItem.Price = orderItemInfo.Price;
            }

            order.OrderStatus = (int)OrderStatusEnum.Submitted;
            order.OrderUpdated = DateTime.Now;
            order.OrderUpdatedBy = userInfo.Id; ;
            order.OrderUpdatedByName = userInfo.GivenName + " " + userInfo.Surname;
            order.OrderSubmitted = DateTime.Now;

            _orderService.SaveOrder();
            var url = new System.Uri(Request.GetDisplayUrl());
            //LogHelper.LogAction((int)LogActionEnum.SubmitOrder,
            //    url.GetLeftPart(UriPartial.Authority), 0, order.OrderId);

            var response = new ApiResponseModel { data = order };


            var model = new ApiResponseModel { data = (int)OrderStatusEnum.Submitted };
            try
            {
                _logger.LogDebug("attempting to send emails");
                if (DiamEmailConfiguration.GetConfig().EmailEnabled)
                { await SendSubmittedEmails(id); }
            }
            catch (Exception ex)
            {
                _logger.LogDebug("Email Failed");

                throw new Exception("Thank you, your order was submitted but failed to send confirmation email. (" + ex.Message + ")");
            }

            return Ok(model);
        }

        //[Route("api/orders/unsubmit/{id:int}")]
        [HttpPost]
        //[MvcAuthorize]
        [Route("/umbraco/api/ordersapi/UnsubmitOrder")]
        public async Task<IActionResult> UnsubmitOrder(int id)
        {

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var order = _orderService.GetOrder(id);

            if (order.OrderStatus != (int)OrderStatusEnum.Submitted)
                throw new ArgumentException("This order cannot be unsubmitted because it is marked as " + order.OrderStatus);

            order.OrderStatus = (int)OrderStatusEnum.Open;
            order.OrderUpdated = DateTime.Now;
            order.OrderUpdatedBy = userInfo.Id; ;
            order.OrderUpdatedByName = userInfo.GivenName + " " + userInfo.Surname;

            _orderService.SaveOrder();

            var model = new ApiResponseModel { data = (int)OrderStatusEnum.Open };

            return Ok(model);
        }




        [Route("/umbraco/api/ordersapi/SendSubmittedEmails")]
        private async Task<Email> SendSubmittedEmails(int orderId)
        {
            _logger.LogDebug("SendSubmittedEmails begin");
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            string sessionName = DiamConfiguration.GetConfig().SessionName;
            var diamSession = HttpContext.Request.Cookies[sessionName];

            if (diamSession == null)
            {
                _logger.LogDebug("diamSession = null");
                throw new Exception("diamSession cookie not found");
            }

            _logger.LogDebug("Get accessToken");
            //var accessToken = AuthHelper.ReAuth(Authorization, WebServerClient);

            _logger.LogDebug("got AccessToken");

            // need to include the xls in this email - or a link to it
            _logger.LogDebug("Get userId");
            var userId = userInfo.Id;
            _logger.LogDebug("Got userId");

            var uri = new System.Uri(Request.GetDisplayUrl());
            string domainURI = uri.Scheme + "://" + uri.Authority;
            //_logger.LogDebug("Get exportedOrderUrl with " + userId);
            var exportedOrderUrl = CreateOrderExportLink(orderId, userId, domainURI);
            _logger.LogDebug("begin admin email");
            var adminEmail = new Email()
            {
                BccList = DiamEmailConfiguration.GetConfig().BCCList,
                DateSent = DateTime.Now,
                EmailTrigger = (int)EmailTrigger.AdminOrderSubmitted,
                FromAddress = DiamEmailConfiguration.GetConfig().FromAddress,
                OrderId = orderId,
                ToAddress = DiamEmailConfiguration.GetConfig().ToAddress,
                RecipientName = DiamEmailConfiguration.GetConfig().RecipientName,
                //UserId = userId,
                EmailEnabled = DiamEmailConfiguration.GetConfig().EmailEnabled,
                EmailSubject = "Order Submitted",
                SuppliedEmailBodyContent = System.Uri.EscapeDataString(exportedOrderUrl)
            };


            //var emailHelper = new EmailHelper(_logger);
            var adminResponse = await _emailHelper.SendEmail(adminEmail);
            _logger.LogDebug("End Admin Email");


            var userEmail = new Email()
            {
                //BccList = DiamEmailConfiguration.GetConfig().BCCList,
                DateSent = DateTime.Now,
                EmailTrigger = (int)EmailTrigger.UserOrderSubmitted,
                FromAddress = DiamEmailConfiguration.GetConfig().FromAddress,
                OrderId = orderId,
                ToAddress = userInfo.Email,
                RecipientName = userInfo.GivenName + " " + userInfo.Surname,
                UserId = userInfo.Id,
                EmailEnabled = DiamEmailConfiguration.GetConfig().EmailEnabled,
                EmailSubject = "Order Submitted"
            };

            var userResponse = await _emailHelper.SendEmail(userEmail);
            _logger.LogDebug("End User Email");

            List<string> errors = new List<string>();

            if (!adminResponse.EmailSendSuccess.HasValue || !adminResponse.EmailSendSuccess.Value)
            {
                errors.Add("admin");
            }
            if (!userResponse.EmailSendSuccess.HasValue || !userResponse.EmailSendSuccess.Value)
            {
                errors.Add("user");
            }

            if (errors.Count > 0)
            { throw new Exception(string.Join(",", errors)); }

            return userResponse;
        }


        //[Route("api/orders/remove/{id:int}")]
        [HttpPost]
        [Route("/umbraco/api/ordersapi/RemoveOrder")]
        public async Task<IActionResult> RemoveOrder(int id)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            // TODO: check the current user has rights to remove this order
            var userId = userInfo.Id;

            var order = _orderService.GetOrder(id);

            if (order.OrderStatus != (int)OrderStatusEnum.Open)
            {
                throw new ArgumentException("Only open orders can be removed.");
            }

            order.OrderStatus = (int)OrderStatusEnum.Cancelled;
            _orderService.SaveOrder();

            var model = new ApiResponseModel { data = (int)OrderStatusEnum.Cancelled };

            return Ok(model);

        }


        #endregion
        #region orderItems
        [HttpPost]
        [Route("/umbraco/api/ordersapi/AddOrderItem")]
        public async Task<IActionResult> AddOrderItem(AddOrderItemModel model)
        {

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            if (model.OrderId == null)
            {
                var activeOrderId = AuthHelper.GetActiveOrderId(HttpContext, User);

                if (activeOrderId == 0)
                    throw new ArgumentException("There is no active order.");

                model.OrderId = activeOrderId;
            }

            var response = new ApiResponseModel();

            var part = _catalogueService.GetPart(model.PartId);
            var order = _orderService.GetOrder(model.OrderId.Value);

            if (order.OrderStatus != (int)OrderStatusEnum.Open)
                throw new ArgumentException("This order is not currently open.");


            if (part == null)
            {
                response.data = string.Empty;
            }
            else
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    DateAdded = DateTime.Now,
                    PartId = model.PartId,
                    PartName = part.Name,
                    PartNumber = part.PartNumber,
                    //  Price = itemCost, // I don't think we should even store this until its submitted
                    Quantity = model.Quantity,
                    PlanogramId = null
                };

                _orderService.CreateOrderItem(orderItem);
                order.OrderUpdated = DateTime.Now;
                order.OrderUpdatedBy = userInfo.Id; ;
                order.OrderUpdatedByName = userInfo.GivenName + " " + userInfo.Surname;

                _orderService.SaveOrder();
                //Log Action
                var url = new System.Uri(Request.GetDisplayUrl());
                //LogHelper.LogAction((int)LogActionEnum.EditOrder,
                //    url.GetLeftPart(UriPartial.Authority), 0, order.OrderId);

                var orderModel = OrderHelper.BuildFullOrder(order, _orderService, _planogramService);

                response.data = orderModel;

            }

            return Ok(response);

        }

        [HttpPost]
        //[Route("api/orders/deleteOrderItem/{orderId:int}/{orderItemId:int}")]
        [Route("/umbraco/api/ordersapi/DeleteOrderItem")]
        public async Task<IActionResult> DeleteOrderItem(DeleteOrderItemModel model)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var order = _orderService.GetOrder(model.OrderId);

            var orderItem = order.OrderItems.FirstOrDefault(x => x.OrderId == model.OrderItemId);


            // if (!RolesHelper.IsAdminShopper(Helpers.UserInfo.Roles) && orderItem?.PlanogramId != null)
            //     throw new ArgumentException("This function is only available to admin shoppers.");


            if (RolesHelper.IsAdminShopper(userInfo.Roles) || orderItem?.PlanogramId == null)
            {
                var response = new ApiResponseModel();


                if (order.OrderStatus != (int)OrderStatusEnum.Open)
                    throw new ArgumentException("This order is not currently open.");

                _orderService.DeleteOrderItem(model.OrderItemId);
                //Log Action
                var url = new System.Uri(Request.GetDisplayUrl());
                //LogHelper.LogAction((int)LogActionEnum.EditOrder,
                //    url.GetLeftPart(UriPartial.Authority), 0, order.OrderId);

                order.OrderUpdated = DateTime.Now;
                order.OrderUpdatedBy = userInfo.Id; ;
                order.OrderUpdatedByName = userInfo.GivenName + " " + userInfo.Surname;

                var orderModel = OrderHelper.BuildFullOrder(order, _orderService, _planogramService);

                response.data = orderModel;

                return Ok(response);
            }
            else
            {
                throw new ArgumentException("This function is only available to admin shoppers.");
            }
        }

        [HttpPost]
        //[Route("api/orders/deletePlanogram/{orderId:int}/{planogramId:int}")]
        [Route("/umbraco/api/ordersapi/DeletePlanogram")]
        public async Task<IActionResult> DeletePlanogram(DeletePlanogramModel model)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            if (!RolesHelper.IsAdminShopper(Helpers.UserInfo.Roles))
                throw new ArgumentException("This function is only available to admin shoppers.");

            var response = new ApiResponseModel();
            var order = _orderService.GetOrder(model.OrderId);

            if (order.OrderStatus != (int)OrderStatusEnum.Open)
                throw new ArgumentException("This order is not currently open.");

            var orderPlanogram =
                order.OrderPlanograms?.FirstOrDefault(
                    op => op.OrderId == model.OrderId && op.PlanogramId == model.PlanogramId);

            // this bit is because of the changing requirements meaning we have two fashions of storing orderitems against planograms (legacy protection)
            if (orderPlanogram != null)
                _orderService.DeleteFullPlanogram(model.OrderId, orderPlanogram.OrderPlanogramId);
            else
                _orderService.DeletePlanogram(model.OrderId, model.PlanogramId);


            order.OrderUpdated = DateTime.Now;
            order.OrderUpdatedBy = userInfo.Id; ;
            order.OrderUpdatedByName = userInfo.GivenName + " " + userInfo.Surname;


            _orderService.SaveOrder();

            var orderModel = OrderHelper.BuildFullOrder(order, _orderService, _planogramService);

            response.data = orderModel;


            return Ok(response);

        }



        [HttpPost]
        [Route("/umbraco/api/ordersapi/DeleteFullPlanogram")]
        public async Task<IActionResult> DeleteFullPlanogram(DeleteFullPlanogramModel model)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            if (!RolesHelper.IsAdminShopper(Helpers.UserInfo.Roles))
                throw new ArgumentException("This function is only available to admin shoppers.");

            var response = new ApiResponseModel();
            var order = _orderService.GetOrder(model.OrderId);

            if (order.OrderStatus != (int)OrderStatusEnum.Open)
                throw new ArgumentException("This order is not currently open.");


            _orderService.DeleteFullPlanogram(model.OrderId, model.OrderPlanogramId);

            order.OrderUpdated = DateTime.Now;
            order.OrderUpdatedBy = userInfo.Id; ;
            order.OrderUpdatedByName = userInfo.GivenName + " " + userInfo.Surname;

            _orderService.SaveOrder();

            var orderModel = OrderHelper.BuildFullOrder(order, _orderService, _planogramService);

            response.data = orderModel;


            return Ok(response);

        }

        [HttpPost]
        //[Route("api/orders/updateOrderItemQuantity/{orderItemId:int}/{quantity:int}")]
        [Route("/umbraco/api/ordersapi/UpdateOrderItemQuantity")]
        public async Task<IActionResult> UpdateOrderItemQuantity(UpdateOrderItemQuantityModel model)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var response = new ApiResponseModel();

            var orderItem = _orderService.GetOrderItem(model.OrderItemId);

            if (orderItem == null)
            {
                throw new ArgumentException("Order item could not be found");
            }

            var order = _orderService.GetOrder(orderItem.OrderId);

            if (order.OrderStatus != (int)OrderStatusEnum.Open)
                throw new ArgumentException("This order is not currently open.");


            if (model.Quantity < orderItem.InitialQuantity)
            {
                if (!RolesHelper.IsAdminShopper(userInfo.Roles))
                    throw new ArgumentException("Minimum quantity for items in this planogram is " + orderItem.InitialQuantity);

                orderItem.InitialQuantity = model.Quantity;
            }



            orderItem.Quantity = model.Quantity;
            _orderService.SaveOrderItem();

            order.OrderUpdated = DateTime.Now;
            order.OrderUpdatedBy = userInfo.Id; ;
            order.OrderUpdatedByName = userInfo.GivenName + " " + userInfo.Surname;

            _orderService.SaveOrder();

            var orderModel = OrderHelper.BuildFullOrder(order, _orderService, _planogramService);

            response.data = orderModel;

            return Ok(response);


        }




        [HttpPost]
        //[Route("api/orders/updateOrderItemQuantity/{orderItemId:int}/{quantity:int}")]
        [Route("/umbraco/api/ordersapi/UpdateOrderPlanogramQuantity")]
        public async Task<IActionResult> UpdateOrderPlanogramQuantity(UpdateOrderPlanogramQuantityModel model)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var response = new ApiResponseModel();

            //var orderItem = OrderService.GetOrderItem(model.OrderItemId);

            var order = _orderService.GetOrder(model.OrderId);
            var orderItems = order.OrderItems.Where(oi => oi.OrderPlanogramId == model.OrderPlanogramId).ToList();

            if (!orderItems.Any())
            {
                throw new ArgumentException("This planogram has no items.");
            }


            if (order.OrderStatus != (int)OrderStatusEnum.Open)
                throw new ArgumentException("This order is not currently open.");

            var initialQuantity = orderItems.First().InitialQuantity;

            if (model.Quantity < initialQuantity)
            {
                if (!RolesHelper.IsAdminShopper(userInfo.Roles))
                    throw new ArgumentException("Minimum quantity for items in this planogram is " + initialQuantity);
            }


            foreach (var orderItem in orderItems)
            {
                orderItem.Quantity = model.Quantity;

                if (orderItem.InitialQuantity < model.Quantity)
                    orderItem.InitialQuantity = model.Quantity;
            }

            //OrderService.SaveOrderItem();

            order.OrderUpdated = DateTime.Now;
            order.OrderUpdatedBy = userInfo.Id; ;
            order.OrderUpdatedByName = userInfo.GivenName + " " + userInfo.Surname;

            _orderService.SaveOrder();

            var orderModel = OrderHelper.BuildFullOrder(order, _orderService, _planogramService);

            response.data = orderModel;

            return Ok(response);


        }


        #endregion





        private string CreateOrderExportLink(int orderId, string userId, string domainURI)
        {
            _logger.LogDebug("CreateOrderExportLink start");
            Order order = _orderService.GetOrder(orderId);
            // we can retrieve the userId from the request
            //var userProfile = OauthService.GetUserProfile(userId);

            try
            {
                string fileName = string.Format("order_{0}_{1}.xls", userId.ToString(), order.OrderTitle);
                string webRootPath = _webHostEnvironment.WebRootPath;
                string contentRootPath = _webHostEnvironment.ContentRootPath;
                var downloadsPath = Path.Combine(contentRootPath + "~/user_downloads/" + userId.ToString() + "/");
                var footerPath = Path.Combine(contentRootPath + ("~/user_downloads/" + userId.ToString() + "/"));

                if (!Directory.Exists(downloadsPath))
                {
                    Directory.CreateDirectory(downloadsPath);
                }
                string path = downloadsPath;
                string wPath = domainURI + "/user_downloads/" + userId.ToString() + "/";
                string webPath = $"{wPath}{fileName}";
                string filePath = $"{path}\\{fileName}";

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                var exportManager = new ExportManager(_planogramService, _orderService);
                exportManager.ExportOrderToXls(filePath, order);

                _logger.LogDebug("CreateOrderExportLink End " + webPath);

                return webPath;
            }
            catch (Exception Ex)
            {
                _logger.LogDebug("CreateOrderExportLink fail " + Ex.Message);
                //IActionResult message = new IActionResult(HttpStatusCode.BadRequest);

                // Get stack trace for the exception with source file information
                //var st = new StackTrace(ex, true);
                // Get the top stack frame
                //var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                //var line = frame.GetFileLineNumber();

                throw;
            }

        }


    }
}