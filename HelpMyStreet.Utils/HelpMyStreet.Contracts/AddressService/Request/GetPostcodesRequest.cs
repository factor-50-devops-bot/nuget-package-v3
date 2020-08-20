using HelpMyStreet.Contracts.AddressService.Response;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace HelpMyStreet.Contracts.AddressService.Request
{
    public class GetPostcodesRequest : IRequest<GetPostcodesResponse>
    {
        [Required]
        public PostcodeList PostcodeList { get; set; }

        [Required]
        public bool IncludeAddressDetails { get; set; }
    }
}
