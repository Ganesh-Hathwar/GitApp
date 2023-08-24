using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using GitApp.Data;
using GitApp.Interfaces;
using System.Globalization;

namespace GitApp.Services
{
    public class RepositoriesServices : IRepositoriesServices
    {
        private readonly HttpClient _httpClient;
        public RepositoriesServices(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
        }
        public async Task<List<RepositoryData>> GetRepositoriesAsync(string accessToken)
        {
            SetAuthorizationHeader(accessToken);
            var response = await _httpClient.GetAsync("https://api.github.com/user/repos");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<RepositoryData>>();
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return null;
            }
        }
        public async Task<int> GetRepositoryBranchCountAsync(string accessToken, string owner, string repo)
        {
            SetAuthorizationHeader(accessToken);
            var branches = await _httpClient.GetFromJsonAsync<BranchInfo[]>($"https://api.github.com/repos/{owner}/{repo}/branches");
            return branches.Length;
        }

        public async Task<TeamInfo> GetTeamInfoAsync(string accessToken, string owner, string repo)
        {
            SetAuthorizationHeader(accessToken);
            var response = await _httpClient.GetAsync($"https://api.github.com/repos/{owner}/{repo}/teams");

            if (response.IsSuccessStatusCode)
            {
                var teams = await response.Content.ReadFromJsonAsync<TeamInfo[]>();

                if (teams.Length > 0)
                {
                    return teams[0];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return null;
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
