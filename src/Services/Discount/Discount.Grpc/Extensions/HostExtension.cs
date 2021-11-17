using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Threading;

namespace Discount.Grpc.Extensions
{
    public static class HostExtension
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAbility = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postgres database.");

                    using (var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
                    {
                        connection.Open();

                        using (var command = new NpgsqlCommand() { Connection = connection })
                        {
                            command.CommandText = "DROP TABLE IF EXISTS Coupon";
                            command.ExecuteNonQuery();

                            command.CommandText = @"CREATE TABLE Coupon(
	                                                    ID SERIAL PRIMARY KEY NOT NULL,
	                                                    ProductName VARCHAR(24) NOT NULL,
	                                                    Description	TEXT,
	                                                    Amount	INT
                                                    )";
                            command.ExecuteNonQuery();

                            command.CommandText = @"INSERT INTO Coupon(ProductName, Description, Amount) VALUES ('IPhone X', 'Iphone discount', 150)";
                            command.ExecuteNonQuery();

                            command.CommandText = @"INSERT INTO Coupon(ProductName, Description, Amount) VALUES ('Samsung 10', 'Samsung discount', 100)";
                            command.ExecuteNonQuery();
                        }
                    }

                    logger.LogInformation("Migrated postgres database.");
                }
                catch (NpgsqlException e)
                {
                    logger.LogError(e, "An error occurred while migrating the postgres database.");

                    if (retryForAbility < 50)
                    {
                        retryForAbility++;
                        Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAbility);
                    }
                }
            }

            return host;
        }
    }
}
