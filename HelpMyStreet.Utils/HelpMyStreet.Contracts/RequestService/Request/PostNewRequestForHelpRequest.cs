using HelpMyStreet.Contracts.RequestService.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreet.Contracts.RequestService.Request
{
    public class PostNewRequestForHelpRequest : IRequest<PostNewRequestForHelpResponse>
    {
        public HelpRequest HelpRequest { get; set; }
        public NewJobsRequest NewJobsRequest { get; set; }  
    }
}
