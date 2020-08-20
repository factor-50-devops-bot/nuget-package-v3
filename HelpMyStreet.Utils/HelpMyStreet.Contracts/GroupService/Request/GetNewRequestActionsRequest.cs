using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreet.Utils.Models;
using MediatR;

namespace HelpMyStreet.Contracts.GroupService.Request
{
    public class GetNewRequestActionsRequest :IRequest<GetNewRequestActionsResponse>
    {
        public HelpRequest HelpRequest { get; set; }
        public NewJobsRequest NewJobsRequest { get; set; }
    }
}
