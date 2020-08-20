using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.UserService.Response
{
    public class GetIncompleteRegistrationStatusResponse
    {
        public List<UserRegistrationStep> Users { get; set; }
    }
}
