using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Flow.Models.Entities;

public class ActionInstance : EntityBase
{
    [JsonIgnore]
    public Workflow workflow { get; set; }

    public string workflowId { get; set; }

    public string actionId { get; set; }

    public Action action { get; set; }

    public JObject input { get; set; }

    public JObject output { get; set; }

    public JObject configuration { get; set; }
}
