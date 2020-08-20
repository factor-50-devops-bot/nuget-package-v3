using HelpMyStreet.Contracts.RequestService.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.RequestService.Request
{
    public class PutUpdateJobStatusToDoneRequest : IRequest<PutUpdateJobStatusToDoneResponse>
    {
        public int JobID { get; set; }
        public int CreatedByUserID { get; set; }
    }
}
