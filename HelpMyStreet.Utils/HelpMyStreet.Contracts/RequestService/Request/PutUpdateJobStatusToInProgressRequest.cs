using HelpMyStreet.Contracts.RequestService.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.RequestService.Request
{
    public class PutUpdateJobStatusToInProgressRequest : IRequest<PutUpdateJobStatusToInProgressResponse>
    {
        public int JobID { get; set; }
        public int CreatedByUserID { get; set; }
        public int VolunteerUserID { get; set; }
    }
}
