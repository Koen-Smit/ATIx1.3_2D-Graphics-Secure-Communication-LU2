using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LU2_WebApi.Tests
{
    [TestClass]
    public sealed class Environment2DTests
    {
        private Mock<IRepository<Environment2D>> _mockRepo;
        private Environment2DController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<Environment2D>>();
            _controller = new Environment2DController(_mockRepo.Object);
        }

        [TestMethod]
        public async Task GetAll_ReturnsListOfEnvironments()
        {
            // Arrange
            var mockData = new List<Environment2D> { new Environment2D { Id = 1, Name = "TestEnv", MaxHeight = 500, MaxLength = 500 } };
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(mockData);

            // Act
            var result = await _controller.GetAll();
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<Environment2D>));
        }

        [TestMethod]
        public async Task GetById_ValidId_ReturnsEnvironment()
        {
            // Arrange
            var environment = new Environment2D { Id = 1, Name = "TestEnv", MaxHeight = 500, MaxLength = 500 };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(environment);

            // Act
            var result = await _controller.GetById(1);
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsInstanceOfType(okResult.Value, typeof(Environment2D));
        }

        [TestMethod]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync((Environment2D)null);

            // Act
            var result = await _controller.GetById(2);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Create_ValidEnvironment_ReturnsCreatedAtAction()
        {
            // Arrange
            var newEnv = new Environment2D { Id = 1, Name = "NewEnv", MaxHeight = 500, MaxLength = 500 };
            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Environment2D>())).ReturnsAsync(newEnv);

            // Act
            var result = await _controller.Create(newEnv);
            var createdResult = result.Result as CreatedAtActionResult;

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.IsInstanceOfType(createdResult.Value, typeof(Environment2D));
        }

        [TestMethod]
        public async Task Create_NullEnvironment_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Create(null);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Update_ValidEnvironment_ReturnsNoContent()
        {
            // Arrange
            var updatedEnv = new Environment2D { Id = 1, Name = "UpdatedEnv", MaxHeight = 600, MaxLength = 600 };
            _mockRepo.Setup(repo => repo.UpdateAsync(1, updatedEnv)).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(1, updatedEnv);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Update_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var updatedEnv = new Environment2D { Id = 1, Name = "UpdatedEnv", MaxHeight = 600, MaxLength = 600 };
            _mockRepo.Setup(repo => repo.UpdateAsync(2, updatedEnv)).ReturnsAsync(false);

            // Act
            var result = await _controller.Update(2, updatedEnv);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Delete_ValidId_ReturnsNoContent()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.DeleteAsync(2)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(2);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
    }
}
