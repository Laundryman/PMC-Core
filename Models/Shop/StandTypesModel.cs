using System.Collections.Generic;

namespace CoreSystem.Models.Shop
{
    public class StandTypesModel
    {
        public List<StandTypeModel> StandTypes { get; set; }
    }

    public class StandTypeModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}