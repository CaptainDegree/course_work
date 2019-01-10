using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MoneyShareTermApp.Models
{
    public class PersonSettingsBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string getFromForm(string param) => bindingContext.HttpContext.Request.Form[param];

            var person = new Person
            {
                CommentPrice = new CurrencySet()
                {
                    Euro = decimal.Parse(getFromForm("CommentPrice.Euro")),
                    Ruble = decimal.Parse(getFromForm("CommentPrice.Ruble")),
                    Dollar = decimal.Parse(getFromForm("CommentPrice.Dollar"))
                },
                MessagePrice = new CurrencySet()
                {
                    Euro = decimal.Parse(getFromForm("MessagePrice.Euro")),
                    Ruble = decimal.Parse(getFromForm("MessagePrice.Ruble")),
                    Dollar = decimal.Parse(getFromForm("MessagePrice.Dollar"))
                },
                SubscriptionPrice = new CurrencySet()
                {
                    Euro = decimal.Parse(getFromForm("SubscriptionPrice.Euro")),
                    Ruble = decimal.Parse(getFromForm("SubscriptionPrice.Ruble")),
                    Dollar = decimal.Parse(getFromForm("SubscriptionPrice.Dollar"))
                },
            };

            var props = typeof(Person).GetProperties();

            foreach (var prop in props)
                if (prop.GetCustomAttribute(typeof(RequiredAttribute)) != null)
                {
                    var val = getFromForm(prop.Name);
                    prop.SetValue(person, DateTime.TryParse(val, out DateTime res) ? (object)res : (object)val);
                }

            person.Hidden = bool.Parse(getFromForm("Hidden"));
            person.Id = int.Parse(getFromForm("Id"));

            bindingContext.Result = ModelBindingResult.Success(person);

            return Task.CompletedTask;
        }
    }
}
