using HelpMyStreet.Contracts.GroupService.Response;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace HelpMyStreet.Contracts.GroupService.Request
{
    public class PostRevokeRoleRequest : IRequest<PostRevokeRoleResponse>
    {
        [Required]
        public int? GroupID { get; set; }

        [Required]
        public int? UserID { get; set; }

        [Required]
        public RoleRequest Role { get; set; }

        [Required]
        public int? AuthorisedByUserID { get; set; }
    }
}
