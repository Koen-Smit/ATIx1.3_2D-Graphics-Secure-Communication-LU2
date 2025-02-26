using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[TestClass]
public class SceneControllerTests
{
    private Mock<IEnvironmentRepository> _mockEnvironmentRepository;
    private Mock<IEntityRepository> _mockEntityRepository;
    private Mock<IAuthenticationService> _mockAuthService;
    private SceneController _controller;

    [TestInitialize]
    public void Setup()
    {
        // Setup mocks for the dependencies
        _mockEnvironmentRepository = new Mock<IEnvironmentRepository>();
        _mockEntityRepository = new Mock<IEntityRepository>();
        _mockAuthService = new Mock<IAuthenticationService>();

        // Initialize the controller with mocked dependencies
        _controller = new SceneController(
            _mockEnvironmentRepository.Object,
            _mockEntityRepository.Object,
            _mockAuthService.Object
        );
    }

    [TestMethod]
    public async Task GetEnvironments_ShouldReturnEnvironments_WhenUserIsAuthenticated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var environments = new List<Environment>
        {
            new Environment { Id = Guid.NewGuid(), Name = "Scene 1", UserId = userId, MaxLength = 20, MaxHeight = 25, CreatedAt = DateTime.UtcNow },
            new Environment { Id = Guid.NewGuid(), Name = "Scene 2", UserId = userId, MaxLength = 24, MaxHeight = 28, CreatedAt = DateTime.UtcNow }
        };

        // Mock the authentication service to return the user ID
        _mockAuthService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId.ToString());

        // Mock the repository to return the environments for the user
        _mockEnvironmentRepository.Setup(repo => repo.GetEnvironmentsFromUser(userId)).ReturnsAsync(environments);

        // Act
        var result = await _controller.GetEnvironments();

        // Assert
        var actionResult = result.Result as OkObjectResult;
        Assert.IsNotNull(actionResult, "Action result should not be null.");
        Assert.AreEqual(200, actionResult.StatusCode, "Status code should be 200 OK.");

        var returnedEnvironments = actionResult.Value as IEnumerable<EnvironmentDTO>;
        Assert.IsNotNull(returnedEnvironments, "Returned environments should not be null.");
        Assert.AreEqual(environments.Count, returnedEnvironments.Count(), "Returned environment count should match the mocked environments count.");
        Assert.AreEqual(environments.First().Name, returnedEnvironments.First().Name, "The first environment name should match.");
    }

    [TestMethod]
    public async Task GetEnvironments_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _mockAuthService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(string.Empty);

        // Act
        var result = await _controller.GetEnvironments();

        // Assert
        var actionResult = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(actionResult, "Action result should not be null.");
        Assert.AreEqual(401, actionResult.StatusCode, "Status code should be 401 Unauthorized.");
        Assert.AreEqual("User is not authenticated.", actionResult.Value, "Error message should indicate authentication failure.");
    }

    [TestMethod]
    public async Task GetEnvironments_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockAuthService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId.ToString());
        _mockEnvironmentRepository.Setup(repo => repo.GetEnvironmentsFromUser(userId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetEnvironments();

        // Assert
        var actionResult = result.Result as ObjectResult;
        Assert.IsNotNull(actionResult, "Action result should not be null.");
        Assert.AreEqual(500, actionResult.StatusCode, "Status code should be 500 Internal Server Error.");
        Assert.AreEqual("An error occurred while fetching user environments: Database error", actionResult.Value, "Error message should match the exception message.");
    }

    [TestMethod]
    public async Task GetAllEntities_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _mockAuthService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(string.Empty);

        // Act
        var result = await _controller.GetAllEntitys(Guid.NewGuid());

        // Assert
        var actionResult = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(actionResult, "Action result should not be null.");
        Assert.AreEqual(401, actionResult.StatusCode, "Status code should be 401 Unauthorized.");
        Assert.AreEqual("User is not authenticated.", actionResult.Value, "Error message should indicate authentication failure.");
    }

    [TestMethod]
    public async Task CreateEntity_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _mockAuthService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(string.Empty);
        var createRequest = new EntityRequest
        {
            Prefab_Id = "test",
            PositionX = 1,
            PositionY = 1,
            ScaleX = 1,
            ScaleY = 1,
            RotationZ = 0,
            SortingLayer = 1,
            EnvironmentId = Guid.NewGuid()
        };

        // Act
        var result = await _controller.CreateEntity(createRequest);

        // Assert
        var actionResult = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(actionResult, "Action result should not be null.");
        Assert.AreEqual(401, actionResult.StatusCode, "Status code should be 401 Unauthorized.");
        Assert.AreEqual("User is not authenticated.", actionResult.Value, "Error message should indicate authentication failure.");
    }

    [TestMethod]
    public async Task CreateEntity_ShouldReturnNotFound_WhenEnvironmentDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var createRequest = new EntityRequest
        {
            Prefab_Id = "test",
            PositionX = 1,
            PositionY = 1,
            ScaleX = 1,
            ScaleY = 1,
            RotationZ = 0,
            SortingLayer = 1,
            EnvironmentId = Guid.NewGuid() // Non-existent environment
        };

        // Mock the authentication service to return the user ID
        _mockAuthService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId.ToString());

        // Mock the repository to return null for the environment
        _mockEnvironmentRepository.Setup(repo => repo.GetEnvironmentById(createRequest.EnvironmentId, userId)).ReturnsAsync((Environment)null);

        // Act
        var result = await _controller.CreateEntity(createRequest);

        // Assert
        var actionResult = result.Result as NotFoundObjectResult;
        Assert.IsNotNull(actionResult, "Action result should not be null.");
        Assert.AreEqual(404, actionResult.StatusCode, "Status code should be 404 Not Found.");
        Assert.AreEqual("Environment not found or does not belong to the user.", actionResult.Value, "Error message should indicate environment does not exist.");
    }
}