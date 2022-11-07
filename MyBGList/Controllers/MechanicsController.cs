using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.Attributes;
using MyBGList.DTO;
using MyBGList.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;

namespace MyBGList.Controllers;

[ApiController]
[Route("[controller]")]
public class MechanicsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<MechanicsController> _logger;

    public MechanicsController(ApplicationDbContext dbContext, ILogger<MechanicsController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet(Name = "GetMechanics")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    public async Task<RestDTO<Mechanic[]>> Get([FromQuery] RequestDTO<MechanicDTO> input)
    {
        var query = _dbContext.Mechanics.AsQueryable();
        if (!string.IsNullOrEmpty(input.FilterQuery))
        {
            query = query.Where(m => m.Name.Contains(input.FilterQuery));
        }
        query = query.OrderBy($"{input.SortColumn} {input.SortOrder}")
                     .ThenBy("Id")
                     .Skip(input.PageIndex * input.PageSize)
                     .Take(input.PageSize);
        var recordCount = (!string.IsNullOrEmpty(input.FilterQuery)) ? await _dbContext.Mechanics.CountAsync(m => m.Name.Contains(input.FilterQuery)) : await _dbContext.Mechanics.CountAsync();
        return new RestDTO<Mechanic[]>()
        {
            Data = await query.ToArrayAsync(),
            PageIndex = input.PageIndex,
            PageSize = input.PageSize,
            SortColumn = input.SortColumn,
            SortOrder = input.SortOrder,
            FilterQuery = input.FilterQuery,
            RecordCount = recordCount,
            Links = new List<LinkDTO>
            {
                new LinkDTO(Url.Action(null, "Mechanics", null, Request.Scheme)!, "self", "GET")
            }
        };
    }

    [HttpPost(Name = "UpdateMechanic")]
    [ResponseCache(NoStore = true)]
    public async Task<RestDTO<Mechanic?>> Post(MechanicDTO model)
    {
        var mechanic = await _dbContext.Mechanics
                                       .Where(m => m.Id == model.Id)
                                       .FirstOrDefaultAsync();
        if (mechanic != null)
        {
            if (!string.IsNullOrEmpty(model.Name))
            {
                mechanic.Name = model.Name;
            }
            mechanic.LastModifiedDate = DateTime.Now;
            _dbContext.Mechanics.Update(mechanic);
            await _dbContext.SaveChangesAsync();
        }
        return new RestDTO<Mechanic?>()
        {
            Data = mechanic,
            Links = new List<LinkDTO>
            {
                new LinkDTO(
                    Url.Action(null, "Mechanics", model, Request.Scheme)!,
                    "self",
                    "POST"
                )
            }
        };
    }

    [HttpDelete(Name = "DeleteMechanic")]
    [ResponseCache(NoStore = true)]
    public async Task<RestDTO<Mechanic?>> Delete(int id)
    {
        var mechanic = await _dbContext.Mechanics
                                       .Where(m => m.Id == id)
                                       .FirstOrDefaultAsync();
        if (mechanic != null)
        {
            _dbContext.Mechanics.Remove(mechanic);
            await _dbContext.SaveChangesAsync();
        }
        return new RestDTO<Mechanic?>()
        {
            Data = mechanic,
            Links = new List<LinkDTO>
            {
                new LinkDTO(
                    Url.Action(null, "Mechanics", id, Request.Scheme)!,
                    "self",
                    "DELETE"
                )
            }
        };
    }
}
