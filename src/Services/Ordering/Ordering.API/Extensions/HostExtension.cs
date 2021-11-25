using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace Ordering.API.Extensions
{
    public static class HostExtension
    {
        public static IHost MigrationDatabase<TContext>(this IHost host,
                                                        Action<TContext, IServiceProvider> seeder,
                                                        int? retry = 0) where TContext : DbContext
        {
            int retryForAvailability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("START Migration database");

                    Invoke(context, services, seeder);

                    logger.LogInformation("END Migration database");
                }
                catch (SqlException ex)
                {
                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        Thread.Sleep(2000);
                        MigrationDatabase<TContext>(host, seeder, retryForAvailability);
                    }
                }
            }
            return host;
        }

        private static void Invoke<TContext>(TContext context, 
                                            IServiceProvider services, 
                                            Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}
