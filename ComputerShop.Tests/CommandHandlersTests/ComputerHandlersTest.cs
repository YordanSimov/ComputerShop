using AutoMapper;
using ComputerShop.Automapper;
using ComputerShop.BL.CommandHandlers;
using ComputerShop.DL.Interfaces;
using ComputerShop.Models.MediatR.Commands;
using ComputerShop.Models.Models;
using ComputerShop.Models.Requests;
using ComputerShop.Models.Responses;
using Moq;
using System.Net;

namespace ComputerShop.Tests.CommandHandlersTests
{
    public class ComputerHandlersTest
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

        private readonly IMapper mapper;
        private Mock<IComputerRepository> computerRepository;
        private Mock<IBrandRepository> brandRepository;
        private CancellationToken cancellationToken;

        public ComputerHandlersTest()
        {
            var mockMapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new Automapping());
            });
            mapper = mockMapperConfig.CreateMapper();

            computerRepository = new Mock<IComputerRepository>();
            brandRepository = new Mock<IBrandRepository>();
            cancellationToken = new CancellationToken();
        }

        [Fact]
        public async Task Computer_GetAll_Count()
        {
            var expectedCount = 2;
            computerRepository.Setup(x => x.GetAll()).ReturnsAsync(computers);

            var getAllCommand = new GetAllComputersCommand();
            var handler = new GetAllComputersCommandHandler(computerRepository.Object);
            var handlerResult = await handler.Handle(getAllCommand, cancellationToken);

            Assert.NotNull(handlerResult);
            Assert.NotEmpty(handlerResult);
            Assert.Equal(expectedCount, handlerResult.Count());
        }

        [Fact]
        public async Task Computer_GetAll_Count_Empty()
        {
            computerRepository.Setup(x => x.GetAll()).ReturnsAsync(() => Enumerable.Empty<Computer>());

            var getAllCommand = new GetAllComputersCommand();
            var handler = new GetAllComputersCommandHandler(computerRepository.Object);
            var handlerResult = await handler.Handle(getAllCommand, cancellationToken);

            Assert.NotNull(handlerResult);
            Assert.Empty(handlerResult);
        }

        [Fact]
        public async Task Computer_GetComputerById_NotFound()
        {
            var computerId = 3;
            var expectedComputer = computers.FirstOrDefault(x => x.Id == computerId);

            computerRepository.Setup(x => x.GetById(computerId)).ReturnsAsync(expectedComputer);

            var getByIdCommand = new GetByIdComputerCommand(computerId);
            var handler = new GetByIdComputerCommandHandler(computerRepository.Object);
            var handlerResult = await handler.Handle(getByIdCommand, cancellationToken);           

            Assert.Null(handlerResult);
            Assert.Equal(expectedComputer, handlerResult);
        }
        [Fact]
        public async Task Computer_GetComputerById_OK()
        {
            var computerId = 1;
            var expectedComputer = computers.FirstOrDefault(x => x.Id == computerId);

            computerRepository.Setup(x => x.GetById(computerId)).ReturnsAsync(expectedComputer);

            var getByIdCommand = new GetByIdComputerCommand(computerId);
            var handler = new GetByIdComputerCommandHandler(computerRepository.Object);
            var handlerResult = await handler.Handle(getByIdCommand, cancellationToken);
        
            Assert.NotNull(handlerResult);
            Assert.Equal(expectedComputer, handlerResult);
        }

        [Fact]
        public async Task Computer_GetComputerByName_OK()
        {
            var computerName = "Computer1";
            var expectedComputer = computers.FirstOrDefault(x => x.Name == computerName);

            computerRepository.Setup(x => x.GetByName(computerName)).ReturnsAsync(expectedComputer);

            var getByNameCommand = new GetByNameComputerCommand(computerName);
            var handler = new GetByNameComputerCommandHandler(computerRepository.Object);
            var handlerResult = await handler.Handle(getByNameCommand, cancellationToken);
           
            Assert.NotNull(handlerResult);
            Assert.Equal(expectedComputer, handlerResult);
        }

        [Fact]
        public async Task Computer_GetComputerByName_NotFound()
        {
            var computerName = "Computer3";
            var expectedComputer = computers.FirstOrDefault(x => x.Name == computerName);

            computerRepository.Setup(x => x.GetByName(computerName)).ReturnsAsync(expectedComputer);
          
            var getByNameCommand = new GetByNameComputerCommand(computerName);
            var handler = new GetByNameComputerCommandHandler(computerRepository.Object);
            var handlerResult = await handler.Handle(getByNameCommand, cancellationToken);

            Assert.Null(handlerResult);
            Assert.Equal(expectedComputer, handlerResult);
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

            computerRepository.Setup(x => x.GetByName(computerRequest.Name))
                .ReturnsAsync(computers.FirstOrDefault(x => x.Name == computerRequest.Name));
            brandRepository.Setup(x => x.GetById(computerRequest.BrandId))
                .ReturnsAsync(brands.FirstOrDefault(x => x.Id == computerRequest.BrandId));

            computerRepository.Setup(x => x.Add(It.IsAny<Computer>())).Callback(() =>
            {
                computers.Add(computerToAdd);
            })!.ReturnsAsync(() => computers.FirstOrDefault(x => x.Id == expectedComputerId));

            var addCommand = new AddComputerCommand(computerRequest);
            var handler = new AddComputerCommandHandler(computerRepository.Object, mapper, brandRepository.Object);
            var handlerResult = await handler.Handle(addCommand, cancellationToken);

            Assert.NotNull(handlerResult);
            Assert.Equal(response.Message, handlerResult.Message);
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

            computerRepository.Setup(x => x.GetByName(computerRequest.Name))
                .ReturnsAsync(computers.FirstOrDefault(x => x.Name == computerRequest.Name));
            brandRepository.Setup(x => x.GetById(computerRequest.BrandId))
                .ReturnsAsync(brands.FirstOrDefault(x => x.Id == computerRequest.BrandId));

            computerRepository.Setup(x => x.Add(It.IsAny<Computer>())).Callback(() =>
            {
                computers.Add(computerToAdd);
            })!.ReturnsAsync(() => computers.FirstOrDefault(x => x.Id == expectedComputerId));
           
            var addCommand = new AddComputerCommand(computerRequest);
            var handler = new AddComputerCommandHandler(computerRepository.Object, mapper, brandRepository.Object);
            var handlerResult = await handler.Handle(addCommand, cancellationToken);

            Assert.NotNull(handlerResult);
            Assert.Equal(response.Message, handlerResult.Message);
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

            computerRepository.Setup(x => x.GetByName(computerRequest.Name))
                .ReturnsAsync(computers.FirstOrDefault(x => x.Name == computerRequest.Name));
            brandRepository.Setup(x => x.GetById(computerRequest.BrandId))
                .ReturnsAsync(brands.FirstOrDefault(x => x.Id == computerRequest.BrandId));

            computerRepository.Setup(x => x.Add(It.IsAny<Computer>())).Callback(() =>
            {
                computers.Add(computerToAdd);
            })!.ReturnsAsync(() => computers.FirstOrDefault(x => x.Id == expectedComputerId));

            var addCommand = new AddComputerCommand(computerRequest);
            var handler = new AddComputerCommandHandler(computerRepository.Object, mapper, brandRepository.Object);
            var handlerResult = await handler.Handle(addCommand, cancellationToken);

            Assert.NotNull(handlerResult);
            Assert.Equal(response.Message, handlerResult.Message);
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

            computerRepository.Setup(x => x.GetByName(computerRequest.Name))
                .ReturnsAsync(computers.FirstOrDefault(x => x.Name == computerRequest.Name));
            brandRepository.Setup(x => x.GetById(computerRequest.BrandId))
                .ReturnsAsync(brands.FirstOrDefault(x => x.Id == computerRequest.BrandId));

            computerRepository.Setup(x => x.Update(It.IsAny<Computer>())).Callback(() =>
            {
                computers.Remove(computerToUpdate);
                computers.Add(new Computer()
                {
                    Id = 1,
                    Name = computerRequest.Name,
                    Price = computerRequest.Price,
                    Processor = computerRequest.Processor,
                    RAM = computerRequest.RAM,
                    VideoCard = computerRequest.VideoCard,
                    BrandId = computerRequest.BrandId
                });
            });

            var updateCommand = new UpdateComputerCommand(computerRequest);
            var handler = new UpdateComputerCommandHandler(mapper, computerRepository.Object, brandRepository.Object);
            var handlerResult = await handler.Handle(updateCommand, cancellationToken);

            Assert.NotNull(handlerResult);
            Assert.Equal(response.Message, handlerResult.Message);
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

            computerRepository.Setup(x => x.GetByName(computerRequest.Name))
                .ReturnsAsync(computers.FirstOrDefault(x => x.Name == computerRequest.Name));
            brandRepository.Setup(x => x.GetById(computerRequest.BrandId))
                .ReturnsAsync(brands.FirstOrDefault(x => x.Id == computerRequest.BrandId));

            var updateCommand = new UpdateComputerCommand(computerRequest);
            var handler = new UpdateComputerCommandHandler(mapper, computerRepository.Object, brandRepository.Object);
            var handlerResult = await handler.Handle(updateCommand, cancellationToken);

            Assert.NotNull(handlerResult);
            Assert.Equal(response.Message, handlerResult.Message);
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

            computerRepository.Setup(x => x.GetByName(computerRequest.Name))
                .ReturnsAsync(computers.FirstOrDefault(x => x.Name == computerRequest.Name));
            brandRepository.Setup(x => x.GetById(computerRequest.BrandId))
                .ReturnsAsync(brands.FirstOrDefault(x => x.Id == computerRequest.BrandId));

            var updateCommand = new UpdateComputerCommand(computerRequest);
            var handler = new UpdateComputerCommandHandler(mapper, computerRepository.Object, brandRepository.Object);
            var handlerResult = await handler.Handle(updateCommand, cancellationToken);

            Assert.NotNull(handlerResult);
            Assert.Equal(response.Message, handlerResult.Message);
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

            computerRepository.Setup(x => x.GetById(computerToDelete.Id))
                .ReturnsAsync(computerToDelete);
            computerRepository.Setup(x => x.Delete(computerToDelete.Id)).ReturnsAsync(() => computerToDelete);

            var deleteCommand = new DeleteComputerCommand(computerToDelete.Id);
            var handler = new DeleteComputerCommandHandler(computerRepository.Object);
            var handlerResult = await handler.Handle(deleteCommand, cancellationToken);

            Assert.NotNull(handlerResult);
            Assert.Equal(response.Message, handlerResult.Message);
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

            computerRepository.Setup(x => x.GetById(computer.Id))
                .ReturnsAsync(computerToDelete);
            computerRepository.Setup(x => x.Delete(computer.Id)).ReturnsAsync(() => computerToDelete);

            var deleteCommand = new DeleteComputerCommand(computer.Id);
            var handler = new DeleteComputerCommandHandler(computerRepository.Object);
            var handlerResult = await handler.Handle(deleteCommand, cancellationToken);

            Assert.NotNull(handlerResult);
            Assert.Equal(response.Message, handlerResult.Message);
        }
    }
}
