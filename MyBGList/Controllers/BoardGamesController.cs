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
    public async Task<RestDTO<BoardGame[]>> Get([FromQuery] RequestDTO input)
    {
        var query = _dbContext.BoardGames.AsQueryable();
        if (!string.IsNullOrEmpty(input.FilterQuery))
        {
            query = query.Where(bg => bg.Name.Contains(input.FilterQuery));
        }
        query = query.OrderBy($"{input.SortColumn} {input.SortOrder}")
                     .ThenBy("Id")
                     .Skip(input.PageIndex * input.PageSize)
                     .Take(input.PageSize);
        var recordCount = (!string.IsNullOrEmpty(input.FilterQuery)) ? await _dbContext.BoardGames.CountAsync(bg => bg.Name.Contains(input.FilterQuery)) : await _dbContext.BoardGames.CountAsync();
        return new RestDTO<BoardGame[]>()
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
                new LinkDTO(Url.Action(null, "BoardGames", null, Request.Scheme)!, "self", "GET")
            }
        };
    }

    [HttpPost(Name = "UpdateBoardGame")]
    [ResponseCache(NoStore = true)]
    public async Task<RestDTO<BoardGame?>> Post(BoardGameDTO model)
    {
        var boardGame = await _dbContext.BoardGames
                                        .Where(bg => bg.Id == model.Id)
                                        .FirstOrDefaultAsync();
        if (boardGame != null)
        {
            if (!string.IsNullOrEmpty(model.Name))
            {
                boardGame.Name = model.Name;
            }
            if (model.Year.HasValue && model.Year.Value > 0)
            {
                boardGame.Year = model.Year.Value;
            }
            boardGame.LastModifiedDate = DateTime.Now;
            _dbContext.BoardGames.Update(boardGame);
            await _dbContext.SaveChangesAsync();
        }
        return new RestDTO<BoardGame?>()
        {
            Data = boardGame,
            Links = new List<LinkDTO>
            {
                new LinkDTO(
                    Url.Action(null, "BoardGames", model, Request.Scheme)!,
                    "self",
                    "POST"
                )
            }
        };
    }

    [HttpDelete(Name = "DeleteBoardGame")]
    [ResponseCache(NoStore = true)]
    public async Task<RestDTO<BoardGame?>> Delete(int id)
    {
        var boardGame = await _dbContext.BoardGames
                                        .Where(bg => bg.Id == id)
                                        .FirstOrDefaultAsync();
        if (boardGame != null)
        {
            _dbContext.BoardGames.Remove(boardGame);
            await _dbContext.SaveChangesAsync();
        }
        return new RestDTO<BoardGame?>()
        {
            Data = boardGame,
            Links = new List<LinkDTO>
            {
                new LinkDTO(
                    Url.Action(null, "BoardGames", id, Request.Scheme)!,
                    "self",
                    "DELETE"
                )
            }
        };
    }
}
