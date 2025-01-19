using AutoMapper;
using CoreSystem2024.Controllers.shop;
using CoreSystem2024.Helpers;
using CoreSystem2024.Models;
using dplo.Domain.Entities;
using dplo.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dplo.ViewModels;
using Microsoft.Extensions.Configuration;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Web.Common.Controllers;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using UserInfo = CoreSystem2024.Helpers.UserInfo;

/// NEED TO MOVE THI CALLS TO THE API VIA PROXY
namespace diam_planogram.Controllers
{
    [Authorize]
    public class PlanogramController : BaseApiController
    {

        #region constructor

        private IPlanogramService _planogramService;
        private ICountryService _countryService;
        private readonly IMemberManager _memberManager;
        private IConfiguration _config;
        private readonly IMapper _mapper;


        public PlanogramController(ICountryService countryService, IPlanogramService planogramService, IMemberManager memberManager, IConfiguration config, IMapper mapper) : base(config)
        {
            _planogramService = planogramService;
            _countryService = countryService;
            _memberManager = memberManager;
            _config = config;
            _mapper = mapper;
        }
        #endregion

        [HttpGet]
        [Route("/Api/Planogram/GetPlanogramNotes")]
        public async Task<PlanogramNotesModel> GetPlanogramNotes(int planogramId)
        {

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            //var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            //we will create a custom model
            var planogramNotesModel = new PlanogramNotesModel();
            planogramNotesModel.BrandId = int.Parse(_config["AppSettings:ClientBrandId"] ?? "0");
            var country = _countryService.GetCountry(userInfo.DiamCountryId);

            //We're not using the country and region here: but we need to think about how we might regarding users.
            var notes = _planogramService.GetPlanogramNotes(userInfo.Id, planogramNotesModel.BrandId, country.CountryId, default(int), planogramId).ToList();
            //List<PNotesViewModel> pComments = new List<PNotesViewModel>();

            notes.Sort((x, y) => DateTime.Compare(y.NoteDate, x.NoteDate));
            //foreach (PlanogramNote note in notes)
            //{
            //    PNotesViewModel inReplyTo;

            //    PNotesViewModel newPv = (PNotesViewModel)note;
            //    var repliedNoteID = note.NoteInReplyTo ?? default(long);
            //    //if we have a replied to id then use it
            //    if (repliedNoteID != 0)
            //    {
            //        inReplyTo = (PNotesViewModel)_planogramService.GetNote(repliedNoteID);
            //        newPv.InReplyTo = inReplyTo;
            //        pComments.Add(newPv);
            //    }
            var pComments = _mapper.Map<List<PNotesViewModel>>(notes);

            //}

            planogramNotesModel.ApiUrl = _config["AppSettings:ApiIdentifier"] ?? String.Empty;
            planogramNotesModel.Notes = pComments;
            return planogramNotesModel;

        }

        [HttpPost]
        [Route("/Api/Planogram/AddPlanogramNote")]
        public async Task<IActionResult> AddPlanogramNote([FromBody] NewNoteModel note)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var planogramId = note.PlanogramId;

            try
            {
                var newPNote = new PlanogramNote();
                newPNote.Note = note.Note;
                newPNote.PlanogramId = note.PlanogramId;
                newPNote.NoteDate = DateTime.Now;
                newPNote.UserId = userInfo.Id; ;
                newPNote.UserName = userInfo.UserName;
                newPNote.NoteTitle = userInfo.UserName +
                                     String.Format("{0:d/M/yyyy HH:mm:ss}", newPNote.NoteDate);
                _planogramService.CreatePlanogramNote(newPNote);
                return Ok("Saved");
            }
            catch (Exception ex)
            {
                return BadRequest("Failed");
            }

        }
        [HttpPost]
        [Route("/Api/Planogram/ReplyPlanogramNote")]
        public async Task<IActionResult> ReplyPlanogramNote([FromBody] NewNoteModel note)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
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
                newPNote.UserId = userInfo.Id; ;
                newPNote.UserName = userInfo.UserName;
                newPNote.NoteTitle = userInfo.UserName +
                                     String.Format("{0:d/M/yyyy HH:mm:ss}", newPNote.NoteDate);
                _planogramService.CreatePlanogramNote(newPNote);
                return Ok("Saved");
            }
            catch (Exception ex)
            {
                return BadRequest("Failed");
            }

        }
        [HttpPost]
        [Route("/Api/Planogram/DuplicatePlanogram")]
        public async Task<IActionResult> DuplicatePlanogram([FromBody] DupPlanoModel dupPlano)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            UserViewModel userInfo = AuthHelper.GetUserInfo(memberIdentity);

            Planogram planogram = _planogramService.GetPlanogram(dupPlano.PlanogramId);
            if (dupPlano.NewPlanoName != planogram.Name)
            {
                _planogramService.ClonePlanogram(planogram.PlanogramId, dupPlano.NewPlanoName, userInfo, dupPlano.IsUpdate);
            }

            return Ok("Saved");

        }

    }
}