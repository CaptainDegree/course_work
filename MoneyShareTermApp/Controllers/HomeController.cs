using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyShareTermApp.Models;

namespace MoneyShareTermApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Authorize()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddUser(Person person)
        {
            var dbContext = new postgresContext();

            if (dbContext.Person.Find(person.Id) is null)
            {
                dbContext.Person.AddAsync(person);
                dbContext.SaveChanges();
                return RedirectToAction("Profile");
            }

            return View("Register");
        }
    }
}