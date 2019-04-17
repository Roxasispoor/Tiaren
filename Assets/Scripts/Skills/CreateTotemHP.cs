using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTotemHP : CreateTotemSkill
{
    public CreateTotemHP(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a totemHP skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["CreateTotemHP"]);
        Init(deserializedSkill["CreateTotemHP"]);
    }
}
