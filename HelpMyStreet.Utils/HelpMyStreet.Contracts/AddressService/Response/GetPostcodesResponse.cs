using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HelpMyStreet.Contracts.AddressService.Response
{
    public class GetPostcodesResponse
    {
        public Dictionary<string,GetPostcodeResponse> PostcodesResponse { get; set; }
    }
}
