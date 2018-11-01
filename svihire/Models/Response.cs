using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace svihire.Models
{
    public class Response : BaseEntity
    {
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
        public Guid QuestionId { get; set; }
        public Guid QuestionResponseId { get; set; }
        public Guid CandidateId { get; set; }
        public Candidate Respondent { get; set; }
        public Guid RecordId { get; set; }
        public string QuestionText { get; set; }
        public int ResponseScore { get; set; }
        public string SurveyResponseText { get; set; }
    }
}
