using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ecommerce_api.Services;

namespace ecommerce_api.Tests
{
    /// <summary>
    /// Simple manual test class for RepositoryInfoService.
    /// This can be run manually to verify the GitHub API integration works.
    /// </summary>
    public class RepositoryInfoServiceTest
    {
        public static async Task TestBasicFunctionality()
        {
            // Setup
            var httpClient = new HttpClient();
            var configuration = new ConfigurationBuilder().Build();
            var service = new RepositoryInfoService(httpClient, configuration);

            try
            {
                // Test with a well-known public repository
                Console.WriteLine("Testing RepositoryInfoService with microsoft/vscode repository...");

                // Test branches
                Console.WriteLine("\nTesting GetBranchesAsync...");
                var branches = await service.GetBranchesAsync("microsoft", "vscode");
                Console.WriteLine($"Found {branches.Count} branches");
                if (branches.Count > 0)
                {
                    Console.WriteLine($"First few branches: {string.Join(", ", branches.Take(3))}");
                }

                // Test commit count
                Console.WriteLine("\nTesting GetCommitCountAsync...");
                var commitCount = await service.GetCommitCountAsync("microsoft", "vscode");
                Console.WriteLine($"Commit count (first 100): {commitCount}");

                // Test collaborators
                Console.WriteLine("\nTesting GetCollaboratorsAsync...");
                var collaborators = await service.GetCollaboratorsAsync("microsoft", "vscode");
                Console.WriteLine($"Found {collaborators.Count} collaborators");
                if (collaborators.Count > 0)
                {
                    Console.WriteLine($"First few collaborators: {string.Join(", ", collaborators.Take(3))}");
                }

                Console.WriteLine("\nAll tests completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                httpClient.Dispose();
            }
        }
    }
}