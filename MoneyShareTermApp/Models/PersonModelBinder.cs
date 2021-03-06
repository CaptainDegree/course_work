﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
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
            string getFromForm(string param) => bindingContext.HttpContext.Request.Form[param];

            var person = new Person
            {
                Account = new CurrencySet()
                {
                    Euro = 0,
                    Ruble = 0,
                    Dollar = 0
                },
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
                Mailer = new MoneyMailer()
            };

            var props = typeof(Person).GetProperties();

            foreach (var prop in props)
                if (prop.GetCustomAttribute(typeof(RequiredAttribute)) != null)
                {
                    var val = getFromForm(prop.Name);
                    prop.SetValue(person, DateTime.TryParse(val, out DateTime res) ? (object)res : (object)val);
                }

            bindingContext.Result = ModelBindingResult.Success(person);

            return Task.CompletedTask;
        }
    }
}
