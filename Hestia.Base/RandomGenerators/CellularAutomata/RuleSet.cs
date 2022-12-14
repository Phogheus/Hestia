using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Hestia.Base.RandomGenerators.CellularAutomata
{
    /// <summary>
    /// Defines a generic Cellular Automata Ruleset
    /// </summary>
    /// <seealso href="https://en.wikipedia.org/wiki/Cellular_automaton"/>
    public sealed class RuleSet : IEquatable<RuleSet>
    {
        #region Fields

        private const char RULE_SET_DELIMITER = '/';
        private const string STAY_ALIVE_VALUE_IS_INVALID_ERROR_MESSAGE = "Stay alive value must be a positive non-zero number.";
        private const string TO_BE_BORN_VALUE_IS_INVALID_ERROR_MESSAGE = "To be born value must be a positive non-zero number.";
        private const string RULE_SET_INPUT_IS_INVALID_ERROR_MESSAGE = "Rule set must be digits only in the format 'X/Y'.";

        #endregion Fields

        #region Properties

        /// <summary>
        /// Returns the underlying ruleset in 'X/Y' format
        /// </summary>
        public string FormattedRuleSet { get; }

        /// <summary>
        /// Returns the neighbor rules for allowing a cell to remain alive
        /// </summary>
        [JsonIgnore]
        public int[] NeighborCountCellStaysAlive { get; }

        /// <summary>
        /// Returns the neighbor rules for allowing a cell to be born
        /// </summary>
        [JsonIgnore]
        public int[] NeighborCountCellIsBorn { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor taking a cellular automata ruleset in the digits only format of 'X/Y'
        /// </summary>
        /// <param name="formattedRuleSet">Rule set</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="formattedRuleSet"/> is not set</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if ruleset splits into more or less than 2 groups</exception>
        /// <exception cref="FormatException">Thrown if ruleset components are made of anything but digits</exception>
        [JsonConstructor]
        public RuleSet(string formattedRuleSet)
        {
            if (string.IsNullOrWhiteSpace(formattedRuleSet))
            {
                throw new ArgumentNullException(nameof(formattedRuleSet));
            }

            // Replace \ with / in case of simple user (probably me) goofs,
            // and then split on /
            var ruleSetComponents = formattedRuleSet
                .Replace('\\', RULE_SET_DELIMITER)
                .Split(RULE_SET_DELIMITER, StringSplitOptions.RemoveEmptyEntries);

            if (ruleSetComponents.Length != 2)
            {
                throw new ArgumentOutOfRangeException(nameof(formattedRuleSet), RULE_SET_INPUT_IS_INVALID_ERROR_MESSAGE);
            }

            NeighborCountCellStaysAlive = int.TryParse(ruleSetComponents[0], out var toStayAlive) && toStayAlive > 0
                ? GetDigits(toStayAlive)
                : throw new FormatException(STAY_ALIVE_VALUE_IS_INVALID_ERROR_MESSAGE);

            NeighborCountCellIsBorn = int.TryParse(ruleSetComponents[1], out var toBeBorn) && toBeBorn > 0
                ? GetDigits(toBeBorn)
                : throw new FormatException(TO_BE_BORN_VALUE_IS_INVALID_ERROR_MESSAGE);

            FormattedRuleSet = string.Join(RULE_SET_DELIMITER, ruleSetComponents);
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="other">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public bool Equals(RuleSet? other)
        {
            return FormattedRuleSet == other?.FormattedRuleSet;
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="obj">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public override bool Equals(object? obj)
        {
            return obj is RuleSet ruleSet && Equals(ruleSet);
        }

        /// <summary>
        /// Returns the hash code for this instance
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return (NeighborCountCellStaysAlive.Length * 397) ^ NeighborCountCellIsBorn.Length;
        }

        /// <summary>
        /// Returns true if the two given values equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values equate</returns>
        public static bool operator ==(RuleSet? left, RuleSet? right)
        {
            return (left is null && right is null) || (left?.Equals(right) ?? false);
        }

        /// <summary>
        /// Returns true if the two given values do not equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values do not equate</returns>
        public static bool operator !=(RuleSet? left, RuleSet? right)
        {
            return !(left == right);
        }

        #endregion Public Methods

        #region Private Methods

        private static int[] GetDigits(int input)
        {
            return input.ToString()
                .Select(x => int.Parse(x.ToString()))
                .Distinct()
                .OrderBy(x => x)
                .ToArray();
        }

        #endregion Private Methods
    }
}
