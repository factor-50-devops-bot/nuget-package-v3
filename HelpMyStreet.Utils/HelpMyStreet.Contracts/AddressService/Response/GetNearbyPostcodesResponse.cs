using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HelpMyStreet.Contracts.AddressService.Response
{
    [DataContract(Name = "getNearbyPostcodesResponse")]
    public class GetNearbyPostcodesResponse
    {
        [DataMember(Name = "postcodes")]
        public IReadOnlyList<GetNearbyPostCodeResponse> Postcodes { get; set; } = new List<GetNearbyPostCodeResponse>();
    }
}
