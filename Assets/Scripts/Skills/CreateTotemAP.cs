using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTotemAP : CreateTotemSkill
{
    public CreateTotemAP(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a totemAP skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["CreateTotemAP"]);
        Init(deserializedSkill["CreateTotemAP"]);
    }
}
