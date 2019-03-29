using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;

public class BowAttack : Skill
{
    [SerializeField]
    public float power;

    public BowAttack(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a normal bow skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["BowAttack"]);
        InitSpecific(deserializedSkill["BowAttack"]);
    }

    private void InitSpecific(JToken deserializedSkill)
    {
        power = (float)deserializedSkill["power"];
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
        DamageCalculated damageCalculated = new DamageCalculated(power, DamageCalculated.DamageScale.DEXT);
        damageCalculated.Launcher = caster;
        target.DispatchEffect(damageCalculated);
    }

    public override void Preview(NetIdeable target)
    {
        Debug.LogError("Preview not implemented");
    }
}
