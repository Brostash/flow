using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Flow.Models.Entities;

[JsonObject(MemberSerialization.OptIn)]
public class ActionInstance : EntityBase
{
    [JsonIgnore]
    public Workflow Workflow { get; set; }

    public string WorkflowId { get; set; }

    public string ActionId { get; set; }

    [JsonIgnore]
    public Action Action { get; set; }

    public JObject Input { get; set; }

    public JObject Output { get; set; }

    public JObject Configuration { get; set; }
}
