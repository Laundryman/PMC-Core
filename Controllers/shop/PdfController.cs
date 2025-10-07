using CoreSystem2024.Helpers;
using CoreSystem2024.Models.Shop;
using diam_planogram.Helpers;
using diam_planogram.Models.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PMApplication.Interfaces.ServiceInterfaces;
//using Microsoft.Graph.ExternalConnectors;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace CoreSystem2024.Controllers.shop
{
    public class PdfController : RenderController
    {

        #region Services, managers

        public IOrderService _orderService;
        public IPlanogramService _planogramService;
        public IConfiguration _config;

        public PdfController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, IOrderService orderService, IPlanogramService planogramService, IConfiguration config) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _orderService = orderService;
            _planogramService = planogramService;
            _config = config;
        }

        #endregion



        private async Task<OrderModel> BuildOrderConfirmationModel(int id)
        {
            var order = await _orderService.GetOrder(id);

            if (order == null) throw new ArgumentException("Order ID not found");

            // TODO: handle authentication
            var userId = UserInfo.Id; ;
            var isAdmin = RolesHelper.IsAdminUser(UserInfo.Roles);
            var isRegionalManager = RolesHelper.IsClientValidator(UserInfo.Roles);

            return OrderHelper.BuildFullOrder(order, _orderService, _planogramService, _config);

        }

        //[Route("pdf/order-confirmation/{id:int}/{pdf:bool}")]
        public async Task<IActionResult> OrderConfirmation(int id, bool? preview)
        {
            var orderModel = await BuildOrderConfirmationModel(id);

            var model = new PdfModel { Order = orderModel };

            if (preview.HasValue && preview.Value)
            {
                return CurrentTemplate(model);
            }

            var html2Pdf = new PdfHelper
            {
                MarginLeft = 0,
                MarginRight = 0,
                MarginBottom = 10,
                MarginTop = 5,
                PageSize = PdfHelper.PageSizes.A4,
                Dpi = 300
            };

            var bytes = html2Pdf.Print(model, PdfHelper.Template.OrderConfirmation);

            MemoryStream ms = new MemoryStream(bytes);

            // HttpContext.Response.AddHeader("Content-Disposition:attachment;", $"filename=order-confirmation-{id}.pdf");

            // Return the file as a PDF
            return File(ms, "application/pdf", $"order-confirmation-{id}.pdf");
        }

    }
}