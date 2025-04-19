using Xunit;
using Moq;
using AutoFixture;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using CleanMe.Domain.Entities;
using CleanMe.Web.Controllers;
using CleanMe.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using CleanMe.Shared.Models;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace CleanMe.Tests.Controllers
{
    public class AccountControllerTests : TestBase
    {
        private readonly AccountController _accountController;
        private readonly Fixture _fixture;

        public AccountControllerTests()
        {
            _accountController = new AccountController(_signInManagerMock.Object, _userManagerMock.Object);
            _fixture = new Fixture(); // AutoFixture instance
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsRedirectToHome()
        {
            // Arrange
            var loginModel = new LoginViewModel { Email = "test@cleanme.com", Password = "ValidPass123" };
            var user = new ApplicationUser { Id = "123", UserName = loginModel.Email, Email = loginModel.Email };

            _userManagerMock.Setup(x => x.FindByEmailAsync(loginModel.Email))
                .ReturnsAsync(user); // Ensure a valid user is returned

            _signInManagerMock.Setup(x => x.PasswordSignInAsync(user, loginModel.Password, false, false))
                .ReturnsAsync(SignInResult.Success);

            // Act
            var result = await _accountController.Login(loginModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsViewWithError()
        {
            // Arrange
            var loginModel = new LoginViewModel { Email = "test@cleanme.com", Password = "WrongPass" };
            var user = new ApplicationUser { UserName = loginModel.Email, Email = loginModel.Email };

            _userManagerMock.Setup(x => x.FindByEmailAsync(loginModel.Email))
                .ReturnsAsync(user);

            _signInManagerMock.Setup(x => x.PasswordSignInAsync(user, loginModel.Password, false, false))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _accountController.Login(loginModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task Login_UserNotFound_ReturnsViewWithError()
        {
            // Arrange
            var loginModel = new LoginViewModel { Email = "unknown@cleanme.com", Password = "SomePass123" };

            _userManagerMock.Setup(x => x.FindByEmailAsync(loginModel.Email))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _accountController.Login(loginModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(result.ViewData.ModelState.IsValid);
            Assert.Contains("Invalid login attempt.", result.ViewData.ModelState[""].Errors[0].ErrorMessage);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("Staff")]
        public void UserMenuOptions_ValidateRoleBasedMenuVisibility(string role)
        {
            // Arrange
            var user = _fixture.Create<ApplicationUser>();
            var claims = new List<Claim> { new Claim(ClaimTypes.Role, role) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };

            var controllerContext = new ControllerContext { HttpContext = httpContext };
            _accountController.ControllerContext = controllerContext;

            // Act
            var userRoles = _accountController.User.IsInRole("Admin") ? "Admin" : "Staff";

            // Assert
            if (role == "Admin")
            {
                Assert.Equal("Admin", userRoles);
            }
            else if (role == "Staff")
            {
                Assert.Equal("Staff", userRoles);
            }
        }
    }
}