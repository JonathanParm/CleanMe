using Moq;
using Microsoft.AspNetCore.Identity;
using CleanMe.Domain.Entities;
using CleanMe.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace CleanMe.Tests
{
    public abstract class TestBase
    {
        protected readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        protected readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;

        protected TestBase()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                null, null, null, null);
        }
    }
}