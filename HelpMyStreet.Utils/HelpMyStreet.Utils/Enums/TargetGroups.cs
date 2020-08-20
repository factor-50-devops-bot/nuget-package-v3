using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Utils.Enums
{
    public enum TargetGroups
    {
        ThisGroup = 0,
        ThisGroupAndChildren = 1,
        ParentGroup = 2,
        SiblingsAndParentGroup = 3,
        GenericGroup = 4,
    }
}
