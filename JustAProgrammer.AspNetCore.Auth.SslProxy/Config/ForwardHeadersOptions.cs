using System.Collections.Generic;

namespace JustAProgrammer.AspNetCore.Auth.SslProxy.Config
{
    /// <summary>
    /// Options for configuring <see cref="Microsoft.AspNetCore.Builder.ForwardedHeadersOptions"/>
    /// </summary>
    public class ProxyOptions
    {
        /// <summary>
        /// Gets or sets whether or not to allow all headers.
        /// </summary>
        /// <remarks>Bad security practice to do this.</remarks>
        /// <seealso cref="Microsoft.AspNetCore.Builder.ForwardedHeadersOptions.KnownNetworks"/>
        bool AllIPs { get; set; } = false;

        /// <summary>
        /// Get or sets hostnames of IPs whose Forward headers will be honored. 
        /// </summary>
        /// <remarks>This won't deal with round robin DNS, etc.</remarks>
        /// <seealso cref="Microsoft.AspNetCore.Builder.ForwardedHeadersOptions.KnownProxies"/>
        List<string> AllowedHosts { get; set; } = new List<string> { "ssl" };
    }
}