using Microsoft.Azure.Devices;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace IotHubDeviceSyncTool
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {


            var configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json")
              .AddEnvironmentVariables()
              .Build();

            Console.WriteLine(configuration.GetConnectionString("DestinationIotHubConnectionString"));
            Console.WriteLine(configuration.GetConnectionString("BlobSasToken"));

            RegistryManager sourceRegistryManager = RegistryManager.CreateFromConnectionString(configuration.GetConnectionString("SourceIotHubConnectionString"));
            RegistryManager destinationRegistryManager = RegistryManager.CreateFromConnectionString(configuration.GetConnectionString("DestinationIotHubConnectionString"));

            string containerSasUri = configuration.GetConnectionString("BlobSasToken");

            Console.WriteLine($"Start export of IoT Hub devices to blob");
            // Call an export job on the IoT Hub to retrieve all devices

            JobProperties exportJob =
              await sourceRegistryManager.ExportDevicesAsync(containerSasUri, false);
            var exportJobResult = await WaitForJobCompletion(sourceRegistryManager, exportJob);

            Console.WriteLine($"Start import of IoT Hub devices to blob");

            JobProperties importJob =
            await destinationRegistryManager.ImportDevicesAsync(containerSasUri, containerSasUri);

            var importJobResult = await WaitForJobCompletion(destinationRegistryManager, importJob);
        }

        private static async Task<JobProperties> WaitForJobCompletion(RegistryManager registryManager, JobProperties currentJob)
        {
            while (true)
            {
                currentJob = await registryManager.GetJobAsync(currentJob.JobId);
                if (currentJob.Status == JobStatus.Completed ||
                    currentJob.Status == JobStatus.Failed ||
                    currentJob.Status == JobStatus.Cancelled)
                {
                    // Job has finished executing
                    break;
                }

                Console.WriteLine($"Job progress: {currentJob.Progress}%");
                await Task.Delay(TimeSpan.FromSeconds(5));

            }
            Console.WriteLine($"Job Completed with status {currentJob.Status}");
            return currentJob;
        }
    }
}
