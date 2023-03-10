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
    public IActionResult Get(string? search = null, bool? disabled = null,
        int pageNo = 1, int pageSize = 10, string sortField = nameof(Movie.Id), string sortDir = SortDir.Desc)
    {
        var pagedList = _unitOfWork.Movies.ToPagedList(new ()
        {
            Search = search,
            Disabled = disabled,
            PageNo = pageNo,
            PageSize = pageSize,
            SortField = sortField,
            SortDir = sortDir
        });

        var result = OperationResult.CreateSuccessMessage(pagedList);
        return Ok(result);
    }
    
    [HttpPost]
    public IActionResult Add(Movie model)
    {
        model.CreatedDate = DateTime.UtcNow;
        _unitOfWork.Movies.Add(model);

        _unitOfWork.SaveChanges();

        var result = OperationResult.CreateSuccessMessage(model);

        var uri = new Uri(GetAbsoluteUri(model.Id.ToString()));
        return Created(uri, result);
    }

    [HttpPatch("{id:long}")]
    public IActionResult Update(Movie model, long id)
    {
        if (model.Id != id) return BadRequest();

        var item = _unitOfWork.Movies.SingleOrDefault(p => p.Id == id);
        if (item is null) return NotFound();

        item.Name = model.Name;
        item.Description = model.Description;
        item.ReleaseDate = model.ReleaseDate;
        item.Disabled = model.Disabled;

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