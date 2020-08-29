using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Ordering.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Data
{
    public class OrderContextSeed
    {

        public static async Task SeedAsync(OrderContext context, ILoggerFactory loggerFactory, int? retry = 0)
        {
            int retryForAvailablity = retry.Value;

            try
            {
                context.Database.Migrate();
                if (!context.Orders.Any())
                {
                    context.Orders.AddRange(GetPreConfiguredOrders());

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (retryForAvailablity < 3)
                {
                    retryForAvailablity++;
                    var log = loggerFactory.CreateLogger<OrderContextSeed>();
                    log.LogError(ex.Message);
                    await SeedAsync(context, loggerFactory, retryForAvailablity);
                }
            }
        }

        private static IEnumerable<Order> GetPreConfiguredOrders()
        {
            return new List<Order>
            {
                new Order() { UserName="Siddharth", FirstName="Siddharth", LastName= "Rawat", EmailAddress ="sidr@saxo.com", AddressLine = "Gurgoan", TotalPrice = 1000 },
                new Order() { UserName = "swn", FirstName = "Selim", LastName = "Arslan", EmailAddress ="sel@ars.com", AddressLine = "Ferah", TotalPrice = 3486 }
            };
        }
    }
}
