using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LU2_WebApi.Tests
{
    [TestClass]
    public sealed class Object2DTests
    {
        private Mock<IRepository<Object2D>> _mockRepo;
        private Object2DController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<Object2D>>();
            _controller = new Object2DController(_mockRepo.Object);
        }

        [TestMethod]
        public async Task GetAll_ReturnsListOfObjects()
        {
            var mockData = new List<Object2D> { new Object2D { Id = 1, PrefabId = "Prefab123" } };
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(mockData);
            var result = await _controller.GetAll();
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<Object2D>));
        }

        [TestMethod]
        public async Task GetById_ValidId_ReturnsObject()
        {
            var obj = new Object2D { Id = 1, PrefabId = "Prefab123" };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(obj);
            var result = await _controller.GetById(1);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsInstanceOfType(okResult.Value, typeof(Object2D));
        }

        [TestMethod]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            _mockRepo.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync((Object2D)null);
            var result = await _controller.GetById(2);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Create_ValidObject_ReturnsCreatedAtAction()
        {
            var newObj = new Object2D { Id = 1, PrefabId = "Prefab123" };
            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Object2D>())).ReturnsAsync(newObj);
            var result = await _controller.Create(newObj);
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.IsInstanceOfType(createdResult.Value, typeof(Object2D));
        }

        [TestMethod]
        public async Task Create_NullObject_ReturnsBadRequest()
        {
            var result = await _controller.Create(null);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Update_ValidObject_ReturnsNoContent()
        {
            var updatedObj = new Object2D { Id = 1, PrefabId = "UpdatedPrefab" };
            _mockRepo.Setup(repo => repo.UpdateAsync(1, updatedObj)).ReturnsAsync(true);
            var result = await _controller.Update(1, updatedObj);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Update_InvalidId_ReturnsNotFound()
        {
            var updatedObj = new Object2D { Id = 1, PrefabId = "UpdatedPrefab" };
            _mockRepo.Setup(repo => repo.UpdateAsync(2, updatedObj)).ReturnsAsync(false);
            var result = await _controller.Update(2, updatedObj);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Delete_ValidId_ReturnsNoContent()
        {
            _mockRepo.Setup(repo => repo.DeleteAsync(1)).ReturnsAsync(true);
            var result = await _controller.Delete(1);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_InvalidId_ReturnsNotFound()
        {
            _mockRepo.Setup(repo => repo.DeleteAsync(2)).ReturnsAsync(false);
            var result = await _controller.Delete(2);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
    }
}
