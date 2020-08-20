using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Utils.Models
{
    public class User
    {
        // Generated in database
        public int ID { get; set; }


        // Supplied in registration step 1
        public string FirebaseUID { get; set; }
        public DateTime DateCreated { get; set; }


        // Supplied in registration step 2
        public string PostalCode { get; set; }


        // Supplied in registration step 3
        public List<SupportActivities> SupportActivities { get; set; }
        public float? SupportRadiusMiles { get; set; }
        public bool? SupportVolunteersByPhone { get; set; }

        // Automatically set to true following registration step 3
        public bool? IsVolunteer { get; set; }
        public bool? EmailSharingConsent { get; set; }
        public bool? MobileSharingConsent { get; set; }
        public bool? OtherPhoneSharingConsent { get; set; }
        public bool? HMSContactConsent { get; set; }


        // Supplied in registration step 4
        public bool? StreetChampionRoleUnderstood { get; set; }
        public List<string> ChampionPostcodes { get; set; }


        // Supplied in registration step 5
        public bool? IsVerified { get; set; }


        public UserPersonalDetails UserPersonalDetails { get; set; }
        public Dictionary<int, DateTime> RegistrationHistory { get; set; }

        public int? ReferringGroupId { get; set; }
        public string Source { get; set; }
    }
}
