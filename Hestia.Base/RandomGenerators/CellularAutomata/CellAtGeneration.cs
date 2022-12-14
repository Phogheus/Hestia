namespace Hestia.Base.RandomGenerators.CellularAutomata
{
    /// <summary>
    /// Defines an instance of a <see cref="CellOverTime"/> at a particular generation
    /// </summary>
    /// <param name="Generation">Generation of the cell</param>
    /// <param name="ContainedLife">True if the cell contained life at the generation</param>
    /// <param name="HeatMapValue">
    /// Normalized value between 0.0 and 1.0, representing the number of times this cell had
    /// life across all generations up to the requested generation
    /// </param>
    /// <param name="PositionX">Cell X position in a <see cref="GenerationalCellMap"/></param>
    /// <param name="PositionY">Cell Y position in a <see cref="GenerationalCellMap"/></param>
    public record CellAtGeneration(int Generation, bool ContainedLife, double HeatMapValue, int PositionX, int PositionY);
}
