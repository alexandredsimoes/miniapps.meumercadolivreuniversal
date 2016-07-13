using System;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using FluentAssertions;
using MyML.UWP.Log;
using Xunit;


namespace MyML.UWP.Tests
{

    public class MiscTests
    {
        [Fact]
        public void Gerar_Log_Deve_Retornar_Excecao()
        {
            Task t1 = new Task(() =>
            {
                Task.Delay(100);
                AppLogs.WriteError("Erro", "Erro");
            });
           

            

            Action a = () =>
            {
                
                
            };

            a.ShouldThrow<SecurityAccessDeniedException>();
        }
    }
}
