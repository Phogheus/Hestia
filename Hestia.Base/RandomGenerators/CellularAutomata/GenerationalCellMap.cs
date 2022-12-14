using System;
using System.Collections.Generic;
using System.Linq;
using Hestia.Base.Utilities;

namespace Hestia.Base.RandomGenerators.CellularAutomata
{
    /// <summary>
    /// Defines a generational map recording <see cref="CellOverTime"/> states for N number of generations
    /// </summary>
    public class GenerationalCellMap
    {
        #region Fields

        private const string WIDTH_IS_INVALID_ERROR_MESSAGE = "Width must be positive and non-zero.";
        private const string HEIGHT_IS_INVALID_ERROR_MESSAGE = "Height must be positive and non-zero.";

        private readonly Random _randomNumberGenerator;
        private readonly CellOverTime[,] _cellMap;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Returns the underlying ruleset governing this map
        /// </summary>
        public RuleSet RuleSet { get; }

        /// <summary>
        /// Map width
        /// </summary>
        public int MapWidth { get; }

        /// <summary>
        /// Map height
        /// </summary>
        public int MapHeight { get; }

        /// <summary>
        /// Seed used for this map
        /// </summary>
        public int Seed { get; }

        /// <summary>
        /// <see cref="PredictableGuid"/> based on the <see cref="Seed"/> of this map
        /// </summary>
        public Guid MapId { get; }

