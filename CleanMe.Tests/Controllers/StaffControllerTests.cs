//using CleanMe.Application.ViewModels;
//using CleanMe.Domain.Entities;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using Moq;
//using System.Collections.Generic;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Xunit;
//using CleanMe.Web.Controllers;
//using CleanMe.Shared.Models;
//using Xunit.Abstractions;
//using Microsoft.AspNetCore.Http;
//using CleanMe.Application.Interfaces;
//using Microsoft.Extensions.Logging;
//using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
//using CleanMe.Domain.Enums;

//namespace CleanMe.Web.Tests.Controllers
//{
//    public class StaffControllerTests
//    {
//        private readonly Mock<IRegionService> _staffServiceMock;
//        private readonly Mock<IUserService> _userServiceMock;
//        //private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
//        private readonly Mock<ITempDataDictionary> _tempDataMock;
//        private readonly Dictionary<string, object> _tempDataStorage = new Dictionary<string, object>();
//        private readonly Mock<ILogger<StaffController>> _loggerMock;
//        private readonly Mock<IErrorLoggingService> _errorLoggingServiceMock;

//        private readonly StaffController _controller;

//        private readonly ITestOutputHelper _output;

//        public StaffControllerTests(ITestOutputHelper output)
//        {
//            _staffServiceMock = new Mock<IRegionService>();
//            _userServiceMock = new Mock<IUserService>();

//            //_userManagerMock = new Mock<UserManager<ApplicationUser>>(
//                //Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
//            _errorLoggingServiceMock = new Mock<IErrorLoggingService>();

//            _tempDataMock = new Mock<ITempDataDictionary>();

//            _loggerMock = new Mock<ILogger<StaffController>>();

//            _controller = new StaffController(
//                _staffServiceMock.Object,
//                _userServiceMock.Object,
//                //_userManagerMock.Object,
//                _loggerMock.Object,
//                _errorLoggingServiceMock.Object)
//            {
//                TempData = _tempDataMock.Object
//            };

//            _output = output; // Capture test output
//        }

//        [Fact]
//        public async Task AddEdit_Post_ValidModel_CreatesNewStaff()
//        {
//            // Arrange
//            var model = new StaffViewModel
//            {
//                StaffId = 0, // Creating a new staff member
//                StaffNo = 123,
//                FirstName = "John",
//                FamilyName = "Doe",
//                Email = "newuser@example.com",
//                PhoneHome = "12345678",
//                PhoneMobile = "021987654",
//                IrdNumber = "123456",
//                BankAccount = "012 345 567 00",
//                PayrollId = "876",
//                WorkRole = WorkRole.Employee,
//                IsActive = true,
//                ShowPasswordSection = false
//            };

//            // Mock dependencies
//            MockAuthenticatedAdmin();
//            MockTempData();
//            MockUserManager();

//            _staffServiceMock.Setup(s => s.CreateStaffAsync(It.IsAny<StaffViewModel>(), It.IsAny<string>()))
//                             .ReturnsAsync(1); // Ensure a valid StaffId is returned

//            // Debugging Output
//            _output.WriteLine("DEBUG: Calling AddEdit...");

//            // Act
//            var result = await _controller.AddEdit(model) as RedirectToActionResult;

//            // Debugging Output
//            if (result == null)
//            {
//                _output.WriteLine("ERROR: AddEdit did not return a RedirectToActionResult.");
//            }

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal("Index", result.ActionName);

//            _output.WriteLine("DEBUG: Test completed successfully.");
//        }


//        [Fact]
//        public async Task AddEdit_Post_ValidModel_UpdatesExistingStaff()
//        {
//            // Arrange
//            var model = new StaffViewModel
//            {
//                StaffId = 1,
//                StaffNo = 321,
//                FirstName = "John",
//                FamilyName = "Doe",
//                Email = "updateduser@example.com",
//                ApplicationUserId = "user123",
//                PhoneHome = "12345678",
//                PhoneMobile = "021987654",
//                IrdNumber = "123456",
//                BankAccount = "012 345 567 00",
//                PayrollId = "876",
//                WorkRole = WorkRole.Employee,
//                IsActive = true,
//                ShowPasswordSection = false
//            };

