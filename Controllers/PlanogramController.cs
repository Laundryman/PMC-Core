using System.Configuration;
using System.Net;
using dplo.Service;
using CoreSystem.Models;
using dplo.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using UserInfo = CoreSystem.Helpers.UserInfo;


namespace diam_planogram.Controllers
{
    [Authorize]
    public class PlanogramController : UmbracoApiController
  {

        #region constructor
        private IStandService _standService;
        private IPlanogramService _planogramService;
        private ICatalogueService _catalogueService;
        private ICountryService _countryService;
        private ICategoryService _categoryService;
        private IProductService _productService;


        public PlanogramController(
            IStandService standService,
            IPlanogramService planogramService,
            ICatalogueService catalogueService,
            ICountryService countryService,
            ICategoryService categoryService,
            IProductService productService
            )
        {
            _standService = standService;
            _planogramService = planogramService;
            _catalogueService = catalogueService;
            _countryService = countryService;
            _categoryService = categoryService;
            _productService = productService;
        }
        #endregion

        public PlanogramNotesModel GetPlanogramNotes(int planogramId)
        {

                //we will create a custom model
                var planogramNotesModel = new PlanogramNotesModel();
                planogramNotesModel.BrandId = int.Parse(ConfigurationManager.AppSettings["brand"]);
                var country = _countryService.GetCountry(UserInfo.DiamCountryId);

                //We're not using the country and region here: but we need to think about how we might regarding users.
                var notes = _planogramService.GetPlanogramNotes(UserInfo.Id, planogramNotesModel.BrandId, country.CountryId, default(int), planogramId).ToList();
                List<PNotesViewModel> pComments = new List<PNotesViewModel>();

                notes.Sort((x, y) => DateTime.Compare(y.NoteDate, x.NoteDate));
                foreach (PlanogramNote note in notes)
                {
                    PNotesViewModel inReplyTo;

                    PNotesViewModel newPv = (PNotesViewModel) note;
                    var repliedNoteID = note.NoteInReplyTo ?? default(long);
                    //if we have a replied to id then use it
                    if (repliedNoteID != 0)
                    {
                        inReplyTo = (PNotesViewModel)_planogramService.GetNote(repliedNoteID);
                        newPv.InReplyTo = inReplyTo;
                    }
                    pComments.Add(newPv);


                }

                planogramNotesModel.ApiUrl = ConfigurationManager.AppSettings["apiURL"];
                planogramNotesModel.Notes = pComments;
                return planogramNotesModel;

        }

        public IActionResult AddPlanogramNote([FromBody] NewNoteModel note)
        {
            var planogramId = note.PlanogramId;
     
                try
                {
                    var newPNote = new PlanogramNote();
                    newPNote.Note = note.Note;
                    newPNote.PlanogramId = note.PlanogramId;
                    newPNote.NoteDate = DateTime.Now;
                    newPNote.UserId = UserInfo.Id;;
                    newPNote.UserName = UserInfo.UserName;
                    newPNote.NoteTitle = UserInfo.UserName +
                                         String.Format("{0:d/M/yyyy HH:mm:ss}", newPNote.NoteDate);
                    _planogramService.CreatePlanogramNote(newPNote);
                    return Ok("Saved");
                }
                catch (Exception ex)
                {
                    return BadRequest("Failed");
                }

        }

        public IActionResult ReplyPlanogramNote([FromBody] NewNoteModel note)
        {
            var planogramId = note.PlanogramId;
            var noteId = note.ReplyNoteId;
            var inReplyTo = _planogramService.GetNote(noteId);
                try
                {
                    var newPNote = new PlanogramNote();
                    newPNote.Note = note.Note;
                    newPNote.PlanogramId = note.PlanogramId;
                    newPNote.NoteDate = DateTime.Now;
                    newPNote.NoteInReplyTo = noteId;
                    newPNote.InReplyTo = inReplyTo;
                    newPNote.UserId = UserInfo.Id;;
                    newPNote.NoteTitle = UserInfo.UserName +
                                         String.Format("{0:d/M/yyyy HH:mm:ss}", newPNote.NoteDate);
                    _planogramService.CreatePlanogramNote(newPNote);
                    return Ok("Saved");
                }
            catch (Exception ex)
                {
                    return BadRequest("Failed");
                }

        }

        public IActionResult DuplicatePlanogram([FromBody] DupPlanoModel dupPlano)
        {
            Planogram planogram = _planogramService.GetPlanogram(dupPlano.PlanogramId);
            if (dupPlano.NewPlanoName != planogram.Name)
            {
                _planogramService.ClonePlanogram(planogram.PlanogramId,dupPlano.NewPlanoName, UserInfo.userViewModel, dupPlano.IsUpdate);
            }

            return Ok("Saved");

        }

    }
}