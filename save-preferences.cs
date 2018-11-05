using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Documents;

namespace Chiverton365.AzureFunctions
{


    public static class save_preferences
        {

        [FunctionName("save_preferences")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "preferences")] HttpRequest req,
            ILogger log, ExecutionContext context)
            {
            log.LogInformation("Entering save_preferences operation for the Personalization API.");

            //  get config values
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var documentClient = MySingletonDocClient.GetDocumentClient(config);
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (requestBody == null)
                {
                return new BadRequestObjectResult("Please pass JSON string in the request body");
                }
            else
                {
                CosmosData data = JsonConvert.DeserializeObject<CosmosData>(requestBody) ;
                string id = "" + data.id; 

                if (id == "") 
                    {
                    return new BadRequestObjectResult("Please pass valid id in the request body");
                    }
                else
                    {
                    Uri collectionUri = UriFactory.CreateDocumentCollectionUri(config["COSMOS_DB_PERSONALIZATION_DATABASE"], config["COSMOS_DB_PERSONALIZATION_COLLECTION"]);

                    DateTime start = DateTime.Now;
                    await documentClient.UpsertDocumentAsync(collectionUri, data, null, true);
                    TimeSpan time = DateTime.Now - start;

                    // add timing & location properties
                    CosmosDataWithTimings o = JsonConvert.DeserializeObject<CosmosDataWithTimings>(requestBody);
                    o.actualReadOrWriteEndPoint = documentClient.WriteEndpoint.ToString();
                    o.preferredCosmosDBLocation = config["COSMOS_DB_PREFERRED_LOCATION"];
                    o.duration = time.Milliseconds;

                    return (ActionResult)new OkObjectResult(o);            
                    }
                }
            }
        }
}
