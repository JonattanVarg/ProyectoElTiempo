using API.Controllers;
using API.Dtos.Identity;
using API.Dtos.Responses;
using API.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace API.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<SignInManager<AppUser>> _signInManagerMock;
        private readonly Mock<ILogger<AccountController>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(
                Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<AppUser>>(
                _userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<AppUser>>(),
                null, null, null, null);
            _loggerMock = new Mock<ILogger<AccountController>>();
            _configurationMock = new Mock<IConfiguration>();

            _controller = new AccountController(
                _userManagerMock.Object,
                _loggerMock.Object,
                _signInManagerMock.Object,
                _configurationMock.Object);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Required");

            var registerDto = new RegisterDto
            {
                FullName = "David Martinez",
                Password = "Password123!"
            };

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsConflict_WhenEmailAlreadyExists()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "existente@gmail.com",
                FullName = "Existente",
                Password = "Password123!"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new AppUser { Email = registerDto.Email });

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            var response = Assert.IsType<AuthResponseDto>(conflictResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Equal("El correo electrónico ya está en uso.", response.Message);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "juandiaz@gmail.com",
                FullName = "Juan Diaz",
                Password = "Password123!"
            };

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
             {
             }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((AppUser)null);

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal("Account created successfully", response.Message);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenUserNotFound()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "nonexistent@test.com",
                Password = "Password123!"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((AppUser)null);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var response = Assert.IsType<AuthResponseDto>(unauthorizedResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Equal("Credenciales inválidas", response.Message);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "juandiaz@gmail.com",
                Password = "Password123!"
            };

            var user = new AppUser { Email = loginDto.Email, FullName = "Juan Diaz" };

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            // Simulamos el inicio de sesión con contraseña
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), false, true))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Simulamos la obtención de roles del usuario
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
                .ReturnsAsync(new List<string> { "Reclutador" });

            // Simulamos la configuración de los valores JWT
            _configurationMock.Setup(x => x.GetSection(It.IsAny<string>()).Value)
                .Returns("ValorFicticio");


            // Simulamos la configuración de los valores JWT
            _configurationMock.Setup(x => x.GetSection("JWTSetting").GetSection("securityKey").Value)
                .Returns("YourStrongTestKey123YourStrongTestKey123");

            _configurationMock.Setup(x => x.GetSection("JWTSetting").GetSection("ValidIssuer").Value)
                .Returns("TestIssuer");

            _configurationMock.Setup(x => x.GetSection("JWTSetting").GetSection("ValidAudience").Value)
                .Returns("TestAudience");


            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Token);
            Assert.Equal("Inicio de sesión exitoso.", response.Message);
        }
    }
}
