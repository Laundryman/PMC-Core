using diam_planogram.Models.Shop;
using System.Globalization;

namespace CoreSystem2024.Models.Shop
{
    public class PdfModel
    {
        public OrderModel Order { get; set; }
        public CultureInfo UnitedKingdom = CultureInfo.GetCultureInfo("en-GB");
    }
}