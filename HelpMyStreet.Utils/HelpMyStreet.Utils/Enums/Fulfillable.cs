using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.RequestService.Response
{
    public enum Fulfillable
    {
        Rejected_InvalidPostcode = 1,
        Rejected_Unfulfillable = 2,
        Rejected_Error = 3,
        Accepted_PassToStreetChampion = 4,
        Accepted_PassToVolunteer = 5,
        Accepted_ManualReferral = 6,
        Accepted_DiyRequest = 7
    }
}
