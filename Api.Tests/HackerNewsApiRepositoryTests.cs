using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Api.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language;
using Moq.Protected;
using NUnit.Framework;

namespace Api.Tests
{
    public class HackerNewsApiRepositoryTests
    {
        private IConfiguration _configuration;
        private Mock<IHttpClientFactory> _mockFactory;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private ISetupSequentialResult<Task<HttpResponseMessage>> _sequentialSetup;
        private Mock<ILogger<HackerNewsApiRepository>> _mockLogger;
        
        [SetUp]
        public void Setup()
        {
            var configurationsVariables = new Dictionary<string, string>
            {
                {"HttpUserAgent", "API Client"},
                {"TopStoriesNumber", "1"},
                {"HackerNewsAPI:BestStoriesUrl", "http://127.0.0.1/"},
                {"HackerNewsAPI:ItemDetailUrl",  "http://127.0.0.1/"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationsVariables)
                .Build();

            _mockFactory = new Mock<IHttpClientFactory>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _sequentialSetup = _mockHttpMessageHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", 
                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());

            _mockLogger = new Mock<ILogger<HackerNewsApiRepository>>();
        }

        [Test]
        public async Task ReturnsTwentyTwoIdsWithOnlyTwoStories()
        {
            _sequentialSetup
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22]"),
                });

            SetArticlesMock();

            _mockFactory.Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(_mockHttpMessageHandler.Object));
            
            var repository = new HackerNewsApiRepository(_configuration, _mockFactory.Object, _mockLogger.Object);

            Assert.AreEqual((await repository.GetBestStories()).Count, 1);
        }
        
        [Test]
        public async Task ReturnsTwoIdsWithNoValidStories()
        {
            _sequentialSetup
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[10,11]"),
                });

            _mockFactory.Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(_mockHttpMessageHandler.Object));
            
            var repository = new HackerNewsApiRepository(_configuration, _mockFactory.Object, _mockLogger.Object);

            Assert.Null(await repository.GetBestStories(), null);
        }
        
        [Test]
        public async Task ReturnsZeroIdsWithTwoStories()
        {
            _sequentialSetup
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[]"),
                });

            SetArticlesMock();

            _mockFactory.Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(_mockHttpMessageHandler.Object));
            
            var repository = new HackerNewsApiRepository(_configuration, _mockFactory.Object, _mockLogger.Object);

            Assert.AreEqual(await repository.GetBestStories(), null);
        }

        [Test]
        public async Task GoodBestStoriesListResponseWithErrorResult()
        {
            _sequentialSetup
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            _mockFactory.Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(_mockHttpMessageHandler.Object));
            
            var repository = new HackerNewsApiRepository(_configuration, _mockFactory.Object, _mockLogger.Object);

            Assert.Null(await repository.GetBestStories(), null);
        }
        
        [Test]
        public async Task BadStoriesListRequest()
        {
            _sequentialSetup
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            _mockFactory.Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(_mockHttpMessageHandler.Object));
            
            var configuration = new ConfigurationBuilder()
                .Build();
            
            var repository = new HackerNewsApiRepository(configuration, _mockFactory.Object, _mockLogger.Object);

            Assert.Null(await repository.GetBestStories(), null);
        }
        
        [Test]
        public async Task BadStoriesListResponse()
        {
            _sequentialSetup
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("Bad Response")
                });

            _mockFactory.Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(_mockHttpMessageHandler.Object));
            
            var repository = new HackerNewsApiRepository(_configuration, _mockFactory.Object, _mockLogger.Object);

            Assert.Null(await repository.GetBestStories(), null);
        }
        
        [Test]
        public async Task GoodStoryDetailResponseWithErrorResult()
        {
            _sequentialSetup
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[1]")
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            _mockFactory.Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(_mockHttpMessageHandler.Object));
            
            var repository = new HackerNewsApiRepository(_configuration, _mockFactory.Object, _mockLogger.Object);

            Assert.AreEqual(await repository.GetBestStories(), null);
        }
        
        [Test]
        public async Task BadStoryDetailResponse()
        {
            _sequentialSetup
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[1]")
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("Bad Response"),
                });

            _mockFactory.Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(_mockHttpMessageHandler.Object));
            
            var repository = new HackerNewsApiRepository(_configuration, _mockFactory.Object, _mockLogger.Object);

            Assert.AreEqual(await repository.GetBestStories(), null);
        }
        
        private void SetArticlesMock()
        {
            _sequentialSetup
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"by\":\"user1\",\"descendants\":173,\"id\":1,\"kids\":[100,101],\"score\":946,\"time\":1606072791,\"title\":\"Result 1\",\"type\":\"story\",\"url\":\"https://example.com/\"}"),
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"by\":\"user2\",\"descendants\":173,\"id\":2,\"kids\":[200,201,202],\"score\":1254,\"time\":1606072741,\"title\":\"Result 2\",\"type\":\"story\",\"url\":\"https://example.com/\"}"),
                });
        }
    }
}