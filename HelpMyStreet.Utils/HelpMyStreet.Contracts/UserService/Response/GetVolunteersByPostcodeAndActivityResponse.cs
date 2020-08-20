using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.UserService.Response
{
    public class GetVolunteersByPostcodeAndActivityResponse
    {
        public IEnumerable<VolunteerSummary> Volunteers { get; set; }
    }
}
