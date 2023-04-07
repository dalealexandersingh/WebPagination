using Microsoft.EntityFrameworkCore;

namespace WebPagination.Database
{
    public class ExampleMSSQLDbContext : ExampleBaseDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Initial Catalog=Example;Integrated Security=True;Connection Timeout=0;TrustServerCertificate=true");
            base.OnConfiguring(optionsBuilder);
        }

        public override void InitialiseDataBase()
        {

            this.Database.EnsureCreated();

            if (this.Cities.Count() > 0) {
                return;
            }

            this.Cities.Add(new City() { Name = "Johannesburg" });
            this.Cities.Add(new City() { Name = "New York" });
            this.Cities.Add(new City() { Name = "Stockholm" });
            this.Cities.Add(new City() { Name = "Mexico City" });
            this.Cities.Add(new City() { Name = "Cairo" });
            this.Cities.Add(new City() { Name = "Harare" });
            this.Cities.Add(new City() { Name = "Cape Town" });
            this.Cities.Add(new City() { Name = "Durban" });
            this.Cities.Add(new City() { Name = "Houston" });
            this.Cities.Add(new City() { Name = "Sydney" });
            this.Cities.Add(new City() { Name = "Perth" });
            this.Cities.Add(new City() { Name = "Moscow" });
            this.Cities.Add(new City() { Name = "Pretoria" });
            this.Cities.Add(new City() { Name = "Manila" });
            this.Cities.Add(new City() { Name = "Rome" });
            this.Cities.Add(new City() { Name = "London" });
            this.Cities.Add(new City() { Name = "Tokyo" });

            this.People.Add(new Person { Name = "Agatha", DateOfBirth = new DateTime(1984, 2, 17), Savings = (decimal)100.00, CityId = 1 });
            this.People.Add(new Person { Name = "Leon", DateOfBirth = new DateTime(2001, 5, 1), Savings = (decimal)76234.89, CityId = 1 });
            this.People.Add(new Person { Name = "James", DateOfBirth = new DateTime(1998, 2, 5), Savings = (decimal)235235.98, CityId = 9 });
            this.People.Add(new Person { Name = "Christopher", DateOfBirth = new DateTime(1956, 1, 25), Savings = (decimal)1245.90, CityId = 1 });
            this.People.Add(new Person { Name = "Stephen", DateOfBirth = new DateTime(1904, 12, 20), Savings = (decimal)2358890812524.98, CityId = 2 });
            this.People.Add(new Person { Name = "Kim", DateOfBirth = new DateTime(2010, 6, 6), Savings = (decimal)200.00, CityId = 3 });
            this.People.Add(new Person { Name = "Samantha", DateOfBirth = new DateTime(2007, 3, 9), Savings = (decimal)30.00, CityId = 3 });
            this.People.Add(new Person { Name = "Matthew", DateOfBirth = new DateTime(1995, 9, 23), Savings = (decimal)750.00, CityId = 4 });
            this.People.Add(new Person { Name = "Christian", DateOfBirth = new DateTime(2016, 11, 11), Savings = (decimal)140.20, CityId = 5 });
            this.People.Add(new Person { Name = "Kayla", DateOfBirth = new DateTime(1990, 4, 10), Savings = (decimal)12411.00, CityId = 6 });
            this.People.Add(new Person { Name = "Jimmy", DateOfBirth = new DateTime(1975, 8, 2), Savings = (decimal)334555.66, CityId = 7 });
            this.People.Add(new Person { Name = "Nola", DateOfBirth = new DateTime(1984, 7, 4), Savings = (decimal)356778.22, CityId = 8 });

            this.SaveChanges();
        }
    }
}
