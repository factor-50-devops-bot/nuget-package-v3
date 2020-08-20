using System.Threading.Tasks;

public interface ITestDataGetter
{
    Task<string> GetDataAsync();
    string GetData();
}