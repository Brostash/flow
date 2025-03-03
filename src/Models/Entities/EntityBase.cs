namespace Flow.Models.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[JsonObject(MemberSerialization.OptIn)]

public abstract class EntityBase
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("_etag")]
    public string ETag { get; set; }
}