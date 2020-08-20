using System.Collections.Generic;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreet.Contracts.RequestService.Response
{
    public class GetJobsInProgressResponse
    {
        public List<JobSummary> JobSummaries { get; set; }
    }
}
