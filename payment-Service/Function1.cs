using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;

namespace payment_Service
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<bool> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            var connectionString = "Endpoint=sb://pymt-order-bus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=1t7oE7QNxEfrgQKAwCklb63MRw2xAF81w+ASbL2TMTw=";
            //Streaming the body


            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var client = new ServiceBusClient(connectionString);



                var sender = client.CreateSender("pymt-queue");
                var message = new ServiceBusMessage(requestBody);
                if (requestBody.Contains("scheduled"))
                    message.ScheduledEnqueueTime = DateTimeOffset.UtcNow.AddSeconds(15);

                if (requestBody.Contains("ttl"))
                    message.TimeToLive = TimeSpan.FromSeconds(20);

                await sender.SendMessageAsync(message);


                log.LogInformation("returning True");
                return true;
            }
            catch (Exception)
            {

                return false;
            }
           

        }
    }
}
