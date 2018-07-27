using System.Collections.Generic;
using IdentityServer4.Models;

namespace IS
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource {
                    Name = "Api",
                    DisplayName ="My API",
                    Description = "My API Access",
                    UserClaims = new List<string> {"role"},
                    ApiSecrets = new List<Secret> {new Secret("ThisIsSecretsInfaSotka".Sha256())},
                    Scopes = new List<Scope> {
                        new Scope("MyApi.read"),
                        new Scope("MyApi.write")
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client> {
                new Client
                {
                    ClientId = "resClient",
                    ClientName = "Client Credentials Client Application",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("ThisIsSecretsInfaSotka".Sha256())
                    },
                    AllowedScopes = new List<string> { "MyApi.read" }
                }
            };
        }
    }
}
