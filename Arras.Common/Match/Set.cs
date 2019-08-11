namespace Arras.Common.Match
{
    using System.Collections.Generic;

    /// <summary>
    /// A class representing a set.
    /// </summary>
    public class Set
    {
        public Set()
        {
            this.Legs = new List<Leg>();
        }
        /// <summary>
        /// A list of legs played during a set.
        /// </summary>
        public List<Leg> Legs { get; set; }
    }
}