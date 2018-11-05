using System;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;

namespace Chiverton365.AzureFunctions
{

    class MySingletonDocClient
        {
        private static DocumentClient _DocClient;

        public static DocumentClient GetDocumentClient(IConfigurationRoot config)
            {
            if (_DocClient == null)
                {
                // Perform any "one Time" initialization here
                Uri serviceUri = new Uri(config["COSMOS_DB_HOSTURL"]);
                var authKey = config["COSMOS_DB_KEY"];

                var policy = new ConnectionPolicy
                    {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp,
                    EnableEndpointDiscovery = true,
                    UseMultipleWriteLocations = true,
        
                    // Customize retry options for Throttled requests
                    RetryOptions = new RetryOptions()
                        {
                        MaxRetryAttemptsOnThrottledRequests = 10,
                        MaxRetryWaitTimeInSeconds = 30
                        }
                    } ;  

                // Customize PreferredLocations
                policy.PreferredLocations.Add(config["COSMOS_DB_PREFERRED_LOCATION"]);
                //policy.SetCurrentLocation (config["COSMOS_DB_PREFERRED_LOCATION"])  ;

                _DocClient = new DocumentClient(serviceUri, authKey, policy);
                }

            return _DocClient;
            }
        } 

}