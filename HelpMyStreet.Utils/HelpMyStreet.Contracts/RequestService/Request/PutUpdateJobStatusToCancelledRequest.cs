using HelpMyStreet.Contracts.RequestService.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.RequestService.Request
{
    public class PutUpdateJobStatusToCancelledRequest : IRequest<PutUpdateJobStatusToCancelledResponse>
    {
        public int JobID { get; set; }
        public int CreatedByUserID { get; set; }
    }
}
