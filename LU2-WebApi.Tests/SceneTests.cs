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
    private Mock<IEnvironmentRepository>? _mockEnvironmentRepository;
    private Mock<IEntityRepository>? _mockEntityRepository;
    private Mock<IAuthenticationService>? _mockAuthService;
    private SceneController? _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockEnvironmentRepository = new Mock<IEnvironmentRepository>();
        _mockEntityRepository = new Mock<IEntityRepository>();
        _mockAuthService = new Mock<IAuthenticationService>();

        _controller = new SceneController(
            _mockEnvironmentRepository.Object,
            _mockEntityRepository.Object,
            _mockAuthService.Object
        );
    }

    // Systeemtest: Test of de juiste omgevingen worden opgehaald voor een geauthenticeerde gebruiker.
    [TestMethod]
    public async Task GetEnvironments_ShouldReturnEnvironments_WhenUserIsAuthenticated()
    {
        var userId = Guid.NewGuid();
        var environments = new List<Environment>
        {
            new Environment { Id = Guid.NewGuid(), Name = "Scene 1", UserId = userId, MaxLength = 20, MaxHeight = 25, CreatedAt = DateTime.UtcNow },
            new Environment { Id = Guid.NewGuid(), Name = "Scene 2", UserId = userId, MaxLength = 24, MaxHeight = 28, CreatedAt = DateTime.UtcNow }
        };

        _mockAuthService!.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId.ToString());
        _mockEnvironmentRepository!.Setup(repo => repo.GetEnvironmentsFromUser(userId)).ReturnsAsync(environments);

        var result = await _controller!.GetEnvironments();

        var actionResult = result.Result as OkObjectResult;
        Assert.IsNotNull(actionResult, "Action result should not be null.");
        Assert.AreEqual(200, actionResult.StatusCode, "Status code should be 200 OK.");

        var returnedEnvironments = actionResult.Value as IEnumerable<EnvironmentDTO>;
        Assert.IsNotNull(returnedEnvironments, "Returned environments should not be null.");
        Assert.AreEqual(environments.Count, returnedEnvironments.Count(), "Returned environment count should match the mocked environments count.");
        Assert.AreEqual(environments.First().Name, returnedEnvironments.First().Name, "The first environment name should match.");
    }

    // Systeemtest: Test of een ongeauthenticeerde gebruiker geen omgevingen kan ophalen.
    [TestMethod]
    public async Task GetEnvironments_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        _mockAuthService!.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(string.Empty);
        var result = await _controller!.GetEnvironments();

        var actionResult = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(actionResult, "Action result should not be null.");
        Assert.AreEqual(401, actionResult.StatusCode, "Status code should be 401 Unauthorized.");
        Assert.AreEqual("User is not authenticated.", actionResult.Value, "Error message should indicate authentication failure.");
    }

    // Unit test: Test of de methode correct een interne fout afhandelt.
    [TestMethod]
    public async Task GetEnvironments_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        var userId = Guid.NewGuid();
        _mockAuthService!.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId.ToString());
        _mockEnvironmentRepository!.Setup(repo => repo.GetEnvironmentsFromUser(userId)).ThrowsAsync(new Exception("Database error"));

        var result = await _controller!.GetEnvironments();

        var actionResult = result.Result as ObjectResult;
        Assert.IsNotNull(actionResult, "Action result should not be null.");
        Assert.AreEqual(500, actionResult.StatusCode, "Status code should be 500 Internal Server Error.");
        Assert.AreEqual("An error occurred while fetching user environments: Database error", actionResult.Value, "Error message should match the exception message.");
    }

    // Systeemtest: Test of de juiste entiteiten worden opgehaald voor een geauthenticeerde gebruiker.
    [TestMethod]
    public async Task GetAllEntities_ShouldReturnEntities_WhenUserIsAuthenticated()
    {
        var userId = Guid.NewGuid();
        var environmentId = Guid.NewGuid();
        var entities = new List<Entity>
        {
            new Entity { Id = Guid.NewGuid(), Prefab_Id = "test", PositionX = 1, PositionY = 1, ScaleX = 1, ScaleY = 1, RotationZ = 0, SortingLayer = 1, EnvironmentId = environmentId },
            new Entity { Id = Guid.NewGuid(), Prefab_Id = "test", PositionX = 2, PositionY = 2, ScaleX = 2, ScaleY = 2, RotationZ = 90, SortingLayer = 2, EnvironmentId = environmentId }
        };

        _mockAuthService!.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId.ToString());
        _mockEnvironmentRepository!.Setup(repo => repo.GetEnvironmentById(environmentId, userId)).ReturnsAsync(new Environment { Id = environmentId, UserId = userId });
        _mockEntityRepository!.Setup(repo => repo.GetEntitiesFromEnvironment(environmentId)).ReturnsAsync(entities);

        var result = await _controller!.GetAllEntities(environmentId);

        Assert.IsNotNull(result, "Result should not be null.");
        Assert.IsNotNull(result.Result, "Action result should not be null.");
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult), $"Expected OkObjectResult but got {result.Result.GetType().Name}.");

        var actionResult = result.Result as OkObjectResult;
        Assert.AreEqual(200, actionResult?.StatusCode, "Status code should be 200 OK.");
    }

    // Unit test: Controleert of een ongeauthenticeerde gebruiker geen entiteiten kan ophalen.
    [TestMethod]
    public async Task GetAllEntities_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        _mockAuthService!.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(string.Empty);
        var result = await _controller!.GetAllEntities(Guid.NewGuid());

        var actionResult = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(actionResult, "Action result should not be null.");
        Assert.AreEqual(401, actionResult.StatusCode, "Status code should be 401 Unauthorized.");
        Assert.AreEqual("User is not authenticated.", actionResult.Value, "Error message should indicate authentication failure.");
    }
}
