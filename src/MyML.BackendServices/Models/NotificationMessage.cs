using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyML.BackendServices.Models
{
    public class NotificationMessage : TableEntity
    {
        public NotificationMessage(string questionId, string userId)
        {
            this.PartitionKey = userId;
            this.RowKey = questionId;
        }

        public string Content { get; set; }
        public bool Processed { get; set; }
        public string ProductId { get; set; }
    }
}