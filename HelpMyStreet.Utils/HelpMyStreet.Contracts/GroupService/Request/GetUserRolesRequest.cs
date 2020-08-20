using HelpMyStreet.Contracts.GroupService.Response;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace HelpMyStreet.Contracts.GroupService.Request
{
    public class GetUserRolesRequest : IRequest<GetUserRolesResponse>
    {
        [Required]
        public int? UserID { get; set; }
    }
}
