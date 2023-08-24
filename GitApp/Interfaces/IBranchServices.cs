using GitApp.Data;

namespace GitApp.Interfaces
{
    public interface IBranchServices
    {
        Task<RepositoryData[]> GetRepositoriesAsync(string accessToken);
        Task<BranchInfo[]> GetRepositoryBranchesAsync(string accessToken, string owner, string repo);
        Task<int> GetBranchCommitCountAsync(string accessToken, string owner, string repo, string branch);
        Task<string> GetBranchBuildStatusAsync(string accessToken, string owner, string repo, string branch);
    }
}
