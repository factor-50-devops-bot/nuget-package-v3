using System;
using System.Collections.Generic;
using System.Text;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreet.Contracts.RequestService.Response
{
    public class GetJobDetailsResponse
    {
        public int JobID { get; set; }
        public string PostCode { get; set; }
        public bool ForRequestor { get; set; }
        public string Details { get; set; }
        public RequestPersonalDetails Requestor { get; set; }
        public RequestPersonalDetails Recipient { get; set; }
        public string SpecialCommunicationNeeds { get; set; }
        public string OtherDetails { get; set; }
        public bool ConsentForContact { get; set; }
        public JobStatuses JobStatus { get; set; }
        public int? VolunteerUserID { get; set; }
        public SupportActivities SupportActivity { get; set; }        
        public DateTime DueDate { get; set; }
        public bool HealthCritical { get; set; }
        public DateTime DateRequested { get; set; }  
        public RequestorType RequestorType { get; set; }
        public string OrganisationName { get; set; }
        public DateTime DateStatusLastChanged { get; set; }
        public int DueDays { get; set; }
    }
}
