using System;

namespace HelpMyStreet.Utils.PollyPolicies
{
    public class PollyHttpPoliciesConfig : IPollyHttpPoliciesConfig
    {
        public TimeSpan[] AzureFunctionNotStartedPauses { get; }

        public TimeSpan[] ServiceErrorPauses { get; }


        public PollyHttpPoliciesConfig()
        {
            AzureFunctionNotStartedPauses = new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(40),
                TimeSpan.FromSeconds(60)
            };

            ServiceErrorPauses = new[]
            {
                TimeSpan.FromSeconds(0.1),
                TimeSpan.FromSeconds(0.25),
                TimeSpan.FromSeconds(0.5)
            };
        }
    }
}
