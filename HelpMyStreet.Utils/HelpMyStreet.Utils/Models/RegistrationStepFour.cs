using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Utils.Models
{
    public class RegistrationStepFour
    {
        public int UserID { get; set; }
        public bool? StreetChampionRoleUnderstood { get; set; }
        public List<string> ChampionPostcodes { get; set; }

    }
}
