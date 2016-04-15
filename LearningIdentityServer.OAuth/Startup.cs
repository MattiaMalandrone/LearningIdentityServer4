using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using IdentityModel;
using IdentityServer4.Core;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services.InMemory;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace LearningIdentityServer.OAuth
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var source = System.IO.File.ReadAllText("MyCertificate.b64cert");
            var certBytes = Convert.FromBase64String(source);
            var certificate = new X509Certificate2(certBytes, "password");

            var builder = services.AddIdentityServer(options =>
            {
                options.SigningCertificate = certificate;
                options.RequireSsl = false; // should be true
            });

            builder.AddInMemoryClients(Clients.Get());
            builder.AddInMemoryScopes(Scopes.Get());
            builder.AddInMemoryUsers(Users.Get());
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Verbose);
            loggerFactory.AddDebug(LogLevel.Verbose);
            app.UseIdentityServer();
        }

        public static void Main(string [] args) => WebApplication.Run<Startup>(args);
    }

    public class Users
    {
        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>
            {
                new InMemoryUser
                {
                    Subject = "elemarjr@gmail.com",
                    Username = "elemarjr@gmail.com",
                    Password = "mypass",
                    Claims = new []
                    {
                        new Claim(JwtClaimTypes.Name, "Elemar Jr")
                    }
                }
            };
        }
    }

    public class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            return new[]
            {
                StandardScopes.OpenId,
                StandardScopes.Profile,
                StandardScopes.OfflineAccess,
                new Scope {Name = "advanced", DisplayName = "Advanced Options"}
            };
        }
    }

    public class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "myapi",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    ClientName = "My Beautiful Api",
                    Flow = Flows.ResourceOwner,
                    AllowedScopes =
                    {
                        Constants.StandardScopes.OpenId,
                        "read"
                    },
                    Enabled = true
                }
            };
        }
    }
}
