using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Utils.Models
{
    public class RegistrationStepTwo
    {
        public int UserID { get; set; }
        public string PostalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address { get; set; }
        public string MobilePhone { get; set; }
        public string OtherPhone { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Defaulted to FirstName following step 2
        public string DisplayName { get; set; }
    }
}
