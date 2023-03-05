using Microsoft.AspNetCore.Http;
using Moq;

namespace Films.UnitTests.ControllerTests;

[TestFixture]
public abstract class BaseControllerTests
{
    protected HttpContext? HttpContext;
    protected Mock<HttpRequest>? Request;

    [SetUp]
    public virtual void SetUp()
    {
        Request = new Mock<HttpRequest>();
        Request.Setup(x => x.Scheme).Returns("https");
        Request.Setup(x => x.Host).Returns(HostString.FromUriComponent("localhost:7153"));

        HttpContext = Mock.Of<HttpContext>(p => p.Request == Request.Object);
    }
    
    protected void SetRequestPath(string path)
    {
        Request.Setup(x => x.Path).Returns(PathString.FromUriComponent(path));
    }
}