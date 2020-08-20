using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.GroupService.Response
{
    public class TaskAction
    {
        public Dictionary<NewTaskAction, List<int>> TaskActions { get; set; }
    }
}
