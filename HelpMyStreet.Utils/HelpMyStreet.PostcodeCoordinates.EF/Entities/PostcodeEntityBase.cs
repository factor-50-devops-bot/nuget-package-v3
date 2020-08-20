using System;

namespace HelpMyStreet.PostcodeCoordinates.EF.Entities
{
    /// <summary>
    /// Provides the properties for PostcodeEntity, PostcodeSwitchEntity and PostcodeOldEntity.  PostcodeSwitchEntity and PostcodeOldEntity are used when postcode coordinates are loaded and need to be identical to PostcodeEntity due to use of a switch statement.
    /// </summary>
    public abstract class PostcodeEntityBase 
    {
        public int Id { get; set; }
        public string Postcode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
