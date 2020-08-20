using HelpMyStreet.Contracts.GroupService.Response;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace HelpMyStreet.Contracts.GroupService.Request
{
    public class PostAddUserToDefaultGroupsRequest : IRequest<PostAddUserToDefaultGroupsResponse>
    {
        [Required]
        public int UserID { get; set; }
    }
}
