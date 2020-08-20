using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.UserService.Response
{
    public class GetUsersResponse
    {
        public IEnumerable<UserDetails> UserDetails { get; set; }
    }
}
