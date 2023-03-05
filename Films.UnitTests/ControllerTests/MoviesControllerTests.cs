using System.Net;
using Films.Controllers;
using Films.Core;
using Films.Core.Domain;
using Films.Core.Domain.Filters;
using Films.Core.Domain.Pagination;
using Films.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Films.UnitTests.ControllerTests;

public class MoviesControllerTests : BaseControllerTests
{
    private Mock<IUnitOfWork> _unitOfWork;
    private Mock<IMovieRepository> _repository;
    private MoviesController _controller;

    public override void SetUp()
    {
        base.SetUp();

        _unitOfWork = new Mock<IUnitOfWork>();
        _repository = new Mock<IMovieRepository>();

        _unitOfWork.Setup(s => s.Movies).Returns(_repository.Object);

        _controller = new MoviesController(_unitOfWork.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = HttpContext
            }
        };
    }

    [Test]
    public void Add_WhenItemCreated_ReturnsCreatedResponse()
    {
        // Arrange
        SetRequestPath("/api/Movies");
        var movie = new Movie { Name = "a", Description = "b", };

        // Act
        var result = _controller.Add(movie) as CreatedResult;

        // Assert
        _repository.Verify(a => a.Add(movie));
        _unitOfWork.Verify(a => a.SaveChanges());

        Assert.That(result, Is.TypeOf<CreatedResult>());
        var value = result.Value as OperationResult;

        Assert.Multiple(() =>
        {
            Assert.That(value, Is.TypeOf<OperationResult>());
            Assert.That(value.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
            Assert.That(result.Location, Is.Not.Null);
        });
    }

    [Test]
    public void Update_WhenItemUpdated_ReturnsSuccessResponse()
    {
        // Arrange
        SetRequestPath($"/api/Movies/1");
        _repository.Setup(s => s.SingleOrDefault(p => p.Id == 1)).Returns(new Movie());
        var movie = new Movie { Id = 1, Name = "a", Description = "b", };

        // Act
        var result = _controller.Update(movie, 1) as OkObjectResult;

        // Assert
        _unitOfWork.Verify(a => a.SaveChanges());

        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var value = result.Value as OperationResult;

        Assert.Multiple(() =>
        {
            Assert.That(value, Is.TypeOf<OperationResult>());
            Assert.That(value.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        });
    }

    [Test]
    public void Update_WhenItemNotFound_ReturnsNotFoundResponse()
    {
        // Arrange
        SetRequestPath($"/api/Movies/1");
        var movie = new Movie { Id = 1, Name = "a", Description = "b", };

        // Act
        var result = _controller.Update(movie, 1) as NotFoundResult;

        // Assert
        _unitOfWork.Verify(a => a.SaveChanges(), Times.Never);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
        Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
    }

    [Test]
    public void Update_WhenIdsAreDifferent_ReturnsBadRequest()
    {
        // Arrange
        SetRequestPath($"/api/Movies/1");
        var movie = new Movie { Id = 1, Name = "a", Description = "b", };

        // Act
        var result = _controller.Update(movie, 2) as BadRequestResult;

        // Assert
        _unitOfWork.Verify(a => a.SaveChanges(), Times.Never());

        Assert.That(result, Is.TypeOf<BadRequestResult>());
        Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
    }

    [Test]
    public void GetById_WhenItemNotFound_ReturnsNotFound()
    {
        // Arrange
        SetRequestPath($"/api/Movies/1");

        // Act
        var result = _controller.GetById(1) as NotFoundResult;

        // Assert
        _unitOfWork.Verify(a => a.SaveChanges(), Times.Never());

        Assert.That(result, Is.TypeOf<NotFoundResult>());
        Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
    }

    [Test]
    public void GetById_WhenItemFound_ReturnsItem()
    {
        // Arrange
        SetRequestPath($"/api/Movies/1");
        _repository.Setup(s => s.SingleOrDefault(p => p.Id == 1)).Returns(new Movie { Id = 1 });

        // Act
        var result = _controller.GetById(1) as OkObjectResult;

        // Assert
        _unitOfWork.Verify(a => a.SaveChanges(), Times.Never());

        var value = result.Value as OperationResult;
        Assert.That(value, Is.TypeOf<OperationResult>());
        
        var movie = value.Data as Movie;
        
        Assert.Multiple(() =>
        {
            Assert.That(movie, Is.TypeOf<Movie>());
            Assert.That(value.IsSuccess, Is.True);
            Assert.That(movie.Id, Is.EqualTo(1));
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        });
    }
    
    [Test]
    public void GetById_WhenMoreThanOneItemFound_ThrowsAnException()
    {
        // Arrange
        SetRequestPath($"/api/Movies/1");
        _repository.Setup(s => s.SingleOrDefault(p => p.Id == 1)).Throws<Exception>();

        //Act
        TestDelegate action = () =>
        {
            _controller.GetById(1);
        };
        
        // Assert
        Assert.Throws<Exception>(action);
    }
    
    [Test]
    public void Delete_ItemNotFound_ReturnsNotFoundResponse()
    {
        // Arrange
        SetRequestPath($"/api/Movies/1");

        // Act
        var result = _controller.Delete(1) as NotFoundResult;

        // Assert
        _unitOfWork.Verify(a => a.SaveChanges(), Times.Never());

        Assert.That(result, Is.TypeOf<NotFoundResult>());
        Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
    }

    [Test]
    public void Delete_ItemFound_TheItemIsDeleted()
    {
        // Arrange
        SetRequestPath($"/api/Movies/1");
        var movie = new Movie();
        _repository.Setup(s => s.SingleOrDefault(p => p.Id == 1)).Returns(movie);

        // Act
        var result = _controller.Delete(1) as OkObjectResult;

        // Assert
        _repository.Verify(a => a.Remove(movie));
        _unitOfWork.Verify(a => a.SaveChanges());

        Assert.That(result, Is.TypeOf<OkObjectResult>());
        Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        var value = result.Value as OperationResult;
        Assert.That(value.IsSuccess, Is.True);
    }
    
    [Test]
    public void Get_TheResultIsPaginated()
    {
        // Arrange
        SetRequestPath($"/api/Movies");
        var movies = new List<Movie>();
        var paginatedList = new PaginatedList<Movie>(movies.AsReadOnly(), new PaginationInfo(1, 10, 1, 1)); 
        _repository.Setup(s => s.ToPagedList(It.IsAny<MovieFilter>())).Returns(paginatedList);

        // Act
        var result = _controller.Get() as OkObjectResult;

        // Assert
        _repository.Verify(a => a.ToPagedList(It.IsAny<MovieFilter>()));
        
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        
        var value = result.Value as OperationResult;
        Assert.That(value.IsSuccess, Is.True);
        
        var data = value.Data as PaginatedList<Movie>;
        Assert.That(data, Is.TypeOf<PaginatedList<Movie>>());
    }
}