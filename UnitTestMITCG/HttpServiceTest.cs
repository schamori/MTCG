using MTCG.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestMITCG
{
    internal class HttpServiceTest
    {
        private IHttpService _httpService;
        [SetUp]
        public void Setup()
        {

            var mockController = new Mock<ControllerBase>();

            _httpService = new HttpService(new Mock<ControllerBase>().Object, new Mock<ControllerBase>().Object,
            new Mock<ControllerBase>().Object, new Mock<ControllerBase>().Object, new Mock<ControllerBase>().Object);


        }
        static object[] CreateHttpRequests =
        {
            new object[]
            {
                @"POST /users HTTP/1.1
                Host: localhost:10001
                User-Agent: curl/8.0.1
                Accept: */*
                Content-Type: application/json
                Content-Length: 44

                {""Username"": ""kienboec"", ""Password"": ""daniel""}
                ",
                new HttpRequest(HttpMethod.Post, "/users", content: @"{""Username"": ""kienboec"", ""Password"": ""daniel""}")
            },
            new object[]
            {
                @"PUT /users/kienboec HTTP/1.1
                Host: localhost:10001
                User-Agent: curl/8.0.1
                Accept: */*
                Content-Type: application/json
                Authorization: Bearer altenhof-mtcgToken
                Content-Length: 56

                {""Name"": ""Hoax"", ""Bio"": ""me playin..."", ""Image"":"":-)""}
                "
                ,
                new HttpRequest(HttpMethod.Put, "/users", "kienboec", "altenhof-mtcgToken", @"{""Name"": ""Hoax"", ""Bio"": ""me playin..."", ""Image"":"":-)""}")
            },
            new object[]{
                @"GET /users/altenhof HTTP/1.1
                Host: localhost:10001
                User-Agent: curl/8.0.1
                Accept: */*
                Authorization: Bearer kienboec-mtcgToken",
                new HttpRequest(HttpMethod.Get, "/users", "altenhof", "kienboec-mtcgToken")
            },
            new object[]
            {
                @"GET /tradings HTTP/1.1
                Host: localhost:10001
                User-Agent: curl/8.0.1
                Accept: */*",
                new HttpRequest(HttpMethod.Get, "/tradings")
            }
        };

        [Test]
        [TestCaseSource(nameof(CreateHttpRequests))]
        public void TestCreatePackages(string requestString, HttpRequest httpRequest)
        {


            Assert.That(_httpService.Parse(requestString), Is.EqualTo(httpRequest));
        }

        static object[] HttpMethodCases =
        {
            new object[] { "get", HttpMethod.Get },
            new object[] { "post", HttpMethod.Post },
            new object[] { "put", HttpMethod.Put },
            new object[] { "delete", HttpMethod.Delete },
            new object[] { "patch", HttpMethod.Patch },
        };

        [Test]
        [TestCaseSource(nameof(HttpMethodCases))]
        public void GetMethod_ValidMethods_ReturnsCorrectHttpMethod(string method, HttpMethod expectedMethod)
        {
            var result = _httpService.GetMethod(method);

            Assert.That(result, Is.EqualTo(expectedMethod));
        }

        [Test]
        public void GetMethod_InvalidMethod_ThrowsInvalidDataException()
        {
            // Act & Assert
            Assert.Throws<InvalidDataException>(() => _httpService.GetMethod("invalidMethod"));
        }
    }

}

