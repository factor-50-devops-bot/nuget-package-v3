using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Utils.Models
{
    public class RegistrationStepOne
    {
        public string FirebaseUID { get; set; }
        public DateTime DateCreated { get; set; }
        public string EmailAddress { get; set; }
        public int? ReferringGroupId { get; set; }
        public string Source { get; set; }
    }
}
