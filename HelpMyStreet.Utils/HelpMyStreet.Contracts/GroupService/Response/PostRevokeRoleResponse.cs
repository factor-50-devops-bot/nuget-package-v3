using HelpMyStreet.Utils.Enums;
using MediatR;

namespace HelpMyStreet.Contracts.GroupService.Response
{
    public class PostRevokeRoleResponse
    {
        public GroupPermissionOutcome Outcome { get; set; }
    }
}
