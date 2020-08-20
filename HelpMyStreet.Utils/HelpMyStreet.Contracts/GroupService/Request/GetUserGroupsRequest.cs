using HelpMyStreet.Contracts.GroupService.Response;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace HelpMyStreet.Contracts.GroupService.Request
{
    public class GetUserGroupsRequest : IRequest<GetUserGroupsResponse>
    {
        [Required]
        public int? UserID { get; set; }
    }
}