        /// <summary>
        /// Latest generation this map has simulated to
        /// </summary>
        public int LatestGeneration { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor for a rectangular, two-dimensional, cellular automata map with optional seed for recreation
        /// </summary>
        /// <param name="ruleSet">Defining ruleset for the map</param>
        /// <param name="mapWidth">Map width</param>
        /// <param name="mapHeight">Map height</param>
        /// <param name="seed">Optional seed; if not set a random seed will be selected</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="ruleSet"/> is not set</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="mapWidth"/> or <paramref name="mapHeight"/> are less than 1</exception>
        public GenerationalCellMap(RuleSet ruleSet, int mapWidth, int mapHeight, int? seed = null)
        {
            RuleSet = ruleSet ?? throw new ArgumentNullException(nameof(ruleSet));
            MapWidth = mapWidth > 0 ? mapWidth : throw new ArgumentOutOfRangeException(nameof(mapWidth), WIDTH_IS_INVALID_ERROR_MESSAGE);
            MapHeight = mapHeight > 0 ? mapHeight : throw new ArgumentOutOfRangeException(nameof(mapHeight), HEIGHT_IS_INVALID_ERROR_MESSAGE);

            if (seed == null)
            {
                var randomSeedBytes = new byte[sizeof(int)];
                Random.Shared.NextBytes(randomSeedBytes);
                seed = BitConverter.ToInt32(randomSeedBytes);
            }

            Seed = seed.Value;
            MapId = PredictableGuid.NewGuid(Seed.ToString());

            _randomNumberGenerator = new Random(Seed);
            _cellMap = new CellOverTime[MapWidth, MapHeight];

            Initialize();
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Returns information about the cells at a given generation
        /// </summary>
        /// <param name="generation">Generation to get the map for</param>
        /// <returns>Cell state at the requested generation</returns>
        public CellAtGeneration[,] GetCellMapAtGeneration(int generation)
        {
            // If generation is invalid, return empty
            if (generation < 0)
            {
                return new CellAtGeneration[0, 0];
            }

            // If requested generation is beyond the current generational
            // knowledge: simulate to the generation
            if (generation > LatestGeneration)
            {
                SimulateGenerations(generation - LatestGeneration);
            }

            var returnMap = new CellAtGeneration[MapWidth, MapHeight];

            foreach (var cell in _cellMap)
            {
                var cellHadLifeAtTheTime = cell.GetLifeStateAtGeneration(generation)!.Value;
                var cellHeatValueAtTheTime = cell.GetHeatValueAtGeneration(generation)!.Value;
                var x = cell.PositionX;
                var y = cell.PositionY;

                returnMap[x, y] = new CellAtGeneration(cellHadLifeAtTheTime, cellHeatValueAtTheTime, x, y);
            }

            return returnMap;
        }

        /// <summary>
        /// Resets the historical recordings in this map to the first generation
        /// </summary>
        public void Reset()
        {
            foreach (var cell in _cellMap)
            {
                cell.ClearHistoricalRecord();
            }

            LatestGeneration = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="generation"></param>
        /// <returns></returns>
        public IEnumerable<bool> EnumerateStateByGeneration(int generation)
        {
            // If generation is invalid, return empty
            if (generation < 0)
            {
                yield break;
            }

            // If requested generation is beyond the current generational
            // knowledge: simulate to the generation
            if (generation > LatestGeneration)
            {
                SimulateGenerations(generation - LatestGeneration);
            }

            foreach (var cell in _cellMap)
            {
                yield return cell.GetLifeStateAtGeneration(generation)!.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="generation"></param>
        /// <returns></returns>
        public IEnumerable<float> EnumerateHeatValueByGeneration(int generation)
        {
            // If generation is invalid, return empty
            if (generation < 0)
            {
                yield break;
            }

            // If requested generation is beyond the current generational
            // knowledge: simulate to the generation
            if (generation > LatestGeneration)
            {
                SimulateGenerations(generation - LatestGeneration);
            }

            foreach (var cell in _cellMap)
            {
                yield return cell.GetHeatValueAtGeneration(generation)!.Value;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void Initialize()
        {
            var dimensions = MapWidth * MapHeight;

            // Initialize each cell at generation 0
            for (var i = 0; i < dimensions; i++)
            {
                var x = i % MapWidth;
                var y = (int)Math.Floor(i / (double)MapWidth);

                _cellMap[x, y] = new CellOverTime(_randomNumberGenerator.Next(0, 2) == 1, x, y);
            }

            foreach (var cell in _cellMap)
            {
                var x = cell.PositionX;
                var y = cell.PositionY;

                var canHaveLeftNeighbors = cell.PositionX > 0;
                var canHaveRightNeighbors = cell.PositionX < MapWidth - 1;
                var canHaveTopNeighbors = cell.PositionY > 0;
                var canHaveBottomNeighbors = cell.PositionY < MapHeight - 1;

                if (canHaveTopNeighbors)
                {
                    // Set Top Left neighbor
                    if (canHaveLeftNeighbors)
                    {
                        cell.AddNeighbor(_cellMap[x - 1, y - 1]);
                    }

                    // Set Top neighbor
                    cell.AddNeighbor(_cellMap[x, y - 1]);

                    // Set Top Right neighbor
                    if (canHaveRightNeighbors)
                    {
                        cell.AddNeighbor(_cellMap[x + 1, y - 1]);
                    }
                }

                // Set Right neighbor
                if (canHaveRightNeighbors)
                {
                    cell.AddNeighbor(_cellMap[x + 1, y]);
                }

                if (canHaveBottomNeighbors)
                {
                    // Set Bottom Right neighbor
                    if (canHaveRightNeighbors)
                    {
                        cell.AddNeighbor(_cellMap[x + 1, y + 1]);
                    }

                    // Set Bottom neighbor
                    cell.AddNeighbor(_cellMap[x, y + 1]);

                    // Set Bottom Left neighbor
                    if (canHaveLeftNeighbors)
                    {
                        cell.AddNeighbor(_cellMap[x - 1, y + 1]);
                    }
                }

                // Set Left neighbor
                if (canHaveLeftNeighbors)
                {
                    cell.AddNeighbor(_cellMap[x - 1, y]);
                }
            }
        }

        private void SimulateGenerations(int generationsToRun)
        {
            for (var i = LatestGeneration; i < LatestGeneration + generationsToRun; i++)
            {
                foreach (var cell in _cellMap)
                {
                    var livingNeighborCount = cell.Neighbors.Count(x => x.GetLifeStateAtGeneration(i) == true);

                    if (cell.GetLifeStateAtGeneration(i) == true)
                    {
                        cell.AddGeneration(RuleSet.NeighborCountCellStaysAlive.Contains(livingNeighborCount));
                    }
                    else if (RuleSet.NeighborCountCellIsBorn.Contains(livingNeighborCount))
                    {
                        cell.AddGeneration(true);
                    }
                    else
                    {
                        cell.AddGeneration(false);
                    }
                }
            }

            LatestGeneration += generationsToRun;
        }

        #endregion Private Methods
    }
}
