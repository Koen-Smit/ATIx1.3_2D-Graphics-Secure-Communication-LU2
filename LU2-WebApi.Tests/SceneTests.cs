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
    public async Task GetAllEntities_ShouldReturnEntities_WhenUserIsAuthenticated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var environmentId = Guid.NewGuid();
        var entities = new List<Entity>
    {
        new Entity { Id = Guid.NewGuid(), Prefab_Id = "test", PositionX = 1, PositionY = 1, ScaleX = 1, ScaleY = 1, RotationZ = 0, SortingLayer = 1, EnvironmentId = environmentId },
        new Entity { Id = Guid.NewGuid(), Prefab_Id = "test", PositionX = 2, PositionY = 2, ScaleX = 2, ScaleY = 2, RotationZ = 90, SortingLayer = 2, EnvironmentId = environmentId }
    };

        // Mock authentication service to return the user ID
        _mockAuthService
            .Setup(a => a.GetCurrentAuthenticatedUserId())
            .Returns(userId.ToString());

        // Mock environment repository to ensure the environment exists
        _mockEnvironmentRepository
            .Setup(repo => repo.GetEnvironmentById(environmentId, userId))
            .ReturnsAsync(new Environment { Id = environmentId, UserId = userId });

        // Mock entity repository to return the list of entities
        _mockEntityRepository
            .Setup(repo => repo.GetEntitiesFromEnvironment(environmentId))
            .ReturnsAsync(entities);

        // Act
        var result = await _controller.GetAllEntities(environmentId);

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.IsNotNull(result.Result, "Action result should not be null.");
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult), $"Expected OkObjectResult but got {result.Result.GetType().Name}.");

        var actionResult = result.Result as OkObjectResult;
        Assert.AreEqual(200, actionResult.StatusCode, "Status code should be 200 OK.");

        var returnedEntities = actionResult.Value as IEnumerable<Entity>;
        Assert.IsNotNull(returnedEntities, "Returned entities should not be null.");
        Assert.IsTrue(returnedEntities.Any(), "Returned entities should not be empty.");
        Assert.AreEqual(entities.Count, returnedEntities.Count(), "Returned entity count should match the mocked entities count.");

        var firstEntity = returnedEntities.First();
        Assert.AreEqual(entities.First().Id, firstEntity.Id, "First entity's ID should match.");
        Assert.AreEqual(entities.First().Prefab_Id, firstEntity.Prefab_Id, "First entity's Prefab_Id should match.");
        Assert.AreEqual(entities.First().PositionX, firstEntity.PositionX, "First entity's PositionX should match.");
    }


    [TestMethod]
    public async Task GetAllEntities_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _mockAuthService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(string.Empty);

        // Act
        var result = await _controller.GetAllEntities(Guid.NewGuid());

        // Assert
        var actionResult = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(actionResult, "Action result should not be null.");
        Assert.AreEqual(401, actionResult.StatusCode, "Status code should be 401 Unauthorized.");
        Assert.AreEqual("User is not authenticated.", actionResult.Value, "Error message should indicate authentication failure.");
    }

    [TestMethod]
    public async Task GetAllEntities_ShouldReturnNotFound_WhenEntitiesNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var environmentId = Guid.NewGuid();

        // Mock authentication service to return the user ID
        _mockAuthService
            .Setup(a => a.GetCurrentAuthenticatedUserId())
            .Returns(userId.ToString());

        // Mock environment repository to ensure the environment exists
        _mockEnvironmentRepository
            .Setup(repo => repo.GetEnvironmentById(environmentId, userId))
            .ReturnsAsync(new Environment { Id = environmentId, UserId = userId });

        // Mock entity repository to return null (indicating no entities found)
        _mockEntityRepository
            .Setup(repo => repo.GetEntitiesFromEnvironment(environmentId))
            .ReturnsAsync((List<Entity>)null);

        // Act
        var result = await _controller.GetAllEntities(environmentId);

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.IsNotNull(result.Result, "Action result should not be null.");

        if (result.Result is NotFoundObjectResult notFoundResult)
        {
            Assert.AreEqual(404, notFoundResult.StatusCode, "Status code should be 404 Not Found.");
            Assert.AreEqual("No entities found for this environment.", notFoundResult.Value, "Error message should indicate no entities found.");
        }
        else
        {
            Assert.Fail($"Unexpected result type: {result.Result.GetType().Name}");
        }
    }


    [TestMethod]
    public async Task CreateEntity_ShouldReturnCreated_WhenUserIsAuthenticated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var environmentId = Guid.NewGuid();

        var createRequest = new EntityRequest
        {
            Prefab_Id = "test",
            PositionX = 1,
            PositionY = 1,
            ScaleX = 1,
            ScaleY = 1,
            RotationZ = 0,
            SortingLayer = 1,
            EnvironmentId = environmentId
        };

        var createdEntity = new Entity
        {
            Id = Guid.NewGuid(),
            Prefab_Id = createRequest.Prefab_Id,
            PositionX = createRequest.PositionX,
            PositionY = createRequest.PositionY,
            ScaleX = createRequest.ScaleX,
            ScaleY = createRequest.ScaleY,
            RotationZ = createRequest.RotationZ,
            SortingLayer = createRequest.SortingLayer,
            EnvironmentId = createRequest.EnvironmentId
        };

        // Mock authentication service to return the user ID
        _mockAuthService
            .Setup(a => a.GetCurrentAuthenticatedUserId())
            .Returns(userId.ToString());

        // Mock environment repository to return a valid environment
        _mockEnvironmentRepository
            .Setup(repo => repo.GetEnvironmentById(environmentId, userId))
            .ReturnsAsync(new Environment { Id = environmentId, UserId = userId });

        // Mock entity repository to simulate successful entity creation
        _mockEntityRepository
            .Setup(repo => repo.CreateEntity(It.IsAny<Entity>()))
            .ReturnsAsync(1);

        // Act
        var result = await _controller.CreateEntity(createRequest);

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.IsNotNull(result.Result, "Action result should not be null.");

        if (result.Result is CreatedAtActionResult createdResult)
        {
            Assert.AreEqual(201, createdResult.StatusCode, "Status code should be 201 Created.");
            Assert.AreEqual(nameof(_controller.GetAllEntities), createdResult.ActionName, "Action name should be GetAllEntities.");
        }
        else if (result.Result is BadRequestObjectResult badRequest)
        {
            Assert.Fail($"Request failed with error: {badRequest.Value}");
        }
        else
        {
            Assert.Fail($"Unexpected result type: {result.Result.GetType().Name}");
        }
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