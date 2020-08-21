using System;

namespace HelpMyStreet.Utils.Enums
{
    [Flags]
    public enum VolunteerType : byte
    {
        Helper = 1,
        StreetChampion = 2
    }
}
