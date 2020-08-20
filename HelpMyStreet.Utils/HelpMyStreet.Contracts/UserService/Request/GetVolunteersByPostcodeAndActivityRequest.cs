using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Utils.Enums;
using MediatR;

namespace HelpMyStreet.Contracts.UserService.Request
{
    public class GetVolunteersByPostcodeAndActivityRequest : IRequest<GetVolunteersByPostcodeAndActivityResponse>
    {
        public VolunteerFilter VolunteerFilter { get; set; }
    }
}
