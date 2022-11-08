using AutoMapper;
using ComputerShop.DL.MongoRepositories;
using ComputerShop.Models.MediatR.Commands.IdentityCommands;
using ComputerShop.Models.Models;
using ComputerShop.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ComputerShop.BL.CommandHandlers.IdentityCommandHandlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
    {
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;

        public CreateUserCommandHandler(IMapper mapper, IUserRepository userRepository)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
        }

        public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var usernameCheck = await userRepository.GetUserByUserName(request.UserRequest.UserName);

            if (usernameCheck != null)
            {
                return new UserResponse()
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "Username already in use",
                    UserName = request.UserRequest.UserName
                };
            }
            var user = mapper.Map<User>(request.UserRequest);

            user.Password = HashPassword(user.Password);
            user.Id = Guid.NewGuid();

            var result = await userRepository.CreateUser(user, cancellationToken);
            if (result == IdentityResult.Success)
            {
                return new UserResponse()
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Message = "Successfully created user",
                    UserName = request.UserRequest.UserName
                };
            }
            return new UserResponse()
            {
                HttpStatusCode = HttpStatusCode.BadRequest,
                Message = "Error creating user",
                UserName = request.UserRequest.UserName
            };

        }
        private string HashPassword(string password)
        {
            using (var sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                var stringbuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    stringbuilder.Append(bytes[i].ToString("x2"));
                }
                return stringbuilder.ToString();
            }
        }
    }
}
