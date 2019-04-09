using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : Skill
{


    public Grapple(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a grapple skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["Grapple"]);
        InitSpecific(deserializedSkill["Grapple"]);
    }

    private void InitSpecific(JToken deserializedSkill)
    {
        effects = new List<Effect>();
        effects.Add(new MoveEffect());
    }

    public override void Preview(NetIdeable target)
    {
        throw new System.NotImplementedException();
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
