using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MyML.BackendServices.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyML.BackendServices.Controllers
{
    public class HomeController : Controller
    {
        DataService _dataService = new DataService();

        public ActionResult Index()
        {
            ViewBag.Message = _dataService.EstaAutenticado() ? "OK" : "Não OK";

            //var cn = ConfigurationManager.AppSettings["table_connectionstring"];
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("table_connectionstring"));

            //// Create the table client.
            ////CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            //// Create the table if it doesn't exist.
            ////CloudTable table = tableClient.GetTableReference("notifications");
            ////table.CreateIfNotExists();

            //// Create the table client.
            //CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            //// Create the CloudTable object that represents the "people" table.
            //CloudTable table = tableClient.GetTableReference("notifications");

            //// Create a new customer entity.
            //NotificationMessage notification = new NotificationMessage("1234", "1234");
            //notification.Content = "Message";
            //notification.Processed = false;

            //// Create the TableOperation object that inserts the customer entity.
            //TableOperation insertOperation = TableOperation.Insert(notification);

            //// Execute the insert operation.
            //table.Execute(insertOperation);

            return View();
        }
    }

    
}