//            // Mock dependencies
//            MockAuthenticatedAdmin();
//            MockTempData();
//            MockUserManager();

//            _staffServiceMock.Setup(s => s.UpdateStaffAsync(It.IsAny<StaffViewModel>(), It.IsAny<string>()))
//                             .Returns(Task.CompletedTask);

//            var user = new ApplicationUser
//            {
//                Id = "user123",
//                Email = "old@example.com",
//                UserName = "oldusername"
//            };

//            _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(user);
//            _userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<ApplicationUser>()))
//                            .ReturnsAsync(IdentityResult.Success);

//            // Debugging Output
//            _output.WriteLine("DEBUG: Calling AddEdit...");

//            // Act
//            var result = await _controller.AddEdit(model) as RedirectToActionResult;

//            // Debugging Output
//            if (result == null)
//            {
//                _output.WriteLine("ERROR: AddEdit did not return a RedirectToActionResult.");
//            }

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal("Index", result.ActionName);

//            _output.WriteLine("DEBUG: Test completed successfully.");
//        }


//        [Fact]
//        public async Task AddEdit_Post_CreateUserLogin_CreatesApplicationUser()
//        {
//            // Arrange
//            MockAuthenticatedAdmin(); // Ensure the user is authenticated as an admin
//            MockTempData(); // Ensure TempData is set
//            MockUserManager(); // Ensure UserManager is mocked

//            var model = new StaffViewModel
//            {
//                StaffId = 0, // Simulating new staff
//                StaffNo = 456,
//                FirstName = "John",
//                FamilyName = "Doe",
//                Email = "newuser@example.com",
//                Password = "TestPassword123!",
//                ChangePassword = new ChangePasswordViewModel { Password = "TestPassword123!", ConfirmPassword = "TestPassword123!" },
//                PhoneHome = "12345678",
//                PhoneMobile = "021987654",
//                IrdNumber = "123456",
//                BankAccount = "012 345 567 00",
//                PayrollId = "876",
//                WorkRole = WorkRole.Employee,
//                IsActive = true,
//                ShowPasswordSection = true
//            };

//            _staffServiceMock.Setup(s => s.CreateStaffAsync(It.IsAny<StaffViewModel>(), It.IsAny<string>()))
//                 .ReturnsAsync(1); // Simulates returning new StaffId, staff creation success

//            _userServiceMock.Setup(u => u.CreateUserLoginAsync(It.IsAny<string>(), It.IsAny<string>()))
//                            .ReturnsAsync((IdentityResult.Success, "newUserId123")); // Simulate success

//            // Act
//            _output.WriteLine("DEBUG: Calling AddEdit...");
//            var result = await _controller.AddEdit(model) as RedirectToActionResult;

//            // Debug logs to check execution
//            _output.WriteLine("DEBUG: Checking if AddEdit method returned a result...");
//            Assert.NotNull(result);
//            _output.WriteLine("DEBUG: AddEdit method returned successfully.");

//            Assert.Equal("Index", result.ActionName);
//            _output.WriteLine("DEBUG: Redirected to 'Index' as expected.");

//            // Verify that TempData contains the success message
//            _controller.TempData.TryGetValue("SuccessMessage", out var successMessage);
//            _output.WriteLine($"DEBUG: Retrieved TempData SuccessMessage: {successMessage ?? "[NULL]"}"); // Debug actual message

//            Assert.NotNull(successMessage);
//            _output.WriteLine("DEBUG: SuccessMessage is NOT NULL, test proceeding...");

//            var successMessageString = successMessage.ToString();
//            _output.WriteLine($"DEBUG: Expected: 'User login created successfully!'");
//            _output.WriteLine($"DEBUG: Actual: '{successMessageString}'");

//            Assert.Contains("User login created successfully!", successMessageString); // Ensures correct success message

//            _output.WriteLine("DEBUG: User creation success test completed successfully.");
//        }

