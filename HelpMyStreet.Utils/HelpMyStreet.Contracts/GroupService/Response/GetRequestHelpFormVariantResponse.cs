using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.GroupService.Response
{
    public class GetRequestHelpFormVariantResponse
    {
        public RequestHelpFormVariant RequestHelpFormVariant { get; set; }
        public TargetGroups TargetGroups { get; set; }
    }
}
