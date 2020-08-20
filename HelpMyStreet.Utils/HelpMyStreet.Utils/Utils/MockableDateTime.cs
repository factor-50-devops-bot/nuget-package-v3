using System;
using Microsoft.Extensions.Internal;

namespace HelpMyStreet.Utils.Utils
{
    public class MockableDateTime : ISystemClock
    {
        public DateTimeOffset UtcNow => DateTime.UtcNow;
    }
}
