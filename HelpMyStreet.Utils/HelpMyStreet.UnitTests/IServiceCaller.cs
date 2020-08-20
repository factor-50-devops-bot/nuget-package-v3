using System.Net.Http;
using System.Threading.Tasks;

namespace HelpMyStreet.UnitTests
{
    public interface IServiceCaller
    {
        Task<HttpResponseMessage> GetAsync();
    }
}