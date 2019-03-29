using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;

public class DestroySkill : Skill
{

    CreateBlock DestroyEffect { get { return (CreateBlock)effects[0]; } }

    public DestroySkill(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a destroy skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["DestroySkill"]);
        InitSpecific(deserializedSkill["DestroySkill"]);
    }

    private void InitSpecific(JToken deserializedSkill)
    {
        effects = new List<Effect>();
        effects.Add(new DestroyBloc());
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        if (target as Placeable == null)
        {
            Debug.LogError("target is not a placeable! Not good in push");
            return false;
        }
        return true;
    }

    public override void Preview(NetIdeable target)
    {
        Debug.LogError("Preview not implemented");
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        return PatternDestroy(position, vect);
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        DestroyEffect.Launcher = caster;
        target.DispatchEffect(DestroyEffect);
    }
}
