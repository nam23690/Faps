using FAP.Test.Integration.Helpers;
using FluentAssertions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace FAP.Test.Integration.API
{
    public class UsersApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public UsersApiTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetProfile_WithValidJwt_ReturnsOk()
        {
            // Arrange
            var token = JwtTokenHelper.GenerateTestJwt("HCM", "GetUserByIdQuery,CreateUserCommand,GetUsersQuery,UpdateUserCommand,DeleteUserCommand");
            var client = _factory.CreateClientWithJwt(token);

            // Act
            var response = await client.GetAsync("/api/user");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetProfile_WithoutJwt_ReturnsUnauthorized()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/user");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
