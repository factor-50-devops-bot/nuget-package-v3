using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Utils.Models
{
    public class RegistrationStepThree
    {
        public int UserID { get; set; }
        public List<SupportActivities> Activities { get; set; }
        public float? SupportRadiusMiles { get; set; }
        public bool? SupportVolunteersByPhone { get; set; }
        public bool? UnderlyingMedicalCondition { get; set; }

    }
}
