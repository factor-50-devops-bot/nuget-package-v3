using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.RequestService.Response
{
    public enum CommunicationJobTypes
    {
        SendWelcomeMessage = 1,
        SendRegistrationChasers = 2,
        SendNewTaskNotification = 3,
        SendOpenTaskDigest = 4,
        SendTaskStateChangeUpdate = 5,
        PostYotiCommunication = 6,
        SendTaskReminder = 7
    }
}
