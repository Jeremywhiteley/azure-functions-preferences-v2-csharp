using System.Collections.Generic;


namespace Chiverton365.AzureFunctions
{

public class Link
    {
    public string title;
    public string url;
    }
public class CosmosDataWithTimings 
    {
    public string id;
    public List<Link> links;
    public string color;
    public int duration;
    public string preferredCosmosDBLocation;
    public string actualReadOrWriteEndPoint;
    }

public class CosmosData 
    {
    public string id;
    public List<Link> links;
    public string color;
    }

public class HelloData
    {
    public string url { get; set; }
    public string appid { get; set; }
    }

}