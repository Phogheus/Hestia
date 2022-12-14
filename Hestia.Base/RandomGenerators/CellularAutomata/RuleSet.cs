using System;
using System.Linq;

namespace Hestia.Base.RandomGenerators.CellularAutomata
{
    /// <summary>
    /// Defines a generic Cellular Automata Ruleset
    /// </summary>
    /// <seealso href="https://en.wikipedia.org/wiki/Cellular_automaton"/>
    public sealed class RuleSet
    {
        #region Fields

        private const char RULE_SET_DELIMITER = '/';
        private const string STAY_ALIVE_VALUE_IS_INVALID_ERROR_MESSAGE = "Stay alive value must be a positive non-zero number.";
        private const string TO_BE_BORN_VALUE_IS_INVALID_ERROR_MESSAGE = "To be born value must be a positive non-zero number.";
        private const string RULE_SET_INPUT_IS_INVALID_ERROR_MESSAGE = "Rule set must be digits only in the format 'X/Y'.";

        #endregion Fields

        #region Properties

        /// <summary>
        /// Returns the neighbor rules for allowing a cell to remain alive
        /// </summary>
        public int[] NeighborCountCellStaysAlive { get; }

        /// <summary>
        /// Returns the neighbor rules for allowing a cell to be born
        /// </summary>
        public int[] NeighborCountCellIsBorn { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor taking a cellular automata ruleset in the digits only format of 'X/Y'
        /// </summary>
        /// <param name="ruleSet">Rule set</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="ruleSet"/> is not set</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if ruleset splits into more or less than 2 groups</exception>
        /// <exception cref="FormatException">Thrown if ruleset components are made of anything but digits</exception>
        public RuleSet(string ruleSet)
        {
            if (string.IsNullOrWhiteSpace(ruleSet))
            {
                throw new ArgumentNullException(nameof(ruleSet));
            }

            // Replace \ with / in case of simple user (probably me) goofs,
            // and then split on /
            var ruleSetComponents = ruleSet.Replace('\\', RULE_SET_DELIMITER).Split(RULE_SET_DELIMITER);

            if (ruleSetComponents.Length != 2)
            {
                throw new ArgumentOutOfRangeException(nameof(ruleSet), RULE_SET_INPUT_IS_INVALID_ERROR_MESSAGE);
            }

            NeighborCountCellStaysAlive = int.TryParse(ruleSetComponents[0], out var toStayAlive) && toStayAlive > 0
                ? GetDigits(toStayAlive)
                : throw new FormatException(STAY_ALIVE_VALUE_IS_INVALID_ERROR_MESSAGE);

            NeighborCountCellIsBorn = int.TryParse(ruleSetComponents[1], out var toBeBorn) && toBeBorn > 0
                ? GetDigits(toBeBorn)
                : throw new FormatException(TO_BE_BORN_VALUE_IS_INVALID_ERROR_MESSAGE);
        }

        #endregion Constructors

        #region Private Methods

        private static int[] GetDigits(int input)
        {
            return input.ToString().Select(x => int.Parse(x.ToString())).ToArray();
        }

        #endregion Private Methods
    }
}
