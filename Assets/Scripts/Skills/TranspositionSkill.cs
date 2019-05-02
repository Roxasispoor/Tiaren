using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranspositionSkill : Skill
{
    public TranspositionSkill(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a Transposition skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["TranspositionSkill"]);
        Init(deserializedSkill["TranspositionSkill"]);
    }

    public override void Preview(NetIdeable target)
    {
        Debug.Log("no preview for transposition");
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        throw new System.NotImplementedException();
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        throw new System.NotImplementedException();
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        throw new System.NotImplementedException();
    }
}
