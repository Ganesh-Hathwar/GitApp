namespace GitApp.Interface
{
    public interface IGitHubStaleBranches
    {
        Task<List<string>> GetStaleBranches(string githubToken, string repositoryOwner, string repositoryName);
    }
}