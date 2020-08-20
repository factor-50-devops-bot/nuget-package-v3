using System;

namespace HelpMyStreet.Utils.Dtos
{
    /// <summary>
    /// Dto for postcode coordinate data in Address.Postcode
    /// </summary>
    public class PostcodeDto : ILatitudeLongitude
    {
        public int Id { get; set; }
        public string Postcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}

