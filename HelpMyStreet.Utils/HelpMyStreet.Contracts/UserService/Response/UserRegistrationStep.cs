using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.UserService.Response
{
    public class UserRegistrationStep
    {
        public int RegistrationStep { get; set; }
        public int UserId { get; set; }
        public DateTime DateCompleted { get; set; }
    }
}
