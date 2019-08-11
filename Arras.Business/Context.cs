namespace Arras.Business
{
    using Arras.Data;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Class that handles actions with the SQL Database.
    /// </summary>
    public class Context
    {
        /// <summary>
        /// Initializes the database with its database migrations.
        /// </summary>
        public void InitializeDatabase()
        {
            using (var db = new DatabaseContext())
            {
                db.Database.Migrate();
            }
        }
    }
}