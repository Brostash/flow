using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Flow.Models.Entities;

[JsonObject(MemberSerialization.OptIn)]

public class Workflow : EntityBase
{

    public string Name { get; set; }

    [JsonIgnore]
    public List<ActionInstance> Actions { get; set; }

    public List<string> ActionIds { get; set; }
}
