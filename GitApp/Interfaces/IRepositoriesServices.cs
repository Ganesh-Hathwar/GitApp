using GitApp.Data;

namespace GitApp.Interfaces
{
    public interface IRepositoriesServices
    {
        Task<List<RepositoryData>> GetRepositoriesAsync(string accessToken);
    }
}
