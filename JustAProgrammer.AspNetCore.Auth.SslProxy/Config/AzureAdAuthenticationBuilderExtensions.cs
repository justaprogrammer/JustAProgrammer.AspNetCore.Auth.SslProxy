using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace JustAProgrammer.AspNetCore.Auth.SslProxy.Config
{
    public static class AzureAdAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder)
            => builder.AddAzureAd(_ => { });

        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder, Action<AzureAdOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, ConfigureAzureOptions>();
            builder.AddOpenIdConnect();
            return builder;
        }

        private class ConfigureAzureOptions : IConfigureNamedOptions<OpenIdConnectOptions>
        {
            private readonly ILogger<ConfigureAzureOptions> _logger;
            private readonly AzureAdOptions _azureOptions;

            public ConfigureAzureOptions(IOptions<AzureAdOptions> azureOptions, ILogger<ConfigureAzureOptions> logger)
            {
                _logger = logger;
                _azureOptions = azureOptions.Value;
            }

            public void Configure(string name, OpenIdConnectOptions options)
            {
                options.ClientId = _azureOptions.ClientId;
                options.Authority = $"{_azureOptions.Instance}{_azureOptions.TenantId}";
                options.UseTokenLifetime = true;
                options.CallbackPath = _azureOptions.CallbackPath;
                options.RequireHttpsMetadata = false;
                options.Events.OnMessageReceived = OnMessageReceived;
                options.Events.OnRedirectToIdentityProvider = OnRedirectToIdentityProvider;
            }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            private async Task OnMessageReceived(MessageReceivedContext context)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                _logger.LogInformation("Message Recieved");
            }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            private async Task OnRedirectToIdentityProvider(RedirectContext context)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                if (_azureOptions.ForceSslRedirectUri)
                {
                    _logger.LogTrace("Performing SSL Redirection of URLs.");
                    if (_azureOptions.ForceSslRedirectPort <= 0)
                    {
                        throw new InvalidOperationException("SSL Port must be a postive number!");
                    }

                    foreach (var header in context.HttpContext.Request.Headers)
                    {
                        _logger.LogDebug($"Header: {header.Key}:{string.Join(';', header.Value)}");
                    }

                    _logger.LogTrace($"Redirect URI: {context.Properties.RedirectUri}");
                    ForceSslRedirectUrls(context.ProtocolMessage, context.Properties);
                }
            }

            private void ForceSslRedirectUrls(OpenIdConnectMessage connectMessage, AuthenticationProperties authProperties)
            {
                var scope = new
                {
                    connectMessage.RedirectUri,
                    connectMessage.PostLogoutRedirectUri,
                    AzureAdOptions = _azureOptions
                };
                using (_logger.BeginScope(scope))
                {
                    if (_azureOptions.ForceSslRedirectPort == 443)
                    {
                        authProperties.RedirectUri = RewriteUrlToHttps(authProperties.RedirectUri);
                        connectMessage.RedirectUri = RewriteUrlToHttps(connectMessage.RedirectUri);
                        if (connectMessage.PostLogoutRedirectUri != null)
                        {
                            connectMessage.PostLogoutRedirectUri =
                                RewriteUrlToHttps(connectMessage.PostLogoutRedirectUri);
                        }
                    }
                    else
                    {
                        authProperties.RedirectUri = RewriteUrlToHttps(authProperties.RedirectUri,
                            _azureOptions.ForceSslRedirectPort);
                        connectMessage.RedirectUri = RewriteUrlToHttps(connectMessage.RedirectUri, _azureOptions.ForceSslRedirectPort);
                        if (connectMessage.PostLogoutRedirectUri != null)
                        {
                            connectMessage.PostLogoutRedirectUri =
                                RewriteUrlToHttps(connectMessage.PostLogoutRedirectUri, _azureOptions.ForceSslRedirectPort);
                        }
                    }
                }
            }

            private string RewriteUrlToHttps(string url)
            {
                if (url.StartsWith("http:"))
                {
                    url = $"https{url.Substring(4)}";
                }
                else
                {
                    _logger.LogWarning("Url {url} began with https.", url);
                }
                return url;
            }

            private string RewriteUrlToHttps(string url, int sslPort)
            {
                var uri = new Uri(url);
                return $"https://{uri.Host}:{sslPort}{uri.PathAndQuery}";
            }

            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }
        }
    }
}
