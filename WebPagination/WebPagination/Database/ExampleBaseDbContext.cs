using Microsoft.EntityFrameworkCore;

namespace WebPagination.Database
{
    public class ExampleBaseDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<City> Cities { get; set; }

        public virtual void InitialiseDataBase() { }
    }
}
