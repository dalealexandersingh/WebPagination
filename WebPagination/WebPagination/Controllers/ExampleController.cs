using DataTableExample.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPagination.Database;
using WebPagination.Models;

namespace WebPagination.Controllers
{
    public class ExampleController : Controller
    {
        public IActionResult Index()
        {
            InitialiseData();
            return View();
        }

        [HttpPost]
        [Route("GetPeople")]
        public IActionResult GetPeople(TableSearchModel model)
        {
            using (var db = new ExampleDbContext())
            {
                var year = DateTime.Today.Year;

                var query = from people in db.People
                            join cities in db.Cities on people.CityId equals cities.CityId
                            select new
                            {
                                personid = people.PersonId,
                                name = people.Name,
                                dob = people.DateOfBirth,
                                age = year - people.DateOfBirth.Year,
                                city = cities.Name,
                                savings = people.Savings
                            };

                return query.GetPagedList(model);
            }
        }


        private void InitialiseData() {

            if (System.IO.File.Exists("SQL\\ExampleDatabase.db")) {
                System.IO.File.Delete("SQL\\ExampleDatabase.db");
            }

            using (var db = new ExampleDbContext())
            {
                db.Database.EnsureCreated();

                db.Cities.Add(new City() { CityId = 1, Name = "Johannesburg" });
                db.Cities.Add(new City() { CityId = 2, Name = "New York" });
                db.Cities.Add(new City() { CityId = 3, Name = "Stockholm" });
                db.Cities.Add(new City() { CityId = 4, Name = "Mexico City" });
                db.Cities.Add(new City() { CityId = 5, Name = "Cairo" });
                db.Cities.Add(new City() { CityId = 6, Name = "Harare" });
                db.Cities.Add(new City() { CityId = 7, Name = "Cape Town" });
                db.Cities.Add(new City() { CityId = 8, Name = "Durban" });
                db.Cities.Add(new City() { CityId = 9, Name = "Houston" });
                db.Cities.Add(new City() { CityId = 10, Name = "Sydney" });
                db.Cities.Add(new City() { CityId = 11, Name = "Perth" });
                db.Cities.Add(new City() { CityId = 12, Name = "Moscow" });
                db.Cities.Add(new City() { CityId = 13, Name = "Pretoria" });
                db.Cities.Add(new City() { CityId = 14, Name = "Manila" });
                db.Cities.Add(new City() { CityId = 15, Name = "Rome" });
                db.Cities.Add(new City() { CityId = 16, Name = "London" });
                db.Cities.Add(new City() { CityId = 17, Name = "Tokyo" });

                db.People.Add(new Person { PersonId = 1, Name = "Agatha", DateOfBirth = new DateTime(1984, 2, 17), Savings= (decimal)100.00, CityId = 1 });
                db.People.Add(new Person { PersonId = 2, Name = "Leon", DateOfBirth = new DateTime(2001, 5, 1), Savings = (decimal)76234.89, CityId = 1 });
                db.People.Add(new Person { PersonId = 3, Name = "James", DateOfBirth = new DateTime(1998, 2, 5), Savings = (decimal)235235.98, CityId = 9 });
                db.People.Add(new Person { PersonId = 4, Name = "Christopher", DateOfBirth = new DateTime(1956, 1, 25), Savings = (decimal)1245.90, CityId = 1 });
                db.People.Add(new Person { PersonId = 5, Name = "Stephen", DateOfBirth = new DateTime(1904, 12, 20), Savings = (decimal)2358890812524.98, CityId = 2 });
                db.People.Add(new Person { PersonId = 6, Name = "Kim", DateOfBirth = new DateTime(2010, 6, 6), Savings = (decimal)200.00, CityId = 3 });
                db.People.Add(new Person { PersonId = 7, Name = "Samantha", DateOfBirth = new DateTime(2007, 3, 9), Savings = (decimal)30.00, CityId = 3 });
                db.People.Add(new Person { PersonId = 8, Name = "Matthew", DateOfBirth = new DateTime(1995, 9, 23), Savings = (decimal)750.00, CityId = 4 });
                db.People.Add(new Person { PersonId = 9, Name = "Christian", DateOfBirth = new DateTime(2016, 11, 11), Savings = (decimal)140.20, CityId = 5 });
                db.People.Add(new Person { PersonId = 10, Name = "Kayla", DateOfBirth = new DateTime(1990, 4, 10), Savings = (decimal)12411.00, CityId = 6 });
                db.People.Add(new Person { PersonId = 11, Name = "Jimmy", DateOfBirth = new DateTime(1975, 8, 2), Savings = (decimal)334555.66, CityId = 7 });
                db.People.Add(new Person { PersonId = 12, Name = "Nola", DateOfBirth = new DateTime(1984, 7, 4), Savings = (decimal)356778.22, CityId = 8 });

                db.SaveChanges();
            }
        }

    }
}
