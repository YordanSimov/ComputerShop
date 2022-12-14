using ComputerShop.Models.MediatR.Commands.IdentityCommands;
using ComputerShop.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using System.Security.Claims;
using System.Text;
using ComputerShop.Models.Configurations;

namespace ComputerShop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IConfiguration configuration;

        public IdentityController(IMediator mediator, IConfiguration configuration)
        {
            this.mediator = mediator;
            this.configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost(nameof(CreateUser))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> CreateUser([FromBody] UserRequest userRequest)
        {
            if (string.IsNullOrEmpty(userRequest.Email) || string.IsNullOrEmpty(userRequest.Password))
                return BadRequest("Email or password is invalid");

            var result = await mediator.Send(new CreateUserCommand(userRequest));

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost(nameof(Login))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if (loginRequest != null && !string.IsNullOrEmpty(loginRequest.UserName) && !string.IsNullOrEmpty(loginRequest.Password))
            {
                var user = await mediator.Send(new CheckUserAndPasswordCommand(loginRequest.UserName, loginRequest.Password));

                if (user != null)
                {
                    var role = await mediator.Send(new GetUserRoleCommand(loginRequest.UserName));
                    var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub,JWTAuthenticationSettings.Subject),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat,DateTime.Now.ToString()),
                        new Claim(IdentitySettings.UserId,user.Id.ToString()),
                        new Claim(IdentitySettings.Email,user.Email ?? String.Empty),
                        new Claim(IdentitySettings.UserName,user.UserName ?? String.Empty),
                        new Claim(role,role)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTAuthenticationSettings.Key));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(JWTAuthenticationSettings.Issuer,
                        JWTAuthenticationSettings.Audience, claims, expires: DateTime.UtcNow.AddMinutes(10), signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                return BadRequest("Invalid credentials");
            }
            return NotFound("Missing username and/or password");
        }
    }
}
