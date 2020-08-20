using System.Runtime.Serialization;

namespace HelpMyStreet.Contracts.AddressService.Response
{
    [DataContract(Name = "postcodeWithNumberOfAddresses")]
    public class PostcodeWithNumberOfAddresses
    {
        [DataMember(Name = "pc")]
        public string Postcode { get; set; }

        [DataMember(Name = "ad")]
        public int NumberOfAddresses { get; set; }
    }
}
