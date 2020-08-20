using MediatR;
using HelpMyStreet.Contracts.RequestService.Response;
using System.ComponentModel.DataAnnotations;

namespace HelpMyStreet.Contracts.RequestService.Request
{
    public class GetJobStatusHistoryRequest : IRequest<GetJobStatusHistoryResponse>
    {
        [Required]
        public int JobID { get; set; }
    }
}
