using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MoneyShareTermApp.Models
{
    public class PersonModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            Func<string, string> getFromForm = (string param) => bindingContext.HttpContext.Request.Form[param];

            var person = new Person();
            person.Account = new CurrencySet();
            person.CommentPrice = new CurrencySet()
            {
                Euro = decimal.Parse(getFromForm("CommentPrice.Euro")),
                Ruble = decimal.Parse(getFromForm("CommentPrice.Ruble")),
                Dollar = decimal.Parse(getFromForm("CommentPrice.Dollar"))
            };
            person.MessagePrice = new CurrencySet()
            {
                Euro = decimal.Parse(getFromForm("MessagePrice.Euro")),
                Ruble = decimal.Parse(getFromForm("MessagePrice.Ruble")),
                Dollar = decimal.Parse(getFromForm("MessagePrice.Dollar"))
            };
            person.SubscriptionPrice = new CurrencySet()
            {
                Euro = decimal.Parse(getFromForm("SubscriptionPrice.Euro")),
                Ruble = decimal.Parse(getFromForm("SubscriptionPrice.Ruble")),
                Dollar = decimal.Parse(getFromForm("SubscriptionPrice.Dollar"))
            };
            person.Mailer = new MoneyMailer();

            var props = typeof(Person).GetProperties();

            foreach (var prop in props)
                if (prop.GetCustomAttribute(typeof(RequiredAttribute)) != null)
                {
                    var val = getFromForm(prop.Name);
                    DateTime res;
                    prop.SetValue(person, DateTime.TryParse(val, out res) ? (object)res : (object)val);
                }


            //var k = typeof(Person).GetProperty("Birthday").GetCustomAttributes()
            //person.Birthday = DateTime.Parse(getFromForm());
            //person.FirstName = getFromForm("first_name");
            //person.MiddleName = getFromForm("middle_name");
            //person.SecondName = getFromForm("second_name");
            //person.Password = getFromForm("password");

            bindingContext.Result = ModelBindingResult.Success(person);

            return Task.CompletedTask;
        }
    }
}
