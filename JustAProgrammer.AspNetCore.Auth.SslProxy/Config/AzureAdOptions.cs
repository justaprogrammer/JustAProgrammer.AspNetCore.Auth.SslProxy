namespace JustAProgrammer.AspNetCore.Auth.SslProxy.Config
{
    public class AzureAdOptions
    {
        public string ClientId { get; set; }
        
        public string ClientSecret { get; set; }

        public string Instance { get; set; } = "https://login.microsoftonline.com/";
        
        public string Domain { get; set; }
        
        public string TenantId { get; set; }

        public string CallbackPath { get; set; } = "/signin-oidc";

        /// <summary>
        /// Gets or sets whether to force the OAuth Redirects to be SSL.
        /// </summary>
        public bool ForceSslRedirectUri { get; set; }

        /// <summary>
        /// Gets or Sets the port to force the ssl redirect port to be on.
        /// </summary>
        public int ForceSslRedirectPort { get; set; } = 443;
    }
}
