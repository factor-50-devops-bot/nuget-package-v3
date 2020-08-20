using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HelpMyStreet.Contracts.AddressService.Response
{
    [DataContract(Name = "getNearbyPostcodesWithoutAddressesResponse")]
    public class GetNearbyPostcodesWithoutAddressesResponse
    {
        [DataMember(Name = "nearestPostcodes")]
        public IReadOnlyList<NearestPostcodeWithoutAddress> NearestPostcodes { get; set; }
    }
}
