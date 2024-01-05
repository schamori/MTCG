using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestMITCG
{
    internal class TestAuthenticationService
    {
        [Test]
        public void AuthenticateUser_WithValidToken_ReturnsExpectedUser()
        {
            // arrange
            var userManager = new Mock<IUserManager>();

            var user = new User(1, "test", "psd", 20, 100, false);
            var expectedResponse = user;

            userManager.Setup(um => um.GetUserByAuthToken("testToken")).Returns(user);

            IAuthenticationService _authenticationService = new AuthenticationService(userManager.Object);
            var actualResponse = _authenticationService.AuthenticateUser("testToken", false);

            // assert
            Assert.That(actualResponse, Is.EqualTo(expectedResponse));
        }

        [Test]
        public void AuthenticateAdminUser_WithValidAdminToken_ReturnsExpectedUser()
        {
            // arrange
            var userManager = new Mock<IUserManager>();

            var user = new User(1, "test", "psd", 20, 100, true);
            var expectedResponse = user;

            userManager.Setup(um => um.GetUserByAuthToken("testToken")).Returns(user);

            IAuthenticationService _authenticationService = new AuthenticationService(userManager.Object);
            var actualResponse = _authenticationService.AuthenticateUser("testToken", true);

            // assert
            Assert.That(actualResponse, Is.EqualTo(expectedResponse));
        }
        [Test]

        public void AuthenticateAdminUser_WithUserToken_ReturnsException()
        {
            // arrange
            var userManager = new Mock<IUserManager>();

            var user = new User(1, "testToken", "psd", 20, 100, false);

            userManager.Setup(um => um.GetUserByAuthToken("testToken")).Returns(user);
            IAuthenticationService _authenticationService = new AuthenticationService(userManager.Object);

            try
            {
                _authenticationService.AuthenticateUser("testToken", true);
                Assert.Fail("Expected AccessTokenException was not thrown.");
            }
            catch (AccessTokenException ex)
            {
                Assert.IsTrue(ex.NeedsAdmin, "The exception should indicates that a user was found but a user with admin user is needed.");
            }
            catch (Exception ex)
            {
                // If a different type of exception is thrown, fail the test
                Assert.Fail($"Unexpected exception type: {ex.GetType()}");
            }
        }
        [Test]

        public void AuthenticateUser_WithInValidToken_ReturnsException()
        {
            // arrange
            var userManager = new Mock<IUserManager>();

            var user = new User(1, "testToken", "psd", 20, 100, false);
            var expectedResponse = user;

            userManager.Setup(um => um.GetUserByAuthToken("testToken")).Returns(user);
            IAuthenticationService _authenticationService = new AuthenticationService(userManager.Object);

            try
            {
                _authenticationService.AuthenticateUser("invalidtestToken", false);
                Assert.Fail("Expected AccessTokenException was not thrown.");
            }
            catch (AccessTokenException ex)
            {
                Assert.IsFalse(ex.NeedsAdmin, "The exception  indicates that the token was not found.");
            }
            catch (Exception ex)
            {
                // If a different type of exception is thrown, fail the test
                Assert.Fail($"Unexpected exception type: {ex.GetType()}");
            }
        }

    }

}

