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
                        new Claim(JwtRegisteredClaimNames.Sub,configuration.GetSection("Jwt:Subject").Value),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat,DateTime.Now.ToString()),
                        new Claim("UserId",user.Id.ToString()),
                        new Claim("Email",user.Email ?? String.Empty),
                        new Claim("UserName",user.UserName ?? String.Empty),
                    };

                        claims.Add(new Claim(role, role));

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                        configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddMinutes(10), signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                return BadRequest("Invalid credentials");
            }
            return NotFound("Missing username and/or password");
        }
    }
}
