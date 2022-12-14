using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Hestia.Base.Utilities;

namespace Hestia.Base.RandomGenerators.CellularAutomata
{
    /// <summary>
    /// Defines a generational map recording <see cref="CellOverTime"/> states for N number of generations
    /// </summary>
    public sealed class GenerationalCellMap : IEquatable<GenerationalCellMap>
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
        [JsonIgnore]
        public Guid MapId { get; }

        /// <summary>
        /// Latest generation this map has simulated to
        /// </summary>
        [JsonIgnore]
        public int LatestSimulatedGeneration { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor for a rectangular, two-dimensional, cellular automata map
        /// </summary>
        /// <param name="ruleSet">Defining ruleset for the map</param>
        /// <param name="mapWidth">Map width</param>
        /// <param name="mapHeight">Map height</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="ruleSet"/> is not set</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="mapWidth"/> or <paramref name="mapHeight"/> are less than 1</exception>
        public GenerationalCellMap(RuleSet ruleSet, int mapWidth, int mapHeight)
            : this(ruleSet, mapWidth, mapHeight, GetRandomSeed())
        {
        }

        /// <summary>
        /// Constructor for a rectangular, two-dimensional, cellular automata map with optional seed for recreation
        /// </summary>
        /// <param name="ruleSet">Defining ruleset for the map</param>
        /// <param name="mapWidth">Map width</param>
        /// <param name="mapHeight">Map height</param>
        /// <param name="seed">Optional seed; if not set a random seed will be selected</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="ruleSet"/> is not set</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="mapWidth"/> or <paramref name="mapHeight"/> are less than 1</exception>
        [JsonConstructor]
        public GenerationalCellMap(RuleSet ruleSet, int mapWidth, int mapHeight, int seed)
        {
            RuleSet = ruleSet ?? throw new ArgumentNullException(nameof(ruleSet));
            MapWidth = mapWidth > 0 ? mapWidth : throw new ArgumentOutOfRangeException(nameof(mapWidth), WIDTH_IS_INVALID_ERROR_MESSAGE);
            MapHeight = mapHeight > 0 ? mapHeight : throw new ArgumentOutOfRangeException(nameof(mapHeight), HEIGHT_IS_INVALID_ERROR_MESSAGE);
            Seed = seed;
            MapId = PredictableGuid.NewGuid($"{RuleSet.FormattedRuleSet}_{MapWidth}_{MapHeight}_{Seed}");

            _randomNumberGenerator = new Random(Seed);
            _cellMap = new CellOverTime[MapWidth, MapHeight];

            Initialize();
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Simulates the cell map to the target generation and returns true when simulation is complete
        /// </summary>
        /// <param name="generation">Generation to simulate to</param>
        /// <remarks>This returns false if the given <paramref name="generation"/> is invalid</remarks>
        public bool SimulateToGeneration(int generation)
        {
            // If generation is invalid, return empty
            if (generation < 0)
            {
                return false;
            }

            // If requested generation is beyond the current generational
            // knowledge: simulate to the generation
            if (generation > LatestSimulatedGeneration)
            {
                SimulateGenerations(generation - LatestSimulatedGeneration);
            }

            return true;
        }

        /// <summary>
        /// Returns information about the cells at a given generation
        /// </summary>
        /// <param name="generation">Generation to get the map for</param>
        /// <remarks>If <paramref name="generation"/> is less than zero, this will return empty</remarks>
        /// <returns>Cell state at the requested generation</returns>
        public CellAtGeneration[,] GetCellMapAtGeneration(int generation)
        {
            // If generation is invalid, return empty
            if (!SimulateToGeneration(generation))
            {
                return new CellAtGeneration[0, 0];
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

            LatestSimulatedGeneration = 0;
        }

        /// <summary>
        /// Enumerates through all cells
        /// </summary>
        /// <returns>Unerlying cells</returns>
        public IEnumerable<CellOverTime> EnumerateCells()
        {
            foreach (var cell in _cellMap)
            {
                yield return cell;
            }
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="other">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public bool Equals(GenerationalCellMap? other)
        {
            return MapId == other?.MapId;
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="obj">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public override bool Equals(object? obj)
        {
            return obj is GenerationalCellMap cellMap && Equals(cellMap);
        }

        /// <summary>
        /// Returns the hash code for this instance
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return (MapHeight * 1123) ^ (MapWidth * 397) ^ Seed;
        }

        /// <summary>
        /// Returns true if the two given values equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values equate</returns>
        public static bool operator ==(GenerationalCellMap? left, GenerationalCellMap? right)
        {
            return (left is null && right is null) || (left?.Equals(right) ?? false);
        }

        /// <summary>
        /// Returns true if the two given values do not equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values do not equate</returns>
        public static bool operator !=(GenerationalCellMap? left, GenerationalCellMap? right)
        {
            return !(left == right);
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
            if (generationsToRun <= 0)
            {
                return;
            }

            for (var i = LatestSimulatedGeneration; i < LatestSimulatedGeneration + generationsToRun; i++)
            {
                foreach (var cell in _cellMap)
                {
                    var livingNeighborCount = cell.Neighbors.Count(x => x.GetLifeStateAtGeneration(i) == true);
                    var cellContainsLife = cell.GetLifeStateAtGeneration(i) ?? false;

                    if (cellContainsLife)
                    {
                        // If the cell is currently alive, it stays alive if the neighbor count
                        // satisfies the conditions to do so
                        cell.AddGeneration(RuleSet.NeighborCountCellStaysAlive.Contains(livingNeighborCount));
                    }
                    else if (RuleSet.NeighborCountCellIsBorn.Contains(livingNeighborCount))
                    {
                        // If the cell is not currently alive, a new cell is born if the neighbor
                        // count satisfies the conditions to do so
                        cell.AddGeneration(true);
                    }
                    else
                    {
                        // Cell is not alive, and one cannot be born, add another sad day to the
                        // historical record :(
                        cell.AddGeneration(cellContainsLife);
                    }
                }
            }

            LatestSimulatedGeneration += generationsToRun;
        }

        private static int GetRandomSeed()
        {
            var randomSeedBytes = new byte[sizeof(int)];
            Random.Shared.NextBytes(randomSeedBytes);
            return BitConverter.ToInt32(randomSeedBytes);
        }

        #endregion Private Methods
    }
}
