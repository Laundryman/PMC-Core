using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dplo.Domain;
using dplo.Domain.Entities;

namespace CoreSystem.Models
{
  public class PNotesViewModel
  {
      [Key]
      public Int64 NoteId { get; set; }
      public string Note { get; set; }
      public string UserId { get; set; }
      public string UserName { get; set; }
      public int LegacyUserId { get; set; }
      public String NoteDate { get; set; }
      public Int64? NoteInReplyTo { get; set; }
      public string NoteTitle { get; set; }
      public int PlanogramId { get; set; }

      //public virtual ICollection<PNotesViewModel> RelatedNotes { get; set; }
      public PNotesViewModel InReplyTo;
      //[ForeignKey("PlanogramId")]
      //public virtual Planogram Planogram { get; set; }
      //[ForeignKey("UserId")]
      //public virtual UserProfileSimpleModel User { get; set; }


      public static explicit operator PNotesViewModel(PlanogramNote note)
      {
          PNotesViewModel pv = new PNotesViewModel();
          //var user = new UserProfileSimpleModel
          //{
          //    UserId = note.User.UserId,
          //    UserName = note.User.UserName,
          //    Email = note.User.Email,
          //    FirstName = note.User.FirstName,
          //    LastName = note.User.LastName,
          //    CountryName = note.User.Country.Name,
          //    CountryId = note.User.CountryId
          //};

          pv.NoteId = note.NoteId;
          pv.Note = note.Note;
          pv.UserId = note.UserId;
          pv.UserName = note.UserName;
          pv.LegacyUserId = note.LegacyUserId;
          pv.NoteDate = String.Format("{0:G}", note.NoteDate);
          pv.NoteInReplyTo = note.NoteInReplyTo;
          pv.NoteTitle = note.NoteTitle;
          pv.PlanogramId = note.PlanogramId;
          //pv.User = user;
          //pv.InReplyTo = note.InReplyTo;
          return pv;
      }


  }
}
