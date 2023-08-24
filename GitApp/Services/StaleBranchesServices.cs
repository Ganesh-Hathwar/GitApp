using GitApp.Data;
using GitApp.Interface;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;


public class StaleBranchesServices : IGitHubStaleBranches
{
    private readonly HttpClient _httpClient;
    //private IHttpClientFactory @object;

    public StaleBranchesServices(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    //public StaleBranchesServices(IHttpClientFactory @object)
    //{
    //    this.@object = @object;
    //}

    public async Task<List<string>> GetStaleBranches(string githubToken, string repositoryOwner, string repositoryName)
    {
        var staleBranches = new List<string>();

        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", githubToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("YourApp", "1.0"));

            var branchesApiUrl = $"https://api.github.com/repos/{repositoryOwner}/{repositoryName}/branches";
            var response = await _httpClient.GetStringAsync(branchesApiUrl);

            var branches = JsonSerializer.Deserialize<List<Branch>>(response);

            var now = DateTime.Now;
            var staleThreshold = TimeSpan.FromDays(5);
            var tasks = branches.Select(async branch =>
            {
                if (branch.Commit != null && !string.IsNullOrEmpty(branch.Commit.Sha))
                {
                    var commitApiUrl = $"https://api.github.com/repos/{repositoryOwner}/{repositoryName}/commits/{branch.Commit.Sha}";
                    var commitResponse = await _httpClient.GetStringAsync(commitApiUrl);

                    if (!string.IsNullOrEmpty(commitResponse))
                    {
                        var commitInfo = JsonSerializer.Deserialize<CommitInfo>(commitResponse);

                        if (commitInfo?.Committer != null && !string.IsNullOrEmpty(commitInfo.Committer.Date))
                        {
                            var lastCommitDate = DateTime.Parse(commitInfo.Committer.Date);

                            if (now - lastCommitDate > staleThreshold)
                            {
                                return branch.Name;
                            }
                        }
                    }
                }

                return null;
            });

            staleBranches = (await Task.WhenAll(tasks)).Where(branch => branch != null).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return staleBranches;
    }
}
