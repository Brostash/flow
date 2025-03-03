using Flow.Models.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[JsonObject(MemberSerialization.OptIn)]

public class Action : EntityBase
{
    public string ActionType { get; set; }

}