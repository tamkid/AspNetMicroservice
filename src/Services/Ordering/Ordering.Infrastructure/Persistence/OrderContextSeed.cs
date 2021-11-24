using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext context, ILogger<OrderContextSeed> logger)
        {
            if (!context.Orders.Any())
            {
                logger.LogInformation("Start Seeding data for orders");

                context.Orders.AddRange(GetPreconfiguredOrders());
                await context.SaveChangesAsync();

                logger.LogInformation("End Seeding data for orders");
            }
        }

        private static IEnumerable<Order> GetPreconfiguredOrders()
        {
            return new List<Order>
            {
                new Order() {
                    UserName = "kid",
                    FirstName = "Tam",
                    LastName = "Nguyen",
                    EmailAddress = "thedragonkid01@gmail.com",
                    AddressLine = "32 Tran Van Du",
                    Country = "VietNam",
                    TotalPrice = 350
                }
            };
        }
    }
}
