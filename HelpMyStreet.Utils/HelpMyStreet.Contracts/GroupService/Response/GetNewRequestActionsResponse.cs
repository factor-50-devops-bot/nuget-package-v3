using System.Collections.Generic;

namespace HelpMyStreet.Contracts.GroupService.Response
{
    public class GetNewRequestActionsResponse
    {
        public Dictionary<int,TaskAction> Actions { get; set; }
    }
}