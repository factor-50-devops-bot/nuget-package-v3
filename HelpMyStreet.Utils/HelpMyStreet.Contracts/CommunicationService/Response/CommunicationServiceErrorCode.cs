using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.CommunicationService.Response
{
    public enum CommunicationServiceErrorCode
    {
        InternalServerError = 1,
        EmailProviderDown = 2,
        NoUserFound = 3,
        ValidationError = 4
    }
}
