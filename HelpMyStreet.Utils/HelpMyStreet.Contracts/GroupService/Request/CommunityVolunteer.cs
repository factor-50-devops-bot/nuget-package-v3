using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.GroupService.Request
{
    public class CommunityVolunteer
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public string Location { get; set; }
        public string ImageLocation { get; set; }
    }
}
