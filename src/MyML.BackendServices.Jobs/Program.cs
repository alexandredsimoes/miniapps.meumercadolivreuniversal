using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.BackendServices.Jobs
{
    public class Program
    {
        static void Main(string[] args)
        {
            JobHost host = new JobHost();
            host.RunAndBlock();
        }

        public static void ProcessNotifications()
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=mercadolivreuniversal;AccountKey=Qprnt8KhGNufTL/cL3vnh3FiQc3Tqbecfk7QT7hE2/rIGJofNC49ecQJJYBJMW3u4Rc7+jMci5rj/MS7DhO6EA==");

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("notifications");

            // Create the table query.
            TableQuery<NotificationMessage> rangeQuery = new TableQuery<NotificationMessage>().Where(
                TableQuery.GenerateFilterConditionForBool("Processed", QueryComparisons.Equal, false));


            // Loop through the results, displaying information about the entity.
            foreach (NotificationMessage entity in table.ExecuteQuery(rangeQuery))
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
                    entity.Content, entity.Processed);
            }
        }
    }
}