//        [Fact]
//        public async Task AddEdit_Post_CreateUserLogin_Fails_ReturnsErrorMessage()
//        {
//            // Arrange
//            MockAuthenticatedAdmin(); // Ensures the user is authenticated as an admin
//            MockTempData(); // Ensures TempData is mocked
//            MockUserManager(failCreateUser: true); // Forces user creation failure

//            var model = new StaffViewModel
//            {
//                StaffId = 0, // Simulating new staff
//                StaffNo = 5,
//                FirstName = "John",
//                FamilyName = "Doe",
//                Email = "newuser@example.com",
//                Password = "TestPassword123!",
//                ChangePassword = new ChangePasswordViewModel { Password = "TestPassword123!",  ConfirmPassword = "TestPassword123!" },
//                PhoneHome = "12345678",
//                PhoneMobile = "021987654",
//                IrdNumber = "123456",
//                BankAccount = "012 345 567 00",
//                PayrollId = "876",
//                WorkRole = WorkRole.Employee,
//                IsActive = true,
//                ShowPasswordSection = true
//            };

//            _staffServiceMock.Setup(s => s.CreateStaffAsync(It.IsAny<StaffViewModel>(), It.IsAny<string>()))
//                     .ReturnsAsync(1); // Simulate staff creation success

//            _userServiceMock.Setup(u => u.CreateUserLoginAsync(It.IsAny<string>(), It.IsAny<string>()))
//                            .ReturnsAsync((IdentityResult.Failed(new IdentityError { Description = "User creation failed" }), null)); // ✅ Simulate failure

//            // Act
//            _output.WriteLine("DEBUG: Calling AddEdit...");
//            var result = await _controller.AddEdit(model) as RedirectToActionResult;

//            // Debug logs to check execution
//            _output.WriteLine("DEBUG: Checking if AddEdit method returned a result...");
//            Assert.NotNull(result);
//            _output.WriteLine("DEBUG: AddEdit method returned successfully.");

//            Assert.Equal("Index", result.ActionName);
//            _output.WriteLine("DEBUG: Redirected to 'Index' as expected.");

//            // Verify that TempData contains the error message
//            _controller.TempData.TryGetValue("ErrorMessage", out var errorMessage);
//            _output.WriteLine($"DEBUG: Retrieved TempData ErrorMessage: {errorMessage ?? "[NULL]"}"); // Debug actual message

//            Assert.NotNull(errorMessage);
//            _output.WriteLine("DEBUG: ErrorMessage is NOT NULL, test proceeding...");

//            var errorMessageString = errorMessage.ToString();
//            _output.WriteLine($"DEBUG: Expected: 'Failed to create user login.'");
//            _output.WriteLine($"DEBUG: Actual: '{errorMessageString}'");

//            Assert.Contains("Failed to create user login.", errorMessageString); // Ensures correct error message

//            _output.WriteLine("DEBUG: User creation failure test completed successfully.");
//        }

//        [Fact]
//        public async Task AddEdit_Post_UpdatePassword_Success()
//        {
//            // Arrange
//            var model = new StaffViewModel
//            {
//                StaffId = 1, // Updating an existing staff
//                StaffNo = 987,
//                FirstName = "John",
//                FamilyName = "Doe",
//                Email = "updateduser@example.com",
//                ApplicationUserId = "user123",
//                Password = "NewSecurePass1!",
//                PhoneHome = "12345678",
//                PhoneMobile = "021987654",
//                IrdNumber = "123456",
//                BankAccount = "012 345 567 00",
//                PayrollId = "876",
//                WorkRole = WorkRole.Employee,
//                IsActive = true,
//                ShowPasswordSection = false
//            };

//            // Mock dependencies
//            MockAuthenticatedAdmin();
//            MockTempData();
//            MockUserManager();

//            _staffServiceMock.Setup(s => s.UpdateStaffAsync(It.IsAny<StaffViewModel>(), It.IsAny<string>()))
//                             .Returns(Task.CompletedTask);

//            var user = new ApplicationUser
//            {
//                Id = "user123",
//                Email = "old@example.com",
//                UserName = "oldusername"
//            };

//            _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(user);
//            _userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<ApplicationUser>()))
//                            .ReturnsAsync(IdentityResult.Success);

