using ZestPost.DbService;
using ZestPost.Service;
using ZestPost.DbService.Entity; // Add this for AccountFB
using System;
using System.Collections.Generic; // Add this for List
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ZestPost.Controller
{
    public class PostArticleController
    {
        private readonly ZestPostContext _context;
        private readonly CachingService _cache;

        public PostArticleController(ZestPostContext context, CachingService cache)
        {
            _context = context;
            _cache = cache;
        }

        private async Task ExecuteCancellableOperation(Func<Task> operation, string operationName, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Processing {operationName}...");
            try
            {
                await operation();
                Console.WriteLine($"{operationName} completed.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"{operationName} was cancelled.");
                // Optionally, perform cleanup here if needed after cancellation
                throw; // Re-throw to be caught by EventHandlerService
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during {operationName}: {ex.Message}");
                throw; // Re-throw the exception
            }
        }

        public async Task ProcessArticlePost(List<AccountFB> accounts, JToken generalConfig, JToken contentConfig, CancellationToken cancellationToken)
        {
            await ExecuteCancellableOperation(async () =>
            {
                // Access your data
                Console.WriteLine($"Number of accounts for posting: {accounts?.Count}");
                Console.WriteLine($"General Config: {generalConfig}");
                Console.WriteLine($"Content Config: {contentConfig}");

                // Example: Simulate a long-running operation that can be cancelled
                for (int i = 0; i < 10; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation request
                    Console.WriteLine($"Processing post step {i + 1}...");
                    await Task.Delay(1000, cancellationToken); // Simulate work, delay with cancellation support
                }
            }, "article post", cancellationToken);
        }

        public async Task ProcessScanAccounts(List<AccountFB> accounts, JToken scanConfig, CancellationToken cancellationToken)
        {
            await ExecuteCancellableOperation(async () =>
            {
                // Access your data
                Console.WriteLine($"Number of accounts for scanning: {accounts?.Count}");
                Console.WriteLine($"Scan Config: {scanConfig}");

                // Example: Simulate a long-running scanning operation that can be cancelled
                for (int i = 0; i < 5; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation request
                    Console.WriteLine($"Processing scan step {i + 1}...");
                    await Task.Delay(2000, cancellationToken); // Simulate work, longer delay for scanning
                }
            }, "account scan", cancellationToken);
        }
    }
}
