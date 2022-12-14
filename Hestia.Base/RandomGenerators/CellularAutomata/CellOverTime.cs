using System.Collections.Generic;
using System.Linq;

namespace Hestia.Base.RandomGenerators.CellularAutomata
{
    /// <summary>
    /// Defines a single cell in a <see cref="GenerationalCellMap"/>
    /// </summary>
    public class CellOverTime
    {
        #region Fields

        private readonly List<bool> _historicalRecord;
        private readonly List<CellOverTime> _neighbors;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Returns the historical record of this cell in regards to what generations it did and did not contain life
        /// </summary>
        /// <remarks>Indices can be associated with generations in that index 0 is the first generation</remarks>
        public IReadOnlyList<bool> HistoricalRecord => _historicalRecord.AsReadOnly();

        /// <summary>
        /// Returns all neighbor cells
        /// </summary>
        public IReadOnlyList<CellOverTime> Neighbors => _neighbors.AsReadOnly();

        /// <summary>
        /// Returns the X position in the map this cell is located
        /// </summary>
        public int PositionX { get; }

        /// <summary>
        /// Returns the Y position in the map this cell is located
        /// </summary>
        public int PositionY { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor for a cell in a <see cref="GenerationalCellMap"/> taking first generation life state and map position
        /// </summary>
        /// <param name="containsLife">True if cell initially contains life in the first generation</param>
        /// <param name="positionX">X map position</param>
        /// <param name="positionY">Y map position</param>
        public CellOverTime(bool containsLife, int positionX, int positionY)
        {
            _historicalRecord = new List<bool> { containsLife };
            _neighbors = new List<CellOverTime>();

            PositionX = positionX;
            PositionY = positionY;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Adds the given life state to the historical record of generations
        /// </summary>
        /// <param name="generationContainsLife">Next generation's life state</param>
        public void AddGeneration(bool generationContainsLife)
        {
            _historicalRecord.Add(generationContainsLife);
        }

        /// <summary>
        /// Adds a given cell as a neighbor, ignoring position values
        /// </summary>
        /// <param name="neighbor">Neighbor to add</param>
        /// <remarks>Null values will not be added</remarks>
        public void AddNeighbor(CellOverTime? neighbor)
        {
            if (neighbor == null)
            {
                return;
            }

            _neighbors.Add(neighbor);
        }

        /// <summary>
        /// Returns the life state at the requested generation
        /// </summary>
        /// <param name="generation">Requested generation</param>
        /// <remarks>This returns null if the <paramref name="generation"/> is invalid</remarks>
        /// <returns>Life state or null for invalid generation</returns>
        public bool? GetLifeStateAtGeneration(int generation)
        {
            return generation >= 0 && generation < _historicalRecord.Count
                ? _historicalRecord[generation]
                : null;
        }

        /// <summary>
        /// Returns the normalized value between 0.0 and 1.0 representing the number
        /// of times this cell contained life based on the requested generation
        /// </summary>
        /// <param name="generation">Requested generation</param>
        /// <remarks>This returns null if the <paramref name="generation"/> is invalid</remarks>
        /// <returns>Heat value or null</returns>
        public double? GetHeatValueAtGeneration(int generation)
        {
            return generation >= 0 && generation < _historicalRecord.Count
                ? _historicalRecord.Take(generation + 1).Count(x => x) / (generation + 1d)
                : null;
        }

        /// <summary>
        /// Clears the historical record beyond the first generation
        /// </summary>
        public void ClearHistoricalRecord()
        {
            var firstGenerationContainedLife = _historicalRecord[0];

            _historicalRecord.Clear();
            _historicalRecord.Add(firstGenerationContainedLife);
        }

        #endregion Public Methods
    }
}
