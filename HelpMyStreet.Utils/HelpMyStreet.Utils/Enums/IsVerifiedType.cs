using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Utils.Enums
{
    [Flags]
    public enum IsVerifiedType : byte
    {
        IsVerified = 1,
        IsNotVerified = 2,
        All = 3
    }
}
