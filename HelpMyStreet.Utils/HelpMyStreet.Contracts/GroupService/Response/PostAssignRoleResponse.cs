using HelpMyStreet.Utils.Enums;
using MediatR;

namespace HelpMyStreet.Contracts.GroupService.Response
{
    public class PostAssignRoleResponse
    {
        public GroupPermissionOutcome Outcome { get; set; }
    }
}
