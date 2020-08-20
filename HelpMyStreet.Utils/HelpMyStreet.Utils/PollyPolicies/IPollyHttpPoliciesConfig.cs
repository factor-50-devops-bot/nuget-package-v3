using System;

namespace HelpMyStreet.Utils.PollyPolicies
{
    public interface IPollyHttpPoliciesConfig
    {
        TimeSpan[] AzureFunctionNotStartedPauses { get; }
        TimeSpan[] ServiceErrorPauses { get; }
    }
}