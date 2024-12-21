using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace Play.Common.HealthChecks
{
    public class MongoDbHealthCheck : IHealthCheck
    {
        private readonly MongoClient client;

        public MongoDbHealthCheck(MongoClient client)
        {
            this.client = client;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Set a timeout for the MongoDB operation 
                using (var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30))) 
                { 
                    using (var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellationTokenSource.Token)) 
                    { 
                        await client.ListDatabaseNamesAsync(linkedCancellationToken.Token); 
                        return HealthCheckResult.Healthy(); 
                    } 
                }               
            }
            catch (Exception ex)
            {
                // Log detailed exception information 
                Console.WriteLine($"Health check failed: {ex.Message}");
                return HealthCheckResult.Unhealthy(exception: ex);
            }
        }
    }
}