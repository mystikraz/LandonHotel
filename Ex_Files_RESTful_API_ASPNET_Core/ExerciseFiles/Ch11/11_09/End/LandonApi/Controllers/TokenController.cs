using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using LandonApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace LandonApi.Controllers
{
    [Route("/[controller]")]
    public class TokenController : Controller
    {
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly UserManager<UserEntity> _userManager;

        public TokenController(
            IOptions<IdentityOptions> identityOptions,
            SignInManager<UserEntity> signInManager,
            UserManager<UserEntity> userManager)
        {
            _identityOptions = identityOptions;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost(Name = nameof(TokenExchangeAsync))]
        public async Task<IActionResult> TokenExchangeAsync(
            OpenIdConnectRequest request)
        {
            if (!request.IsPasswordGrantType())
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    ErrorDescription = "The specified grant type is not supported."
                });
            }

            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The username or password is invalid."
                });
            }

            // Ensure the password is valid
            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The username or password is invalid."
                });
            }

            // Create a new authentication ticket and sign in
            var ticket = await CreateTicketAsync(request, user);
            return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
        }

        private async Task<AuthenticationTicket> CreateTicketAsync(
            OpenIdConnectRequest request,
            UserEntity user)
        {
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            // Create a new authentication ticket holding the user identity
            var ticket = new AuthenticationTicket(principal,
                new AuthenticationProperties(),
                OpenIdConnectServerDefaults.AuthenticationScheme);

            // Explicitly specify which claims should be included in the access token
            foreach (var claim in ticket.Principal.Claims)
            {
                // Never include the security stamp (it's a secret value)
                if (claim.Type == _identityOptions.Value.ClaimsIdentity.SecurityStampClaimType) continue;

                // TODO: If there are any other private/secret claims on the user that should
                // not be exposed publicly, handle them here!
                // The token is encoded but not encrypted, so it is effectively plaintext.

                claim.SetDestinations(OpenIdConnectConstants.Destinations.AccessToken);
            }

            return ticket;
        }
    }
}
