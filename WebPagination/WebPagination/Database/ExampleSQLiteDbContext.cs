using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace WebPagination.Database
{
    public class ExampleSQLiteDbContext : ExampleBaseDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=SQL\\ExampleDatabase.db", options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }

        public override void InitialiseDataBase()
        {
            if (System.IO.File.Exists("SQL\\ExampleDatabase.db"))
            {
                System.IO.File.Delete("SQL\\ExampleDatabase.db");
            }

            this.Database.EnsureCreated();

            this.Cities.Add(new City() { CityId = 1, Name = "Johannesburg" });
            this.Cities.Add(new City() { CityId = 2, Name = "New York" });
            this.Cities.Add(new City() { CityId = 3, Name = "Stockholm" });
            this.Cities.Add(new City() { CityId = 4, Name = "Mexico City" });
            this.Cities.Add(new City() { CityId = 5, Name = "Cairo" });
            this.Cities.Add(new City() { CityId = 6, Name = "Harare" });
            this.Cities.Add(new City() { CityId = 7, Name = "Cape Town" });
            this.Cities.Add(new City() { CityId = 8, Name = "Durban" });
            this.Cities.Add(new City() { CityId = 9, Name = "Houston" });
            this.Cities.Add(new City() { CityId = 10, Name = "Sydney" });
            this.Cities.Add(new City() { CityId = 11, Name = "Perth" });
            this.Cities.Add(new City() { CityId = 12, Name = "Moscow" });
            this.Cities.Add(new City() { CityId = 13, Name = "Pretoria" });
            this.Cities.Add(new City() { CityId = 14, Name = "Manila" });
            this.Cities.Add(new City() { CityId = 15, Name = "Rome" });
            this.Cities.Add(new City() { CityId = 16, Name = "London" });
            this.Cities.Add(new City() { CityId = 17, Name = "Tokyo" });

            this.People.Add(new Person { PersonId = 1, Name = "Agatha", DateOfBirth = new DateTime(1984, 2, 17), Savings = (decimal)100.00, CityId = 1 });
            this.People.Add(new Person { PersonId = 2, Name = "Leon", DateOfBirth = new DateTime(2001, 5, 1), Savings = (decimal)76234.89, CityId = 1 });
            this.People.Add(new Person { PersonId = 3, Name = "James", DateOfBirth = new DateTime(1998, 2, 5), Savings = (decimal)235235.98, CityId = 9 });
            this.People.Add(new Person { PersonId = 4, Name = "Christopher", DateOfBirth = new DateTime(1956, 1, 25), Savings = (decimal)1245.90, CityId = 1 });
            this.People.Add(new Person { PersonId = 5, Name = "Stephen", DateOfBirth = new DateTime(1904, 12, 20), Savings = (decimal)2358890812524.98, CityId = 2 });
            this.People.Add(new Person { PersonId = 6, Name = "Kim", DateOfBirth = new DateTime(2010, 6, 6), Savings = (decimal)200.00, CityId = 3 });
            this.People.Add(new Person { PersonId = 7, Name = "Samantha", DateOfBirth = new DateTime(2007, 3, 9), Savings = (decimal)30.00, CityId = 3 });
            this.People.Add(new Person { PersonId = 8, Name = "Matthew", DateOfBirth = new DateTime(1995, 9, 23), Savings = (decimal)750.00, CityId = 4 });
            this.People.Add(new Person { PersonId = 9, Name = "Christian", DateOfBirth = new DateTime(2016, 11, 11), Savings = (decimal)140.20, CityId = 5 });
            this.People.Add(new Person { PersonId = 10, Name = "Kayla", DateOfBirth = new DateTime(1990, 4, 10), Savings = (decimal)12411.00, CityId = 6 });
            this.People.Add(new Person { PersonId = 11, Name = "Jimmy", DateOfBirth = new DateTime(1975, 8, 2), Savings = (decimal)334555.66, CityId = 7 });
            this.People.Add(new Person { PersonId = 12, Name = "Nola", DateOfBirth = new DateTime(1984, 7, 4), Savings = (decimal)356778.22, CityId = 8 });

            this.SaveChanges();
        }
    }
}
