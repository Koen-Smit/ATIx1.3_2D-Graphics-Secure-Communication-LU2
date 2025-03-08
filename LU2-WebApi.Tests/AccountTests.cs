using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
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
        _mockUserManager = new Mock<UserManager<AppUser>>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _controller = new AccountController(_mockAccountRepo.Object, _mockAuthService.Object, _mockUserManager.Object);
    }

    // Unit test: Controleert of de registratie succesvol is en een OK-status retourneert.
    [TestMethod]
    public async Task Register_ReturnsOk_WhenRegistrationSucceeds()
    {
        var request = new AccountRequest { UserName = "testuser", Password = "Test@12345" };
        _mockAccountRepo.Setup(repo => repo.RegisterUser(request))
                        .ReturnsAsync(Result.Success("Registration successful"));

        var result = await _controller.Register(request);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual("Registration successful", okResult.Value);
    }

    // Unit test: Controleert of een mislukte registratie een BadRequest retourneert.
    [TestMethod]
    public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
    {
        var request = new AccountRequest { UserName = "testuser", Password = "Test@12345" };
        _mockAccountRepo.Setup(repo => repo.RegisterUser(request))
                        .ReturnsAsync(Result.Failure("Error occurred"));

        var result = await _controller.Register(request);
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.AreEqual("Error occurred", badRequestResult.Value);
    }

    // Unit test: Controleert of uitloggen succesvol is en een OK-status retourneert.
    [TestMethod]
    public async Task Logout_ReturnsOk_WhenLogoutSucceeds()
    {
        _mockAccountRepo.Setup(repo => repo.LogoutUser())
                        .ReturnsAsync(Result.Success("Logout successful"));

        var result = await _controller.Logout();
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual("Logout successful", okResult.Value);
    }

    // Unit test: Controleert of een ongeauthenticeerde gebruiker geen gebruikersnaam kan opvragen.
    [TestMethod]
    public async Task GetUserName_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
    {
        _mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId())
                        .Returns((string)null!);

        var result = await _controller.GetUserName();
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
        Assert.AreEqual("User is not authenticated or invalid user ID.", unauthorizedResult.Value);
    }

    // Unit test: Controleert of een niet-bestaande gebruiker een NotFound-status retourneert.
    [TestMethod]
    public async Task GetUserName_ReturnsNotFound_WhenUserDoesNotExist()
    {
        _mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId())
                        .Returns(Guid.NewGuid().ToString());
        _mockAccountRepo.Setup(repo => repo.GetUserName(It.IsAny<Guid>()))
                        .ReturnsAsync((string)null!);

        var result = await _controller.GetUserName();
        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.AreEqual("User not found.", notFoundResult.Value);
    }
}
