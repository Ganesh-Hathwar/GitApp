using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GitApp.Services
{
    public class TokenService
    {
        private readonly HttpClient _httpClient;

        private string _currentToken;

        public TokenService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _currentToken = "ghp_vLqbNq9YLBfOeGOEPfg3tT7ePJBZip4LrX2"; // Replace with your initial token
        }

        public async Task<string> GetTokenAsync()
        {
            if (await IsTokenExpiredAsync())
            {
                // Token expired, renew it and update the current token
                _currentToken = await RenewTokenAsync("your_new_personal_access_token"); // Replace with your new PAT
            }

            return _currentToken;
        }

        private async Task<bool> IsTokenExpiredAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _currentToken);

            var response = await _httpClient.SendAsync(request);

            return response.StatusCode == System.Net.HttpStatusCode.Unauthorized;
        }

        private async Task<string> RenewTokenAsync(string newPat)
        {
            // Implement token renewal logic here and return the renewed token
            // This can be similar to the token renewal logic in previous responses
            // Make sure to replace "your_new_personal_access_token" with your new PAT
            // You might need to make a POST request to GitHub's API to renew the token
            // and retrieve the renewed token from the response.
            // Return the renewed token to update _currentToken.
            // If renewal fails, return null or throw an exception as needed.

            return "renewed_token"; // Replace with the renewed token
        }
    }
}
