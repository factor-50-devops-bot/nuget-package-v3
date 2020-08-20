using HelpMyStreet.Contracts.CommunicationService.Response;
using HelpMyStreet.Contracts.RequestService.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.CommunicationService.Request
{
    public class RequestCommunicationRequest : IRequest<RequestCommunicationResponse>
    {
        public CommunicationJob CommunicationJob { get; set; }
        public int? RecipientUserID { get; set; }
        public int? JobID { get; set; }
        public int? GroupID { get; set; }
        public Dictionary<string, string> AdditionalParameters { get; set; }
    }
}
