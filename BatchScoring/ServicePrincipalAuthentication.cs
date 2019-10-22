using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;

namespace BatchScoring
{
    /// <summary>
    /// Class to retrieve a token for a service principal. 
    /// </summary>
    class ServicePrincipalAuthentication
    {
        private string Tenant {get; set;}
        private string ClientId { get; set; }
        private string ClientKey { get; set; }

        public ServicePrincipalAuthentication(string tenant, string clientId, string clientKey)
        {
            this.Tenant = tenant;
            this.ClientId = clientId;
            this.ClientKey = clientKey;
        }

        /// <summary>
        /// This code was taken from an MSDN blog post directly (with minor changes to include
        /// the class instances of Tenant, ClientId, and ClientKey
        /// </summary>
        /// <returns></returns>
        public async Task<String> GetToken()
        {
            string authContextURL = "https://login.windows.net/" + this.Tenant;
            var authenticationContext = new AuthenticationContext(authContextURL);
            var credential = new ClientCredential(this.ClientId, this.ClientKey);
            var result = await authenticationContext
                .AcquireTokenAsync("https://management.azure.com/", credential);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }
            string token = result.AccessToken;
            return token;
        }
    }
}