//            // Debugging Output
//            _output.WriteLine("DEBUG: Calling AddEdit...");

//            // Act
//            var result = await _controller.AddEdit(model) as RedirectToActionResult;

//            // Debugging Output
//            if (result == null)
//            {
//                _output.WriteLine("ERROR: AddEdit did not return a RedirectToActionResult.");
//            }

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal("Index", result.ActionName);
//            _output.WriteLine("DEBUG: Test completed successfully.");
//        }

//        [Fact]
//        public async Task AddEdit_Post_UpdatePassword_Fails_ReturnsErrorMessage()
//        {
//            // Arrange
//            var model = new StaffViewModel
//            {
//                StaffId = 1,
//                StaffNo = 456,
//                FirstName = "John",
//                FamilyName = "Doe",
//                Email = "updateduser@example.com",
//                ApplicationUserId = "user123",
//                Password = "SecurePass1!",
//                PhoneHome = "12345678",
//                PhoneMobile = "021987654",
//                IrdNumber = "123456",
//                BankAccount = "012 345 567 00",
//                PayrollId = "876",
//                WorkRole = WorkRole.Employee,
//                IsActive = true,
//                ShowPasswordSection = false
//            };

//            MockAuthenticatedAdmin();
//            MockTempData(); // Ensure TempData is properly mocked
//            MockUserManager(failPasswordReset: true); // Forces password reset failure

//            _staffServiceMock.Setup(s => s.UpdateStaffAsync(It.IsAny<StaffViewModel>(), It.IsAny<string>()))
//                             .Returns(Task.CompletedTask);

//            var result = await _controller.AddEdit(model) as RedirectToActionResult;

//            Assert.NotNull(result);
//            Assert.Equal("Index", result.ActionName);

//            // Print TempData["ErrorMessage"] before asserting
//            _controller.TempData.TryGetValue("ErrorMessage", out var errorMessage);

//            _output.WriteLine($"DEBUG: Test received TempData ErrorMessage: {errorMessage}");

//            Assert.NotNull(errorMessage);
//            var errorMessageString = errorMessage.ToString();

//            Assert.Contains("Failed to update password.", errorMessageString);
//            Assert.Contains("Password must meet complexity requirements.", errorMessageString);
//            Assert.Contains("Password and confirmation do not match.", errorMessageString);

//            _output.WriteLine("DEBUG: Password reset failure test completed successfully.");
//        }

//        [Fact]
//        public async Task IsEmailAvailable_ValidEmail_ReturnsTrue()
//        {
//            // Arrange
//            _staffServiceMock.Setup(s => s.IsEmailAvailableAsync("new@example.com", 0))
//                             .ReturnsAsync(true);

//            // Act
//            var result = await _controller.IsEmailAvailable("new@example.com", 0) as JsonResult;

//            // Assert
//            Assert.NotNull(result);
//            Assert.NotNull(result.Value);
//            Assert.True((bool)result.Value);
//        }

//        [Fact]
//        public async Task IsEmailAvailable_EmailAlreadyTaken_ReturnsFalse()
//        {
//            // Arrange
//            _staffServiceMock.Setup(s => s.IsEmailAvailableAsync("taken@example.com", 0))
//                             .ReturnsAsync(false);

//            // Act
//            var result = await _controller.IsEmailAvailable("taken@example.com", 0) as JsonResult;

//            // Assert
//            Assert.NotNull(result);
//            Assert.NotNull(result.Value);
//            Assert.False((bool)result.Value);
//        }

//        private void MockUserManager(
//                string userId = "admin123",
//                string email = "admin@example.com",
//                string role = "Admin",
//                bool failCreateUser = false,
//                bool failPasswordReset = false)
//        {
//            var adminUser = new ApplicationUser
//            {
//                Id = userId,
//                Email = email,
//                UserName = "AdminUser"
//            };

//            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
//                            .ReturnsAsync(adminUser);

//            _userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
//                            .ReturnsAsync(adminUser); // Ensure a user is returned

//            _userManagerMock.Setup(u => u.GetRolesAsync(adminUser))
//                            .ReturnsAsync(new List<string> { role });

