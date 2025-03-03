using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Flow.Models.Entities;

[JsonObject(MemberSerialization.OptIn)]

public class Workflow : EntityBase
{

    public string name { get; set; }

    public List<ActionInstance> actionInstance { get; set; }

    public List<string> actionInstanceIds { get; set; }
}
