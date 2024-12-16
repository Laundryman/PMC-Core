using dplo.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreSystem2024.Models
{
    public class PlanogramNotesModel
    {
        public PlanogramNotesModel()
        {
            this.Notes = new HashSet<PNotesViewModel>();
        }

        public string ApiUrl { get; set; }
        public int CountryId { get; set; }
        public int BrandId { get; set; }
        public int PlanogramId { get; set; }

        public virtual ICollection<PNotesViewModel> Notes { get; set; }
        [ForeignKey("PlanogramId")]
        public virtual Planogram Planogram { get; set; }




    }
}
