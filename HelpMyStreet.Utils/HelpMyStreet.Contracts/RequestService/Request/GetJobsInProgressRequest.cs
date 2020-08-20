using MediatR;
using HelpMyStreet.Contracts.RequestService.Response;

namespace HelpMyStreet.Contracts.RequestService.Request
{
    public class GetJobsInProgressRequest : IRequest<GetJobsInProgressResponse>
    {
    }
}
