using HelpMyStreet.Contracts.AddressService.Response;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace HelpMyStreet.Contracts.AddressService.Request
{
    public class GetPostcodeRequest : IRequest<GetPostcodeResponse>
    {
        [Required]
        public string Postcode { get; set; }
    }
}
