using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;

public class BowShot : Skill
{
    [SerializeField]
    public float power;

    DamageCalculated DamageEffect { get { return (DamageCalculated)effects[0]; } }

    public BowShot(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a normal bow skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["BowShot"]);
        InitSpecific(deserializedSkill["BowShot"]);
    }

    private void InitSpecific(JToken deserializedSkill)
    {
        power = (float)deserializedSkill["power"];
        effects = new List<Effect>();
        effects.Add(new DamageCalculated(power, DamageCalculated.DamageScale.DEXT));
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
        return NoPattern(position, vect);
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        DamageEffect.Launcher = caster;
        target.DispatchEffect(DamageEffect);
    }

    public override void Preview(NetIdeable target)
    {
        if (CheckConditions(GameManager.instance.PlayingPlaceable, target))
        {
            DamageEffect.Preview((Placeable)target);
        }
    }
}
