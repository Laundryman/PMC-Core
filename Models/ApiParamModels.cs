namespace CoreSystem2024.Models
{
    public enum ReportTypes
    {
        SkuList = 1,
        CassetteList = 2
    }

    public class GetReportParams
    {
        public int PlanogramId { get; set; }
        public int ReportType { get; set; }
        public int RegionId { get; set; }
        public int CountryId { get; set; }
        public int StandTypeId { get; set; }
    }

    public class ArchivePlanogramData
    {
        public int PlanogramId { get; set; }
        public string JobNumber { get; set; }
    }

    public class GetPlanoParams
    {
        public int Status { get; set; }
        public string JobCode { get; set; }
        public int RegionId { get; set; }
        public int CountryId { get; set; }
        public int StandTypeId { get; set; }
    }

    public class GetMenuParams
    {
        public int BrandId { get; set; }
        public int StandTypeId { get; set; }
        public int CountryId { get; set; }
        public int planogramId { get; set; }
        public string category { get; set; }
    }

    public class PlanogramUpdate
    {
        public string PlanogramName { get; set; }
        public int PlanogramId { get; set; }
    }
}