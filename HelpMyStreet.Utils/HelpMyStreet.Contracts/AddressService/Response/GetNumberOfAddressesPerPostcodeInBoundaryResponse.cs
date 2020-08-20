using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HelpMyStreet.Contracts.AddressService.Response
{
    [DataContract(Name = "getNumberOfAddressesInBoundaryResponse")]
    public class GetNumberOfAddressesPerPostcodeInBoundaryResponse
    {
        [DataMember(Name = "postcodesWithNumberOfAddresses")]
        public IReadOnlyList<PostcodeWithNumberOfAddresses> PostcodesWithNumberOfAddresses { get; set; }
    }
}
