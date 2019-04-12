using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegSwipe : Skill
{
    int buffValue;
    int damage;

    DamageCalculated DamageEffect { get { return (DamageCalculated)effects[0]; } }

    public LegSwipe(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a debufPM skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["LegSwipe"]);
        InitSpecific(deserializedSkill["LegSwipe"]);
    }

    private void InitSpecific(JToken deserializedSkill)
    {
        squareShaped = true;
        buffValue = (int)deserializedSkill["buffValue"];
        damage = (int)deserializedSkill["damage"];
        effects = new List<Effect>();
        effects.Add(new DamageCalculated(damage, DamageCalculated.DamageScale.BRUT));
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        if (!target.IsLiving())
        {
            Debug.LogError("Trying to launch an attack on a block");
            return false;
        }
        return true;
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
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
        ParameterChangeV2<LivingPlaceable, float>.CreateChangeAndReset((LivingPlaceable)target, buffValue, 0, 3);
        target.DispatchEffect(DamageEffect);
    }

    public override void Preview(NetIdeable target)
    {
        //no preview
    }
}
