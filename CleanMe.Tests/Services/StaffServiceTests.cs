//// Namespace: CleanMe.Tests.Services
//using Xunit;
//using Moq;
//using CleanMe.Application.Services;
//using CleanMe.Application.Interfaces;
//using CleanMe.Domain.Interfaces;
//using CleanMe.Application.ViewModels;
//using CleanMe.Domain.Entities;
//using Microsoft.Extensions.Logging;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System;

//namespace CleanMe.Tests.Services
//{
//    public class StaffServiceTests
//    {
//        private readonly Mock<IUnitOfWork> _mockUow = new Mock<IUnitOfWork>();
//        private readonly Mock<IRepository<Staff>> _mockRepo = new Mock<IRepository<Staff>>();
//        private readonly Mock<IStaffRepository> _mockStaffRepo = new Mock<IStaffRepository>();
//        private readonly Mock<IDapperRepository> _mockDapper = new Mock<IDapperRepository>();
//        private readonly Mock<IUserService> _mockUserService = new Mock<IUserService>();
//        private readonly Mock<ILogger<StaffService>> _mockLogger = new Mock<ILogger<StaffService>>();

//        public StaffService(
//    IUnitOfWork unitOfWork,
//    IRepository<Staff> staffRepo,
//    IStaffRepository staffRepository,
//    IDapperRepository dapperRepository,
//    IUserService userService,
//    ILogger<StaffService> logger)


//        [Fact]
//        public async Task AddStaffAsync_ShouldAddStaff_WhenValid()
//        {
//            var model = new StaffViewModel { StaffNo = 101, FirstName = "John", FamilyName = "Smith" };
//            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Staff>())).Returns(Task.CompletedTask);
//            _mockUow.Setup(u => u.CommitAsync()).ReturnsAsync(1);

//            var service = CreateService();
//            var id = await service.AddStaffAsync(model, "user-1");

//            Assert.True(id >= 0);
//            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Staff>()), Times.Once);
//            _mockUow.Verify(u => u.CommitAsync(), Times.Once);
//        }

//        [Fact]
//        public async Task UpdateStaffAsync_ShouldUpdate_WhenStaffExists()
//        {
//            var model = new StaffViewModel { StaffId = 1, StaffNo = 101, FirstName = "Jane", FamilyName = "Doe" };
//            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Staff());
//            _mockUow.Setup(u => u.CommitAsync()).ReturnsAsync(1);

//            var service = CreateService();
//            await service.UpdateStaffAsync(model, "user-2");

//            _mockRepo.Verify(r => r.Update(It.IsAny<Staff>()), Times.Once);
//            _mockUow.Verify(u => u.CommitAsync(), Times.Once);
//        }

//        [Fact]
//        public async Task SoftDeleteStaffAsync_ShouldSetIsDeleted()
//        {
//            var staff = new Staff { staffId = 1, IsDeleted = false };
//            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(staff);
//            _mockUow.Setup(u => u.CommitAsync()).ReturnsAsync(1);

//            var service = CreateService();
//            var result = await service.SoftDeleteStaffAsync(1, "admin");

//            Assert.True(result);
//            Assert.True(staff.IsDeleted);
//            Assert.False(staff.IsActive);
//        }

//        [Theory]
//        [InlineData("", "", "")]
//        [InlineData(null, null, null)]
//        public async Task AddStaffAsync_ShouldThrow_WhenNoContactProvided(string home, string mobile, string email)
//        {
//            var model = new StaffViewModel
//            {
//                StaffNo = 101,
//                FirstName = "John",
//                FamilyName = "Doe",
//                PhoneHome = home,
//                PhoneMobile = mobile,
//                Email = email
//            };

//            var service = CreateService();

//            var exception = await Assert.ThrowsAsync<Exception>(() => service.AddStaffAsync(model, "user"));
//            Assert.Contains("contact", exception.Message, StringComparison.OrdinalIgnoreCase);
//        }

//        [Fact]
//        public async Task IsEmailAvailableAsync_ShouldReturnFalse_WhenEmailExists()
//        {
//            _mockDapper.Setup(d => d.ExecuteScalarAsync<int>(It.IsAny<string>(), It.IsAny<object>()))
//                .ReturnsAsync(1);

//            var service = CreateService();
//            var result = await service.IsEmailAvailableAsync("existing@email.com", null);

//            Assert.False(result);
//        }
//    }
//}
