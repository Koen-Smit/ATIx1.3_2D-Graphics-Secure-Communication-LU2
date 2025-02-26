using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

[TestClass]
public class AccountControllerTests
{
    private Mock<IAccountRepository> _mockAccountRepo;
    private Mock<IAuthenticationService> _mockAuthService;
    private Mock<UserManager<AppUser>> _mockUserManager;
    private AccountController _controller;

    public AccountControllerTests()
    {
        _mockAccountRepo = new Mock<IAccountRepository>();
        _mockAuthService = new Mock<IAuthenticationService>();

        var userStore = new Mock<IUserStore<AppUser>>();
        _mockUserManager = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);

        _controller = new AccountController(_mockAccountRepo.Object, _mockAuthService.Object, _mockUserManager.Object);
    }

    [TestMethod]
    public async Task Register_ReturnsOk_WhenRegistrationSucceeds()
    {
        // Arrange
        var request = new AccountRequest { UserName = "testuser", Password = "Test@12345" };
        _mockAccountRepo.Setup(repo => repo.RegisterUser(request))
                        .ReturnsAsync(Result.Success("Registration successful"));

        // Act
        var result = await _controller.Register(request);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual("Registration successful", okResult.Value);
    }

    [TestMethod]
    public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
    {
        // Arrange
        var request = new AccountRequest { UserName = "testuser", Password = "Test@12345" };
        _mockAccountRepo.Setup(repo => repo.RegisterUser(request))
                        .ReturnsAsync(Result.Failure("Error occurred"));

        // Act
        var result = await _controller.Register(request);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.AreEqual("Error occurred", badRequestResult.Value);
    }

    [TestMethod]
    public async Task Login_ReturnsOk_WhenLoginSucceeds()
    {
        // Arrange
        var request = new LoginRequest { UserName = "testuser", Password = "Test@12345" };
        _mockAccountRepo.Setup(repo => repo.LoginUser(request))
                        .ReturnsAsync(Result.Success("Login successful"));

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual("Login successful", okResult.Value);
    }

    [TestMethod]
    public async Task Login_ReturnsBadRequest_WhenLoginFails()
    {
        // Arrange
        var request = new LoginRequest { UserName = "testuser", Password = "wrongpass" };
        _mockAccountRepo.Setup(repo => repo.LoginUser(request))
                        .ReturnsAsync(Result.Failure("Invalid credentials"));

        // Act
        var result = await _controller.Login(request);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.AreEqual("Invalid credentials", badRequestResult.Value);
    }

    [TestMethod]
    public async Task Logout_ReturnsOk_WhenLogoutSucceeds()
    {
        // Arrange
        _mockAccountRepo.Setup(repo => repo.LogoutUser())
                        .ReturnsAsync(Result.Success("Logout successful"));

        // Act
        var result = await _controller.Logout();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual("Logout successful", okResult.Value);
    }

    [TestMethod]
    public async Task GetUserName_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId())
                        .Returns((string)null);

        // Act
        var result = await _controller.GetUserName();

        // Assert
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
        Assert.AreEqual("User is not authenticated or invalid user ID.", unauthorizedResult.Value);
    }

    [TestMethod]
    public async Task GetUserName_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId())
                        .Returns(Guid.NewGuid().ToString());
        _mockAccountRepo.Setup(repo => repo.GetUserName(It.IsAny<Guid>()))
                        .ReturnsAsync((string)null);

        // Act
        var result = await _controller.GetUserName();

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.AreEqual("User not found.", notFoundResult.Value);
    }
}
