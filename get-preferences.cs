using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;

namespace Chiverton365.AzureFunctions
{

public static class get_preferences
    {
        [FunctionName("CosmosDb_get_preferences")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "preferences/{id}")] HttpRequest req,
            ILogger log, string id, ExecutionContext context)
            {
            log.LogInformation("Entering get_preferences operation for Personalization");

            //  get config values
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var documentClient = MySingletonDocClient.GetDocumentClient(config);

            if (id == null)
                {
                log.LogInformation($"parameter {id} not found");
                return new BadRequestObjectResult("Please pass an id parameter");
                }

            try 
                {
                Uri documentUri = UriFactory.CreateDocumentUri(config["COSMOS_DB_PERSONALIZATION_DATABASE"], config["COSMOS_DB_PERSONALIZATION_COLLECTION"], id);

                DateTime start = DateTime.Now;
                Document doc = await documentClient.ReadDocumentAsync(documentUri);
                TimeSpan time = DateTime.Now - start;
                
                // add timing & location properties
                CosmosDataWithTimings o = JsonConvert.DeserializeObject<CosmosDataWithTimings>(doc.ToString());
                o.actualReadOrWriteEndPoint = documentClient.ReadEndpoint.ToString();
                o.preferredCosmosDBLocation = config["COSMOS_DB_PREFERRED_LOCATION"];
                o.duration = time.Milliseconds;

                return (ActionResult)new OkObjectResult(o);  
                } 
            catch (Exception e)
                {
                log.LogInformation(e.Message);
                log.LogInformation($"data for {id} not found");
                return new NotFoundResult(); 
                }      
            }
    }
}

