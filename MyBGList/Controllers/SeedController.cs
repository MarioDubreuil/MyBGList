using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.Models;
using MyBGList.Models.Csv;
using System.Globalization;

namespace MyBGList.Controllers;

[ApiController]
[Route("[controller]")]
public class SeedController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<BoardGamesController> _logger;

    public SeedController(ApplicationDbContext dbContext, IWebHostEnvironment env, ILogger<BoardGamesController> logger)
    {
        _dbContext = dbContext;
        _env = env;
        _logger = logger;
    }

    [HttpPut(Name = "Seed")]
    [ResponseCache(NoStore = true)]
    public async Task<JsonResult> Put()
    {
        // Setup
        var config = new CsvConfiguration(CultureInfo.GetCultureInfo("pt-BR"))
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };
        using var reader = new StreamReader(Path.Combine(_env.ContentRootPath, "Data/bgg_dataset.csv"));
        using var csv = new CsvReader(reader, config);
        var existingBoardGames = await _dbContext.BoardGames.ToDictionaryAsync(bg => bg.Id);
        var existingDomains = await _dbContext.Domains.ToDictionaryAsync(d => d.Name);
        var existingMechanics = await _dbContext.Mechanics.ToDictionaryAsync(m => m.Name);
        var now = DateTime.Now;

        // Execute
        var records = csv.GetRecords<BggRecord>();
        var skippedRows = 0;
        foreach (var record in records)
        {
            if (!record.ID.HasValue ||
                string.IsNullOrEmpty(record.Name) ||
                existingBoardGames.ContainsKey(record.ID.Value))
            {
                skippedRows++;
                continue;
            }
            var boardGame = new BoardGame()
            {
                Id = record.ID.Value,
                Name = record.Name,
                BGGRank = record.BggRank ?? 0,
                ComplexityAverage = record.ComplexityAverage ?? 0,
                MaxPlayers = record.MaxPlayers ?? 0,
                MinAge = record.MinAge ?? 0,
                MinPlayers = record.MinPlayers ?? 0,
                OwnedUsers = record.OwnedUsers ?? 0,
                PlayTime = record.PlayTime ?? 0,
                RatingAverage = record.RatingAverage ?? 0,
                UsersRated = record.UsersRated ?? 0,
                Year = record.YearPublished ?? 0,
                CreatedDate = now,
                LastModifiedDate = now
            };
            _dbContext.BoardGames.Add(boardGame);
            if (!string.IsNullOrEmpty(record.Domains))
            {
                foreach(var domainName in record.Domains.Split(',', StringSplitOptions.TrimEntries).Distinct(StringComparer.InvariantCultureIgnoreCase))
                {
                    var domain = existingDomains.GetValueOrDefault(domainName);
                    if (domain == null)
                    {
                        domain = new Domain()
                        {
                            Name = domainName,
                            CreatedDate = now,
                            LastModifiedDate = now
                        };
                        _dbContext.Domains.Add(domain);
                        existingDomains.Add(domainName, domain);
                    }
                    _dbContext.BoardGames_Domains.Add(new BoardGames_Domains()
                    {
                        BoardGame = boardGame,
                        Domain = domain,
                        CreatedDate = now
                    });
                }
            }
            if (!string.IsNullOrEmpty(record.Mechanics))
            {
                foreach(var mechanicName in record.Mechanics.Split(',', StringSplitOptions.TrimEntries).Distinct(StringComparer.InvariantCultureIgnoreCase))
                {
                    var mechanic = existingMechanics.GetValueOrDefault(mechanicName);
                    if (mechanic == null)
                    {
                        mechanic = new Mechanic()
                        {
                            Name = mechanicName,
                            CreatedDate = now,
                            LastModifiedDate = now
                        };
                        _dbContext.Mechanics.Add(mechanic);
                        existingMechanics.Add(mechanicName, mechanic);
                    }
                    _dbContext.BoardGames_Mechanics.Add(new BoardGames_Mechanics()
                    {
                        BoardGame = boardGame,
                        Mechanic = mechanic,
                        CreatedDate = now
                    });
                }
            }
        }   

        // Save
        await _dbContext.SaveChangesAsync();

        // Recap
        var result = new JsonResult(new{
            BoardGames = _dbContext.BoardGames.Count(),
            Domains = _dbContext.Domains.Count(),
            Mechanics = _dbContext.Mechanics.Count(),
            SkippedRows = skippedRows
        });
        return result;
    }
}
