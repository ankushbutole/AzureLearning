using System; // Namespace for Console output
using System.Configuration; // Namespace for ConfigurationManager
using System.Threading.Tasks; // Namespace for Task
using Azure.Storage.Queues; // Namespace for Queue storage types
using Azure.Storage.Queues.Models; // Namespace for PeekedMessage
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace OpertionOnQueueStorage
{
    class Program
    {
        static async Task Main(string[] args)
        {

            //#region "To perform operation on Queue storage"
            //// Get the connection string from app settings
            //string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

            //// Instantiate a QueueClient which will be used to create and manipulate the queue
            //QueueClient queueClient = new QueueClient(connectionString, "myqueue");

            //// Create the queue if it doesn't already exist
            //await queueClient.CreateIfNotExistsAsync();

            //if (await queueClient.ExistsAsync())
            //{
            //    Console.WriteLine($"Queue '{queueClient.Name}' created");
            //}
            //else
            //{
            //    Console.WriteLine($"Queue '{queueClient.Name}' exists");
            //}

            //// Async enqueue the message
            //await queueClient.SendMessageAsync("Hello, World");
            //Console.WriteLine($"Message added");

            //// Async receive the message
            //QueueMessage[] retrievedMessage = await queueClient.ReceiveMessagesAsync();
            //Console.WriteLine($"Retrieved message with content '{retrievedMessage[0].MessageText}'");

            //// Async delete the message
            //await queueClient.DeleteMessageAsync(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt);
            //Console.WriteLine($"Deleted message: '{retrievedMessage[0].MessageText}'");

            //// Async delete the queue
            //await queueClient.DeleteAsync();
            //Console.WriteLine($"Deleted queue: '{queueClient.Name}'");

            //#endregion

            //Links used
            //https://docs.microsoft.com/en-us/azure/storage/queues/storage-dotnet-how-to-use-queues?tabs=dotnet
            // Get the connection string from app settings
            //string connectionString = ConfigurationManager.AppSettings["storageConnectionString"];

            // Instantiate a QueueClient which will be used to create and manipulate the queue
            //QueueClient queueClient = new QueueClient(connectionString, "myqueue");

            //queueClient.CreateIfNotExists();

            //if (queueClient.Exists())
            //{
            //    // Send a message to the queue
            //    queueClient.SendMessage("Send messages to queue");
            //}

            //if (queueClient.Exists())
            //{
            //    // Get the message from the queue
            //    QueueMessage[] message = queueClient.ReceiveMessages();

            //    // Update the message contents
            //    queueClient.UpdateMessage(message[0].MessageId,
            //            message[0].PopReceipt,
            //            "Updated contents",
            //            TimeSpan.FromSeconds(60.0)  // Make it invisible for another 60 seconds
            //        );
            //}


            #region "Operation on table storage"
            //https://www.codeproject.com/Articles/1109980/Working-with-Azure-Storage-Tables-using-Csharp

            CloudStorageAccount cloudStorageAccount =
            CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
            Console.WriteLine("Enter Table Name to create");
            string tableName = Console.ReadLine();
            CloudTable cloudTable = tableClient.GetTableReference(tableName);
            CreateNewTable(cloudTable);

            //InsertRecordToTable(cloudTable);

            
            
            DisplayTableRecords(cloudTable);

            #endregion

        }

        public static void CreateNewTable(CloudTable table)
        {
            if (!table.CreateIfNotExists())
            {
                Console.WriteLine("Table {0} already exists", table.Name);
                return;
            }
            Console.WriteLine("Table {0} created", table.Name);
        }

        public static void InsertRecordToTable(CloudTable table)
        {
            Console.WriteLine("Enter customer type");
            string customerType = Console.ReadLine();
            Console.WriteLine("Enter customer ID");
            string customerID = Console.ReadLine();
            Console.WriteLine("Enter customer name");
            string customerName = Console.ReadLine();
            Console.WriteLine("Enter customer details");
            string customerDetails = Console.ReadLine();
            Customer customerEntity = new Customer();
            customerEntity.CustomerType = customerType;
            customerEntity.CustomerID = Int32.Parse(customerID);
            customerEntity.CustomerDetails = customerDetails;
            customerEntity.CustomerName = customerName;
            customerEntity.AssignPartitionKey();
            customerEntity.AssignRowKey();
            Customer custEntity = RetrieveRecord(table, customerType, customerID);
            if (custEntity == null)
            {
                TableOperation tableOperation = TableOperation.Insert(customerEntity);
                table.Execute(tableOperation);
                Console.WriteLine("Record inserted");
            }
            else
            {
                Console.WriteLine("Record exists");
            }
        }

        public static Customer RetrieveRecord(CloudTable table, string partitionKey, string rowKey)
        {
            TableOperation tableOperation = TableOperation.Retrieve<Customer>(partitionKey, rowKey);
            TableResult tableResult = table.Execute(tableOperation);
            return tableResult.Result as Customer;
        }

        public static void UpdateRecordInTable(CloudTable table)
        {
            Console.WriteLine("Enter customer type");
            string customerType = Console.ReadLine();
            Console.WriteLine("Enter customer ID");
            string customerID = Console.ReadLine();
            Console.WriteLine("Enter customer name");
            string customerName = Console.ReadLine();
            Console.WriteLine("Enter customer details");
            string customerDetails = Console.ReadLine();
            Customer customerEntity = RetrieveRecord(table, customerType, customerID);
            if (customerEntity != null)
            {
                customerEntity.CustomerDetails = customerDetails;
                customerEntity.CustomerName = customerName;
                TableOperation tableOperation = TableOperation.Replace(customerEntity);
                table.Execute(tableOperation);
                Console.WriteLine("Record updated");
            }
            else
            {
                Console.WriteLine("Record does not exists");
            }
        }

        public static void DisplayTableRecords(CloudTable table)
        {
            TableQuery<Customer> tableQuery = new TableQuery<Customer>();
            foreach (Customer customerEntity in table.ExecuteQuery(tableQuery))
            {
                Console.WriteLine("Customer ID : {0}", customerEntity.CustomerID);
                Console.WriteLine("Customer Type : {0}", customerEntity.CustomerType);
                Console.WriteLine("Customer Name : {0}", customerEntity.CustomerName);
                Console.WriteLine("Customer Details : {0}", customerEntity.CustomerDetails);
                Console.WriteLine("******************************");
            }
        }

        public static void DeleteRecordinTable(CloudTable table)
        {
            Console.WriteLine("Enter customer type");
            string customerType = Console.ReadLine();
            Console.WriteLine("Enter customer ID");
            string customerID = Console.ReadLine();
            Customer customerEntity = RetrieveRecord(table, customerType, customerID);
            if (customerEntity != null)
            {
                TableOperation tableOperation = TableOperation.Delete(customerEntity);
                table.Execute(tableOperation);
                Console.WriteLine("Record deleted");
            }
            else
            {
                Console.WriteLine("Record does not exists");
            }
        }

        public static void DropTable(CloudTable table)
        {
            if (!table.DeleteIfExists())
            {
                Console.WriteLine("Table does not exists");
            }
        }

    }
}
