using SudokuGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SudokuGame.Services
{
    public class JsonGamePersistenceService : IGamePersistenceService
    {
        public void SaveGame(GameState game, string filePath)
        {
            var dto = CreateSaveDataDTO(game);

            var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public GameState LoadGame(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Save file not found at {filePath}");

            var json = File.ReadAllText(filePath);
            var dto = JsonSerializer.Deserialize<SaveDataDTO>(json);

            return CreateGameFromSaveData(dto);
        }

        private SaveDataDTO CreateSaveDataDTO(GameState game)
        {
            int[][] gridArray = new int[9][];
            bool[][] isInitialArray = new bool[9][];

            for (int i = 0; i < 9; i++)
            {
                gridArray[i] = new int[9];
                isInitialArray[i] = new bool[9];
                for (int j = 0; j < 9; j++)
                {
                    gridArray[i][j] = game.Grid.Grid[i, j];
                    isInitialArray[i][j] = game.Grid.IsInitial[i, j];
                }
            }

            return new SaveDataDTO
            {
                Grid = gridArray,
                IsInitial = isInitialArray,
                Difficulty = (int)game.Difficulty,
                ElapsedTime = game.ElapsedTime.TotalSeconds,
                Mistakes = game.Mistakes,
                StartTime = game.StartTime.ToBinary()
            };
        }

        private GameState CreateGameFromSaveData(SaveDataDTO dto)
        {
            var game = new GameState((DifficultyLevel)dto.Difficulty);
            var grid = new int[9, 9];
            var isInitial = new bool[9, 9];

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    grid[i, j] = dto.Grid[i][j];
                    isInitial[i, j] = dto.IsInitial[i][j];
                }

            game.Grid.SetInitialGrid(grid);

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (!isInitial[i, j] && grid[i, j] != 0)
                        game.Grid.SetCell(i, j, grid[i, j]);

            game.ElapsedTime = TimeSpan.FromSeconds(dto.ElapsedTime);
            game.Mistakes = dto.Mistakes;
            game.StartTime = DateTime.FromBinary(dto.StartTime);

            return game;
        }
    }
}
