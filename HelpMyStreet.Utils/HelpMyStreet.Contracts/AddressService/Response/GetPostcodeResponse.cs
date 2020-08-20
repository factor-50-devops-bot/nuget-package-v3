using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HelpMyStreet.Contracts.AddressService.Response
{
    [DataContract(Name = "postcode")]
    public class GetPostcodeResponse
    {
        [DataMember(Name = "postcode")]
        public string Postcode { get; set; }
        
        [DataMember(Name = "friendlyName")]
        public string FriendlyName { get; set; }

        [DataMember(Name = "addressDetails")]
        public IReadOnlyList<AddressDetailsResponse> AddressDetails { get; set; }

    }
}
