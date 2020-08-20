using System;
using System.Collections.Generic;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreet.Contracts.RequestService.Response
{
    public class GetJobStatusHistoryResponse
    {
        public List<StatusHistory> History { get; set; }
    }

    public class StatusHistory
    {
        public DateTime StatusDate { get; set; }
        public JobStatuses JobStatus { get; set; }
    }

}
