namespace Arras.Data
{
    using Entity;
    using Functional.Maybe;

    /// <summary>
    /// Provides access to the database concerning match services.
    /// </summary>
    public class MatchRepository
    {
        /// <summary>
        /// Gets the latest open match that has not ended yet.
        /// </summary>
        /// <returns></returns>
        public Maybe<StandardMatchEntity> GetContinueMatch()
        {
            return Maybe<StandardMatchEntity>.Nothing;
        }
    }
}