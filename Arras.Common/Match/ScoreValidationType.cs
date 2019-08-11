namespace Arras.Common.Match
{
    /// <summary>
    /// Enum containing all the responses when a score is entered.
    /// </summary>
    public enum ScoreValidationType
    {
        /// <summary>
        /// The score that was entered is invalid (e.g. impossible score)
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// The score is valid and subtracted from the remaining.
        /// </summary>
        Valid = 1,

        /// <summary>
        /// The score that was entered put the remaining on 0. Which means that the leg was won.
        /// </summary>
        EndsLeg = 2
    }
}