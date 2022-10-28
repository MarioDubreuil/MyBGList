using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.DTO;
using MyBGList.Models;
using System.Linq.Dynamic.Core;

namespace MyBGList.Controllers;

[ApiController]
[Route("[controller]")]
public class BoardGamesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<BoardGamesController> _logger;

    public BoardGamesController(ApplicationDbContext dbContext, ILogger<BoardGamesController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet(Name = "GetBoardGames")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    public async Task<RestDTO<BoardGame[]>> Get(int pageIndex = 0, int pageSize = 10, string sortColumn = "Name")
    {
        var query = _dbContext.BoardGames
                        .OrderBy(sortColumn)
                        .ThenBy("Id")
                        .Skip(pageIndex * pageSize)
                        .Take(pageSize);
        return new RestDTO<BoardGame[]>()
        {
            Data = await query.ToArrayAsync(),
            PageIndex = pageIndex,
            PageSize = pageSize,
            SortColumn = sortColumn,
            RecordCount = await _dbContext.BoardGames.CountAsync(),
            Links = new List<LinkDTO>
            {
                new LinkDTO(Url.Action(null, "BoardGames", null, Request.Scheme)!, "self", "GET")
            }
        };
    }
}
