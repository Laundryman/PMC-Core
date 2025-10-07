namespace diam_planogram.Models.Shop
{
    public class PartModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int PartTypeId { get; set; }
        public int Facings { get; set; }
        public string PartNumber { get; set; }
        public string AltPartNumber { get; set; }
        public string CustomerRefNo { get; set; }
        public string ManufacturingProcess { get; set; }
        public string Presentation { get; set; }
        public string TestingType { get; set; }
        public int Stock { get; set; }
        public string Dimensions { get; set; }
        public decimal UnitPrice { get; set; }
        public string Description { get; set; }
        public string PackShotImageSrc { get; set; }
        public decimal UnitCost { get; set; }
        public decimal? LaunchPrice { get; set; }
        public DateTime? LaunchDate { get; set; }
        public string CassetteBio { get; set; }
        public int ParentCategoryId { get; set; }
        public decimal CurrentPrice { get; set; }
        public int? StandTypeId { get; set; }
        public bool DmiReco { get; set; }
        public bool HidePrices { get; set; }

    }
}