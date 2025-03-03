

using Flow.Models.Entities;
using Microsoft.Identity.Client;

public class QueryById
{
    public QueryById(string id, string partitionKey)
    {
        this.id = id;
        this.partitionKey = partitionKey;
    }
    public string id { get; set; }
    public string partitionKey { get; set; }
}