using System.Runtime.Serialization;

namespace HelpMyStreet.Contracts.AddressService.Response
{
    [DataContract(Name = "postcodeCoordinate")]
    public class PostcodeCoordinate
    {
        [DataMember(Name = "pc")] // short names to reduce serialisation and network transfer time
        public string Postcode { get; set; }

        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lng")]
        public double Longitude { get; set; }
    }
}
