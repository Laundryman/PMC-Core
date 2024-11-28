using System.Globalization;
using diam_planogram.Models.Shop;

namespace CoreSystem.Models.Shop
{
    public class PdfModel
    {
        public OrderModel Order { get; set; }
        public CultureInfo UnitedKingdom = CultureInfo.GetCultureInfo("en-GB");
    }
}