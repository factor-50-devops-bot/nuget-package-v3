using HelpMyStreet.Utils.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.GroupService.Response
{
    public class GetChildGroupsResponse
    {
        public List<Group> ChildGroups { get; set; }
    }
}
