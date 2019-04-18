using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;

public class DestroySkill : Skill
{

    DestroyBloc DestroyEffect { get { return (DestroyBloc)effects[0]; } }

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
            Debug.LogError(target.name + " " + target.GetPosition() + " - " + target.netId);
            return false;
        }
        return true;
    }

    public override void Preview(NetIdeable target)
    {
        if (CheckConditions(GameManager.instance.PlayingPlaceable, target))
        {
            DestroyEffect.Preview((Placeable)target);
        }
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        vect =  PatternDestroy(position, vect);
        LivingPlaceable caster = (LivingPlaceable)Grid.instance.GetPlaceableFromVector(position);
        for (int i = 0; i < vect.Count; i++)
        {
            if (caster != null && !CheckSpecificConditions(caster, vect[i]))
            {
                vect.Remove(vect[i]);
            }
        }
        return vect;
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        DestroyEffect.Launcher = caster;
        target.DispatchEffect(DestroyEffect);
    }
}
