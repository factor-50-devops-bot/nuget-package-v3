using HelpMyStreet.Contracts.RequestService.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.RequestService.Request
{
    public class LogRequestRequest : IRequest<LogRequestResponse>
    {
        public string Postcode { get; set; }
    }
}
