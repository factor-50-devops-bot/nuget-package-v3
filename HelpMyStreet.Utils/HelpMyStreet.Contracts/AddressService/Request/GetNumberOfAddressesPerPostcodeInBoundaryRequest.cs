using System.ComponentModel.DataAnnotations;
using HelpMyStreet.Contracts.AddressService.Response;
using MediatR;

namespace HelpMyStreet.Contracts.AddressService.Request
{
    public class GetNumberOfAddressesPerPostcodeInBoundaryRequest : IRequest<GetNumberOfAddressesPerPostcodeInBoundaryResponse>
    {
        [Required]
        [Range(-90, 90)]
        public double SwLatitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double SwLongitude { get; set; }

        [Required]
        [Range(-90, 90)]
        public double NeLatitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double NeLongitude { get; set; }
    }
}
