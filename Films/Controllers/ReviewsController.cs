using Films.Core;
using Films.Core.Domain;
using Films.Core.Domain.Dtos;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Films.Controllers;

public class ReviewsController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    [HttpGet]
    public IActionResult Get(long movieId, string? search = null, int pageNo = 1, int pageSize = 10, string sortField = nameof(Review.Id), string sortDir = SortDir.Desc)
    {
        if (!(movieId > 0)) return BadRequest();

        var pagedList = _unitOfWork.Reviews.ToPagedList(new ()
        {
            MovieId = movieId,
            Search = search,
            PageNo = pageNo,
            PageSize = pageSize,
            SortField = sortField,
            SortDir = sortDir
        });

        var result = OperationResult.CreateSuccessMessage(pagedList);
        return Ok(result);
    }
    
    [HttpPost]
    public IActionResult Add(ReviewDto model)
    {
        var entity = model.Adapt<Review>();
        entity.CreatedDate = DateTime.UtcNow;

        _unitOfWork.Reviews.Add(entity);

        _unitOfWork.SaveChanges();

        var result = OperationResult.CreateSuccessMessage(entity);

        var uri = new Uri(GetAbsoluteUri(entity.Id.ToString()));
        return Created(uri, result);
    }

    [HttpPatch("{id:long}")]
    public IActionResult Update(ReviewDto model, long id)
    {
        if (model.Id != id) return BadRequest();

        var item = _unitOfWork.Reviews.SingleOrDefault(p => p.Id == id);
        if (item is null) return NotFound();

        item.Title = model.Title;
        item.Description = model.Description;
        item.Rating = model.Rating;

        _unitOfWork.SaveChanges();
        
        var result = OperationResult.CreateSuccessMessage();
        return Ok(result);
    }
    
    [HttpGet("{id:long}")]
    public IActionResult GetById(long id)
    {
        var item = _unitOfWork.Reviews.SingleOrDefault(p => p.Id == id);
        if (item is null) return NotFound();

        var result = OperationResult.CreateSuccessMessage(item);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        var item = _unitOfWork.Reviews.SingleOrDefault(p => p.Id == id);
        if (item is null) return NotFound();
        
        _unitOfWork.Reviews.Remove(item);
        _unitOfWork.SaveChanges();

        var result = OperationResult.CreateSuccessMessage();
        return Ok(result);
    }
}