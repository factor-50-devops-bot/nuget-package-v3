using System.Collections.Generic;

namespace HelpMyStreet.Contracts.GroupService.Response
{
    public class GetUserRolesResponse
    {
        public Dictionary<int,List<int>> UserGroupRoles { get; set; }
    }
}
