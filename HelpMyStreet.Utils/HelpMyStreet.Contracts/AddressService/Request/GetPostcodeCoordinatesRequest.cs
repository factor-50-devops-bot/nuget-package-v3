using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HelpMyStreet.Contracts.AddressService.Response;
using MediatR;

namespace HelpMyStreet.Contracts.AddressService.Request
{
    public class GetPostcodeCoordinatesRequest : IRequest<GetPostcodeCoordinatesResponse>
    {
        [Required]
        public IEnumerable<string> Postcodes { get; set; }
    }
}
