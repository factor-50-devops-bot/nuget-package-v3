using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Utils.Models
{
    public class Job
    {
        public int JobID { get; set; }
        public JobStatuses JobStatus { get; set; }
        public int? VolunteerUserID { get; set; }
        public SupportActivities SupportActivity { get; set; }
        public List<Question> Questions { get; set; }
        public string Details { get; set; }
        public int DueDays { get; set; }
        public bool HealthCritical { get; set; }
    }
}
