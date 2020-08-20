using HelpMyStreet.Contracts.UserService.Response;
using MediatR;

namespace HelpMyStreet.Contracts.UserService.Request
{
    public class GetIncompleteRegistrationStatusRequest : IRequest<GetIncompleteRegistrationStatusResponse>
    {
    }
}