//            _userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<ApplicationUser>()))
//                            .ReturnsAsync(IdentityResult.Success);

//            _userManagerMock.Setup(u => u.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()))
//                            .ReturnsAsync("mock-token");

//            if (failCreateUser)
//            {
//                Console.WriteLine("DEBUG: Simulating failed user creation.");
//                _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
//                                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed." }));
//            }
//            else
//            {
//                _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
//                                .ReturnsAsync(IdentityResult.Success);
//            }

//            if (failPasswordReset)
//            {
//                Console.WriteLine("DEBUG: Simulating failed password reset.");

//                _userManagerMock.Setup(u => u.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
//                                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "InvalidPassword", Description = "Password must meet complexity requirements." },
//                                                                    new IdentityError { Code = "Mismatch", Description = "Password and confirmation do not match." }));
//            }
//            else
//            {
//                _userManagerMock.Setup(u => u.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
//                                .ReturnsAsync(IdentityResult.Success);
//            }
//        }

//        private void MockAuthenticatedAdmin()
//        {
//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.Name, "admin@example.com"),
//                new Claim(ClaimTypes.NameIdentifier, "admin123"), // Required for authorization policies
//                new Claim(ClaimTypes.Role, "Admin") // Ensures admin role is set
//            };

//            var identity = new ClaimsIdentity(claims, "TestAuthType");
//            var user = new ClaimsPrincipal(identity);

//            var httpContext = new DefaultHttpContext { User = user };

//            _controller.ControllerContext = new ControllerContext
//            {
//                HttpContext = httpContext
//            };

//            _output.WriteLine($"DEBUG: MockAuthenticatedAdmin executed. User Authenticated: {httpContext.User.Identity.IsAuthenticated}");
//        }


//        //private void MockAuthenticatedAdmin()
//        //{
//        //    var adminUser = new ApplicationUser
//        //    {
//        //        Id = "admin123",
//        //        Email = "admin@example.com",
//        //        UserName = "AdminUser"
//        //    };

//        //    // Mock UserManager to return an authenticated Admin user
//        //    _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
//        //                    .ReturnsAsync(adminUser);

//        //    _userManagerMock.Setup(u => u.FindByIdAsync("admin123"))
//        //                    .ReturnsAsync(adminUser);

//        //    _userManagerMock.Setup(u => u.GetRolesAsync(adminUser))
//        //                    .ReturnsAsync(new List<string> { "Admin" });

//        //    // Mock HttpContext & Authentication for Admin Role
//        //    var claims = new List<Claim>
//        //    {
//        //        new Claim(ClaimTypes.NameIdentifier, "admin123"),
//        //        new Claim(ClaimTypes.Name, "Admin User"),
//        //        new Claim(ClaimTypes.Role, "Admin") // Ensures user is an Admin
//        //    };

//        //    var identity = new ClaimsIdentity(claims, "TestAuthType");
//        //    var claimsPrincipal = new ClaimsPrincipal(identity);

//        //    _controller.ControllerContext = new ControllerContext
//        //    {
//        //        HttpContext = new DefaultHttpContext { User = claimsPrincipal }
//        //    };
//        //}

//        private void MockTempData()
//        {
//            var tempDataStorage = new Dictionary<string, object>(); // In-memory storage for TempData
//            var tempDataMock = new Mock<ITempDataDictionary>();

//            tempDataMock.Setup(t => t.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
//                        .Returns((string key, out object value) =>
//                        {
//                            bool exists = tempDataStorage.TryGetValue(key, out var storedValue);
//                            value = storedValue;
//                            return exists;
//                        });

//            tempDataMock.Setup(t => t[It.IsAny<string>()])
//                        .Returns((string key) => tempDataStorage.ContainsKey(key) ? tempDataStorage[key] : null);

//            tempDataMock.SetupSet(t => t[It.IsAny<string>()] = It.IsAny<object>())
//                        .Callback((string key, object value) => tempDataStorage[key] = value);

//            _controller.TempData = tempDataMock.Object;
//        }
//    }
//}