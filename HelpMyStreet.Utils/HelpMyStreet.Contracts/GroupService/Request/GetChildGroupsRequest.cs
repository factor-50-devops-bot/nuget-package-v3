using HelpMyStreet.Contracts.GroupService.Response;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace HelpMyStreet.Contracts.GroupService.Request
{
    public class GetChildGroupsRequest : IRequest<GetChildGroupsResponse>
    {
        [Required]
        public int? GroupID { get; set; }
    }

}
