using ComputerShop.Controllers;
using ComputerShop.DL.Interfaces;
using ComputerShop.Models.MediatR.Commands;
using ComputerShop.Models.Models;
using ComputerShop.Models.Requests;
using ComputerShop.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace ComputerShop.Tests.ControllerTests
{
    public class ComputerControllerTest
    {
        private List<Computer> computers = new List<Computer>()
        {
            new Computer()
            {
                Id =1,
                BrandId = 1,
                Name = "Computer1",
                Price = 1000,
                Processor = "amd",
                RAM = 8,
                VideoCard = "nvidia"
            },
            new Computer()
            {
                Id =2,
                BrandId = 2,
                Name = "Computer2",
                Price = 1200,
                Processor = "amd",
                RAM = 16,
                VideoCard = "amd"
            },
        };

        private Mock<IMediator> mediator;
        private Mock<IComputerRepository> computerRepository;
        private Mock<IBrandRepository> brandRepository;

        public ComputerControllerTest()
        {
            computerRepository = new Mock<IComputerRepository>();
            brandRepository = new Mock<IBrandRepository>();
            mediator = new Mock<IMediator>();
        }
        [Fact]
        public async Task Computer_GetAll_Count()
        {
            var expectedCount = 2;

            mediator.Setup(x => x.Send(It.IsAny<GetAllComputersCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(computers);

            var controller = new ComputerController(mediator.Object);

            var result = await controller.GetAll();

            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var computerResult = okObjectResult.Value as IEnumerable<Computer>;

            Assert.NotNull(computerResult);
            Assert.NotEmpty(computerResult);
            Assert.Equal(expectedCount, computerResult.Count());
            Assert.Equal(computerResult, computers);
        }

        [Fact]
        public async Task Computer_GetAll_Count_Empty()
        {
            mediator.Setup(x => x.Send(It.IsAny<GetAllComputersCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<Computer>());

            var controller = new ComputerController(mediator.Object);

            var result = await controller.GetAll();
            var notFoundObjectResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundObjectResult);

            var messageResult = notFoundObjectResult.Value as string;
            Assert.NotNull(messageResult);
            Assert.Equal("There aren't any computers in the collection", messageResult);
        }

        [Fact]
        public async Task Computer_GetComputerById_NotFound()
        {
            var computerId = 3;
            var expectedComputer = computers.FirstOrDefault(x => x.Id == computerId);

            mediator.Setup(x => x.Send(It.IsAny<GetByIdComputerCommand>(), It.IsAny<CancellationToken>()));

            var controller = new ComputerController(mediator.Object);

            var result = await controller.GetById(computerId);

            var notFoundObjectResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundObjectResult);
            Assert.True(notFoundObjectResult.Value is int);
            Assert.Equal(computerId, (int)notFoundObjectResult.Value);
        }
        [Fact]
        public async Task Computer_GetComputerById_OK()
        {
            var computerId = 1;
            var expectedComputer = computers.FirstOrDefault(x => x.Id == computerId);

            mediator.Setup(x => x.Send(It.IsAny<GetByIdComputerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedComputer);

            var controller = new ComputerController(mediator.Object);

            var result = await controller.GetById(computerId);
            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var computer = okObjectResult.Value as Computer;
            Assert.NotNull(computer);
            Assert.Equal(expectedComputer.Id, computerId);
        }

        [Fact]
        public async Task Computer_GetComputerByName_OK()
        {
            var computerName = "Computer1";
            var expectedComputer = computers.FirstOrDefault(x => x.Name == computerName);

            mediator.Setup(x => x.Send(It.IsAny<GetByNameComputerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedComputer);

            var controller = new ComputerController(mediator.Object);

            var result = await controller.GetByName(computerName);
            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var computer = okObjectResult.Value as Computer;
            Assert.NotNull(computer);
            Assert.Equal(computer.Name, computerName);
        }

        [Fact]
        public async Task Computer_GetComputerByName_NotFound()
        {
            var computerName = "Computer3";
            var expectedComputer = computers.FirstOrDefault(x => x.Name == computerName);

            mediator.Setup(x => x.Send(It.IsAny<GetByNameComputerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedComputer);

            var controller = new ComputerController(mediator.Object);

            var result = await controller.GetByName(computerName);
            var notFoundObjectResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundObjectResult);
            Assert.True(notFoundObjectResult.Value is string);
            Assert.Equal(computerName, notFoundObjectResult.Value);
        }

        [Fact]

        public async Task Computer_Add_OK()
        {
            var computerRequest = new ComputerRequest()
            {
                BrandId = 1,
                Name = "add name",
                Price = 1200,
                Processor = "procesor",
                RAM = 8,
                VideoCard = "videocard"
            };
            var brands = new List<Brand>()
            {
                new Brand()
                {
                    Id = 1,
                    Name = "brand1"
                },
            };

            var expectedComputerId = 3;
            var computerToAdd = new Computer()
            {
                Id = expectedComputerId,
                Name = computerRequest.Name,
                Price = computerRequest.Price,
                Processor = computerRequest.Processor,
                RAM = computerRequest.RAM,
                VideoCard = computerRequest.VideoCard,
                BrandId = computerRequest.BrandId
            };

            var response = new ComputerResponse()
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Successfully added computer",
                Computer = computerToAdd
            };

            mediator.Setup(x => x.Send(It.IsAny<AddComputerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var controller = new ComputerController(mediator.Object);
            var result = await controller.Add(computerRequest);

            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var resultValue = okObjectResult.Value as ComputerResponse;
            Assert.NotNull(resultValue);
            Assert.Equal(response, resultValue);
        }

        [Fact]
        public async Task Computer_Add_BrandNotFound()
        {
            var computerRequest = new ComputerRequest()
            {
                BrandId = 2,
                Name = "add name",
                Price = 1200,
                Processor = "procesor",
                RAM = 8,
                VideoCard = "videocard"
            };
            var brands = new List<Brand>()
            {
                new Brand()
                {
                    Id = 1,
                    Name = "brand1"
                }
            };

            var expectedComputerId = 3;
            var computerToAdd = new Computer()
            {
                Id = expectedComputerId,
                Name = computerRequest.Name,
                Price = computerRequest.Price,
                Processor = computerRequest.Processor,
                RAM = computerRequest.RAM,
                VideoCard = computerRequest.VideoCard,
                BrandId = computerRequest.BrandId
            };

            var response = new ComputerResponse()
            {
                HttpStatusCode = HttpStatusCode.NotFound,
                Message = "Brand id not found",
            };

            mediator.Setup(x => x.Send(It.IsAny<AddComputerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var controller = new ComputerController(mediator.Object);
            var result = await controller.Add(computerRequest);

            var notFoundObjectResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundObjectResult);

            var resultValue = notFoundObjectResult.Value as ComputerResponse;
            Assert.NotNull(resultValue);
            Assert.Equal(response, resultValue);
        }

        [Fact]
        public async Task Computer_Add_ComputerAlreadyExists()
        {
            var computerRequest = new ComputerRequest()
            {
                BrandId = 1,
                Name = "Computer1",
                Price = 1200,
                Processor = "procesor",
                RAM = 8,
                VideoCard = "videocard"
            };
            var brands = new List<Brand>()
            {
                new Brand()
                {
                    Id = 1,
                    Name = "brand1"
                }
            };

            var expectedComputerId = 3;
            var computerToAdd = new Computer()
            {
                Id = expectedComputerId,
                Name = computerRequest.Name,
                Price = computerRequest.Price,
                Processor = computerRequest.Processor,
                RAM = computerRequest.RAM,
                VideoCard = computerRequest.VideoCard,
                BrandId = computerRequest.BrandId
            };

            var response = new ComputerResponse()
            {
                HttpStatusCode = HttpStatusCode.BadRequest,
                Message = "Computer already exists",
            };

            mediator.Setup(x => x.Send(It.IsAny<AddComputerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var controller = new ComputerController(mediator.Object);
            var result = await controller.Add(computerRequest);

            var badRequestObjectResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestObjectResult);

            var resultValue = badRequestObjectResult.Value as ComputerResponse;
            Assert.NotNull(resultValue);
            Assert.Equal(response, resultValue);
        }

        [Fact]

        public async Task Computer_Update_OK()
        {
            var computerRequest = new ComputerRequest()
            {
                Id = 1,
                BrandId = 1,
                Name = "Computer1",
                Price = 1200,
                Processor = "procesor",
                RAM = 8,
                VideoCard = "videocard"
            };
            var brands = new List<Brand>()
            {
                new Brand()
                {
                    Id = 1,
                    Name = "brand1"
                }
            };

            var computerToUpdate = computers.FirstOrDefault(x => x.Id == computerRequest.Id);

            var response = new ComputerResponse()
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Successfully updated computer",
                Computer = computerToUpdate,
            };

            mediator.Setup(x => x.Send(It.IsAny<UpdateComputerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var controller = new ComputerController(mediator.Object);
            var result = await controller.Update(computerRequest);

            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var resultValue = okObjectResult.Value as ComputerResponse;
            Assert.NotNull(resultValue);
            Assert.Equal(response.Computer.Id, resultValue.Computer.Id);
        }

        [Fact]
        public async Task Computer_Update_BrandIdNotFound()
        {
            var computerRequest = new ComputerRequest()
            {
                Id = 1,
                BrandId = 3,
                Name = "Computer1",
                Price = 1200,
                Processor = "procesor",
                RAM = 8,
                VideoCard = "videocard"
            };
            var brands = new List<Brand>()
            {
                new Brand()
                {
                    Id = 1,
                    Name = "brand1"
                }
            };

            var computerToUpdate = computers.FirstOrDefault(x => x.Id == computerRequest.Id);

            var response = new ComputerResponse()
            {
                HttpStatusCode = HttpStatusCode.NotFound,
                Message = "Brand id to update does not exist.",
            };

            mediator.Setup(x => x.Send(It.IsAny<UpdateComputerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var controller = new ComputerController(mediator.Object);
            var result = await controller.Update(computerRequest);

            var notFoundObjectResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundObjectResult);

            var resultValue = notFoundObjectResult.Value as ComputerResponse;
            Assert.NotNull(resultValue);
            Assert.Equal(response, resultValue);
        }

        [Fact]
        public async Task Computer_Update_ComputerNotFound()
        {
            var computerRequest = new ComputerRequest()
            {
                Id = 3,
                BrandId = 1,
                Name = "Computer3",
                Price = 1200,
                Processor = "procesor",
                RAM = 8,
                VideoCard = "videocard"
            };
            var brands = new List<Brand>()
            {
                new Brand()
                {
                    Id = 1,
                    Name = "brand1"
                }
            };

            var computerToUpdate = computers.FirstOrDefault(x => x.Id == computerRequest.Id);

            var response = new ComputerResponse()
            {
                HttpStatusCode = HttpStatusCode.NotFound,
                Message = "Computer to update does not exist.",
            };

            mediator.Setup(x => x.Send(It.IsAny<UpdateComputerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var controller = new ComputerController(mediator.Object);
            var result = await controller.Update(computerRequest);

            var notFoundObjectResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundObjectResult);

            var resultValue = notFoundObjectResult.Value as ComputerResponse;
            Assert.NotNull(resultValue);
            Assert.Equal(response, resultValue);
        }

        [Fact]
        public async Task Computer_Update_BadRequest()
        {
            mediator.Setup(x => x.Send(It.IsAny<UpdateComputerCommand>(), It.IsAny<CancellationToken>()));

            var controller = new ComputerController(mediator.Object);
            var result = await controller.Update(null);

            var badRequestObjectResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestObjectResult);

            var resultValue = badRequestObjectResult.Value as string;
            Assert.NotNull(resultValue);
            Assert.Equal("Computer can't be null", resultValue);
        }

        [Fact]

        public async Task Computer_Delete_OK()
        {
            var computerToDelete = computers.FirstOrDefault(x => x.Id == 1);
            var response = new ComputerResponse()
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Successfullt deleted computer",
                Computer = computerToDelete,
            };

            mediator.Setup(x => x.Send(It.IsAny<DeleteComputerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var controller = new ComputerController(mediator.Object);
            var result = await controller.Delete(computerToDelete.Id);

            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var resultValue = okObjectResult.Value as ComputerResponse;
            Assert.NotNull(resultValue);
            Assert.Equal(response, resultValue);
        }

        [Fact]

        public async Task Computer_Delete_NotFound()
        {
            var computer = new Computer()
            {
                Id = 4,
                BrandId = 1,
                Name = "Computer1",
                Price = 1000,
                Processor = "amd",
                RAM = 8,
                VideoCard = "nvidia"
            };
            var computerToDelete = computers.FirstOrDefault(x => x.Id == computer.Id);
            var response = new ComputerResponse()
            {
                HttpStatusCode = HttpStatusCode.NotFound,
                Message = "Computer to delete not found",
            };

            mediator.Setup(x => x.Send(It.IsAny<DeleteComputerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var controller = new ComputerController(mediator.Object);
            var result = await controller.Delete(computer.Id);

            var notFoundObjectResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundObjectResult);

            var resultValue = (int)notFoundObjectResult.Value;
            Assert.Equal(computer.Id, resultValue);
        }

        [Fact]
        public async Task Computer_Delete_BadRequest()
        {
            mediator.Setup(x => x.Send(It.IsAny<DeleteComputerCommand>(), It.IsAny<CancellationToken>()));

            var controller = new ComputerController(mediator.Object);
            var result = await controller.Delete(-1);

            var badRequestObjectResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestObjectResult);

            var resultValue = badRequestObjectResult.Value as string;
            Assert.NotNull(resultValue);
            Assert.Equal("Id must be greater than 0", resultValue);
        }
    }
}
