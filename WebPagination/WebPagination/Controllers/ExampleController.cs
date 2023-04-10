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

        [Route("Custom")]
        public IActionResult Custom()
        {
            InitialiseData();
            return View();
        }

        [Route("JQuery")]
        public IActionResult JQuery()
        {
            InitialiseData();
            return View();
        }

        private ExampleBaseDbContext DbContextFactory() {
            return new ExampleMSSQLDbContext();
        }

        [HttpPost]
        [Route("GetPeople")]
        public IActionResult GetPeople(TableSearchModel model)
        {
            using (var db = DbContextFactory())
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

        [HttpPost]
        [Route("GetPeopleFromBody")]
        public IActionResult GetPeopleFromBody([FromBody] TableSearchModel model)
        {
            return GetPeople(model);
        }


        private void InitialiseData() {

            using (var db = DbContextFactory())
            {
                db.InitialiseDataBase();
            }
        }

    }
}
