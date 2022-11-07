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
public class DomainsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<DomainsController> _logger;

    public DomainsController(ApplicationDbContext dbContext, ILogger<DomainsController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet(Name = "GetDomains")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    public async Task<RestDTO<Domain[]>> Get([FromQuery] RequestDTO<DomainDTO> input)
    {
        var query = _dbContext.Domains.AsQueryable();
        if (!string.IsNullOrEmpty(input.FilterQuery))
        {
            query = query.Where(d => d.Name.Contains(input.FilterQuery));
        }
        query = query.OrderBy($"{input.SortColumn} {input.SortOrder}")
                     .ThenBy("Id")
                     .Skip(input.PageIndex * input.PageSize)
                     .Take(input.PageSize);
        var recordCount = (!string.IsNullOrEmpty(input.FilterQuery)) ? await _dbContext.Domains.CountAsync(d => d.Name.Contains(input.FilterQuery)) : await _dbContext.Domains.CountAsync();
        return new RestDTO<Domain[]>()
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
                new LinkDTO(Url.Action(null, "Domains", null, Request.Scheme)!, "self", "GET")
            }
        };
    }

    [HttpPost(Name = "UpdateDomain")]
    [ResponseCache(NoStore = true)]
    public async Task<RestDTO<Domain?>> Post(DomainDTO model)
    {
        var domain = await _dbContext.Domains
                                     .Where(d => d.Id == model.Id)
                                     .FirstOrDefaultAsync();
        if (domain != null)
        {
            if (!string.IsNullOrEmpty(model.Name))
            {
                domain.Name = model.Name;
            }
            domain.LastModifiedDate = DateTime.Now;
            _dbContext.Domains.Update(domain);
            await _dbContext.SaveChangesAsync();
        }
        return new RestDTO<Domain?>()
        {
            Data = domain,
            Links = new List<LinkDTO>
            {
                new LinkDTO(
                    Url.Action(null, "Domains", model, Request.Scheme)!,
                    "self",
                    "POST"
                )
            }
        };
    }

    [HttpDelete(Name = "DeleteDomain")]
    [ResponseCache(NoStore = true)]
    public async Task<RestDTO<Domain?>> Delete(int id)
    {
        var domain = await _dbContext.Domains
                                     .Where(d => d.Id == id)
                                     .FirstOrDefaultAsync();
        if (domain != null)
        {
            _dbContext.Domains.Remove(domain);
            await _dbContext.SaveChangesAsync();
        }
        return new RestDTO<Domain?>()
        {
            Data = domain,
            Links = new List<LinkDTO>
            {
                new LinkDTO(
                    Url.Action(null, "Domains", id, Request.Scheme)!,
                    "self",
                    "DELETE"
                )
            }
        };
    }
}
