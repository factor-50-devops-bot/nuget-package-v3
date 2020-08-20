using System.Runtime.Serialization;

namespace HelpMyStreet.Contracts.AddressService.Response
{
    [DataContract(Name = "nearestPostcode")]
    public class NearestPostcodeWithoutAddress
    {
        [DataMember(Name = "postcode")]
        public string Postcode { get; set; }

        [DataMember(Name = "distanceInMetres")]
        public int DistanceInMetres { get; set; }

    }
}
