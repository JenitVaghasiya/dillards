using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using svihire.Models;
using svihire.Contexts;

namespace svihire.Controllers
{
    public class SurveyController : Controller
    {
        // GET: Survey
        public ActionResult Index(TakeSurveyViewModel model)
        {
            if (model == null)
            {
                return RedirectToAction("../Account/Login");
            }
            else
            {
                return View(model);
            }

        }

        public ActionResult TakeSurvey(Guid id)
        {
            Invite invite = new Invite();
            using (var dbInvite = new InviteContext())
            {
                invite = dbInvite._Invites.Find(id);
                if (invite != null)
                {
                    var existingData = dbInvite.CheckIfCandidateHasCompletedByID(invite.SurveyId, invite.CandidateId);
                    if (existingData != null && existingData.Count > 0)
                    {
                        //check survey && candidate to see if they have taken it. If they have, send them to a page
                        return View("SurveyOnFile");
                    }
                    else
                    {
                        using (var dbSurvey = new SurveyContext(invite.AccountId))
                        {
                            //build our ViewModel
                            var survey = dbSurvey._Surveys.Find(invite.SurveyId);
                            var model = new TakeSurveyViewModel();
                            model.Candidate = dbSurvey.GetCandidateByInviteID(invite.Id);
                            //model.surveyItems = dbInvite.LoadSurveyItems(invite.SurveyId);

                            var questions = dbSurvey.Questions
                            .Where(e => e.SurveyId == invite.SurveyId)
                            .Select(q => new SurveyQuestionViewModel()
                            {
                                Id = q.Id,
                                Question = q.QuestionText,
                                PossibleResponses = dbSurvey.QuestionResponses
                                   .OrderBy(e => e.Order)
                                   .Where(e => e.ResponseSetId == q.ResponseSetId)
                                   .Select(r => new SurveyResponseViewModel { Id = r.Id, Text = r.ResponseText })
                                   .ToList()
                            });

                            model = new TakeSurveyViewModel()
                            {
                                SurveyTitle = survey.SurveyName,
                                SurveyId = survey.Id,
                                QuestionsViewModel = questions.ToList()
                            };

                            if (model.Candidate != null && !String.IsNullOrEmpty(model.SurveyTitle) && model.QuestionsViewModel.Count > 0)
                            {
                                return View("Index", model);
                            }
                            else
                            {
                                return View(); //default - say that's not a valid invite
                            }
                        }
                        
                    }

                }
            }
            return View(); //default - say that's not a valid invite
        }
    }
}