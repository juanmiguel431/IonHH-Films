using Films.Core;
using Films.Core.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Films.Controllers;

public class MoviesController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;

    public MoviesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        var movies = _unitOfWork.Movies.GetAll();

        var result = OperationResult.CreateSuccessMessage(movies);
        return Ok(result);
    }
    
    [HttpPost]
    public IActionResult Add(Movie movie)
    {
        movie.CreatedDate = DateTime.UtcNow;
        _unitOfWork.Movies.Add(movie);

        _unitOfWork.SaveChanges();

        var result = OperationResult.CreateSuccessMessage(movie);

        var uri = new Uri(GetAbsoluteUri(movie.Id.ToString()));
        return Created(uri, result);
    }

    [HttpPatch("{id:long}")]
    public IActionResult Update(Movie movie, long id)
    {
        if (movie.Id != id) return BadRequest();

        var item = _unitOfWork.Movies.SingleOrDefault(p => p.Id == id);
        if (item is null) return NotFound();

        item.Name = movie.Name;
        item.Description = movie.Description;
        item.ReleaseDate = movie.ReleaseDate;

        _unitOfWork.SaveChanges();
        
        var result = OperationResult.CreateSuccessMessage();
        return Ok(result);
    }
    
    [HttpGet("{id:long}")]
    public IActionResult GetById(long id)
    {
        var item = _unitOfWork.Movies.SingleOrDefault(p => p.Id == id);
        if (item is null) return NotFound();

        var result = OperationResult.CreateSuccessMessage(item);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        var item = _unitOfWork.Movies.SingleOrDefault(p => p.Id == id);
        if (item is null) return NotFound();
        
        _unitOfWork.Movies.Remove(item);
        _unitOfWork.SaveChanges();

        var result = OperationResult.CreateSuccessMessage();
        return Ok(result);
    }
}