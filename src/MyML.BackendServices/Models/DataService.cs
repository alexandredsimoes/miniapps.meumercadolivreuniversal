using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyML.BackendServices.Models
{
    public class DataService : IDataService
    {
        public bool EstaAutenticado()
        {
            var result = false;
            var userId = ConfigurationManager.AppSettings["ml:userId"];
            var expiration = ConfigurationManager.AppSettings["ml:keyExpires"];
            var token = ConfigurationManager.AppSettings["ml:token"];
            var dateLogin = ConfigurationManager.AppSettings["ml:loginDate"];


            if (!String.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(token))
            {
                //Verifica a data de expiração
                var data = DateTime.MaxValue;
                var dataLogin = DateTime.MaxValue;

                if (DateTime.TryParse(expiration, out data) && DateTime.TryParse(dateLogin, out dataLogin))
                {
                    //TODO: Verificar opcao para armazenar a data do login, para comparar com a data da expiração
                    result = data > DateTime.Now;
                }
            }
            return result;
        }
    }
}
