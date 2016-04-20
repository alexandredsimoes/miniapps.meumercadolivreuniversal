using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MyML.BackendServices
{
    public class Notifications
    {
        public static Notifications Instance = new Notifications();

        public NotificationHubClient Hub { get; set; }

        private Notifications()
        {            
            Hub = NotificationHubClient.CreateClientFromConnectionString(ConfigurationManager.AppSettings["Microsoft.Azure.NotificationHubs.ConnectionString"],
                                                                         ConfigurationManager.AppSettings["Microsoft.Azure.NotificationHubs.Hub"], true);
        }
    }
}