using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningAttack : Skill
{
    [SerializeField]
    private float power;

    DamageCalculated DamageEffect { get { return (DamageCalculated)effects[0]; } }

    public SpinningAttack(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a Spinning skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["SpinningAttack"]);
        InitSpecific(deserializedSkill["SpinningAttack"]);
    }

    private void InitSpecific(JToken deserializedSkill)
    {
        squareShaped = true;
        power = (float)deserializedSkill["power"];
        effects = new List<Effect>();
        effects.Add(new DamageCalculated(power, DamageCalculated.DamageScale.STR));
    }

    public override void Preview(NetIdeable target)
    {
        List<Placeable> affectedPlaceable = Skill.PatternUseAround((Placeable)target);

        foreach (Placeable placeable in affectedPlaceable)
        {
            LivingPlaceable fleshyTarget = placeable as LivingPlaceable;
            if (fleshyTarget)
            {
                DamageEffect.Preview(fleshyTarget);
            }
        }
    }

    public override void UnPreview(NetIdeable target)
    {
        List<Placeable> affectedPlaceable = Skill.PatternUseAround((Placeable)target);

        foreach (Placeable placeable in affectedPlaceable)
        {
            LivingPlaceable fleshyTarget = placeable as LivingPlaceable;
            if (fleshyTarget)
            {
                DamageEffect.ResetPreview(fleshyTarget);
            }
        }
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
        return PatternAround(position, vect);
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        DamageEffect.Launcher = caster;

        List<Placeable> affectedPlaceable = Skill.PatternUseAround((Placeable)target);

        foreach (Placeable placeable in affectedPlaceable)
        {
            LivingPlaceable fleshyTarget = placeable as LivingPlaceable;
            if (fleshyTarget)
            {
                fleshyTarget.DispatchEffect(DamageEffect);
            }
        }
    }
}
