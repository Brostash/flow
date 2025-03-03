namespace Flow.Models.Entities;


public abstract class EntityBase
{
    public string id { get; set; }

    public string _etag { get; set; }
}