using HelpMyStreet.Contracts.GroupService.Response;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace HelpMyStreet.Contracts.GroupService.Request
{
    public class GetGroupByKeyRequest : IRequest<GetGroupByKeyResponse>
    {
        [Required]
        public string GroupKey { get; set; }
    }

}
