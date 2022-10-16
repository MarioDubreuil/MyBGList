﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyBGList.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BoardGamesController : ControllerBase
    {
        private readonly ILogger<BoardGamesController> _logger;

        public BoardGamesController(ILogger<BoardGamesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<BoardGame> Get()
        {
            return new[]
            {
                new BoardGame()
                {
                    Id = 1,
                    Name = "Axies & Allies",
                    Year = 1981,
                    MinPlayers = 2,
                    MaxPlayers = 5
                },
                new BoardGame()
                {
                    Id = 2,
                    Name = "Citadels",
                    Year = 2000,
                    MinPlayers = 2,
                    MaxPlayers = 8
                },
                new BoardGame()
                {
                    Id = 3,
                    Name = "Terraforming Mars",
                    Year = 2016,
                    MinPlayers = 1,
                    MaxPlayers = 5
                }
            };
    }
}
}