using GitApp.Data;
using GitApp.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitApp.Services
{
    public class BranchesServices : IBranchServices
    {
        private readonly HttpClient _httpClient;

        public BranchesServices(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
        }
            
        public async Task<RepositoryData[]> GetRepositoriesAsync(string accessToken)
        {
            SetAuthorizationHeader(accessToken);
            return await _httpClient.GetFromJsonAsync<RepositoryData[]>("https://api.github.com/user/repos");
        }

        public async Task<BranchInfo[]> GetRepositoryBranchesAsync(string accessToken, string owner, string repo)
        {
            SetAuthorizationHeader(accessToken);
            return await _httpClient.GetFromJsonAsync<BranchInfo[]>($"https://api.github.com/repos/{owner}/{repo}/branches");
        }

        public async Task<int> GetBranchCommitCountAsync(string accessToken, string owner, string repo, string branch)
        {
            SetAuthorizationHeader(accessToken);
            var commits = await _httpClient.GetFromJsonAsync<Commit[]>($"https://api.github.com/repos/{owner}/{repo}/commits?sha={branch}");
            return commits.Length;
        }

        public async Task<string> GetBranchBuildStatusAsync(string accessToken, string owner, string repo, string branch)
        {
            SetAuthorizationHeader(accessToken);
            var query = @"
            {
                repository(owner: """ + owner + @""", name: """ + repo + @""") {
                    ref(qualifiedName: """ + "refs/heads/" + branch + @""") {
                        associatedPullRequests(first: 1) {
                            nodes {
                                commits(last: 1) {
                                    nodes {
                                        commit {
                                            checkSuites(first: 1) {
                                                nodes {
                                                    status
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }";

            var requestContent = new StringContent("{\"query\":\"" + query + "\"}", Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api.github.com/graphql", requestContent);
            var responseData = await response.Content.ReadAsStringAsync();

            var responseObject = JsonSerializer.Deserialize<GitHubGraphQLResponse>(responseData);

            if (responseObject?.data?.repository?.refField?.associatedPullRequests?.nodes != null &&
                responseObject.data.repository.refField.associatedPullRequests.nodes.Length > 0)
            {
                var buildStatus = responseObject.data.repository.refField.associatedPullRequests.nodes[0]
                    .commits.nodes[0].commit.checkSuites.nodes[0].status;

                return buildStatus;
            }
            else
            {
                return "No build status available";
            }
        }

        private void SetAuthorizationHeader(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
        }
    }
}
