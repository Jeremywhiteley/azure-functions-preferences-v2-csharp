using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace Chiverton365.AzureFunctions
{
    public static class hello
    {
        [FunctionName("hello")]
        public static IActionResult Hello(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "hello")] HttpRequest req,
            ILogger log, ExecutionContext context)
            {
            log.LogInformation("Entering hello operation for Personalization");

            //  get config values
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            HelloData data = new HelloData();
            data.appid  = config["WEBAPI_APPID"];
            data.url    = config["WEBAPI_APPID_URI"];

            return (ActionResult) new OkObjectResult(data);
        }
    }
}
