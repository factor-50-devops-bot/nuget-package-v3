using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;

namespace HelpMyStreet.Utils.Models
{
    public class JobSummary
    {
        public int JobID { get; set; }
        public JobStatuses JobStatus { get; set; }
        public int? VolunteerUserID { get; set; }
        public SupportActivities SupportActivity { get; set; }
        public string Details { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsHealthCritical { get; set; }
        public string PostCode { get; set; }
        public double DistanceInMiles { get; set; }
        public string SpecialCommunicationNeeds { get; set; }
        public string OtherDetails { get; set; }
        public List<Question> Questions { get; set; }
        public int? ReferringGroupID { get; set; }
        public List<int> Groups { get; set; }
        public string RecipientOrganisation { get; set; }
        public DateTime DateStatusLastChanged { get; set; }
        public int DueDays { get; set; }
    }
}
