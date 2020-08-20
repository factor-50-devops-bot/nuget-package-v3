using HelpMyStreet.Contracts.GroupService.Response;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace HelpMyStreet.Contracts.GroupService.Request
{
    public class GetGroupMembersRequest : IRequest<GetGroupMembersResponse>
    {
        [Required]
        public int? GroupID { get; set; }
    }
}
