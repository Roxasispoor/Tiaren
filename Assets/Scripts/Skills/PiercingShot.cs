using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;


public class PiercingShot : Skill
{
    [SerializeField]
    public float power;

    public PiercingShot(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a piercing skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["PiercingShot"]);
        InitSpecific(deserializedSkill["PiercingShot"]);
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
        PiercingDamageEffect piercing = new PiercingDamageEffect(power, DamageCalculated.DamageScale.DEXT);
        piercing.Launcher = caster;
        target.DispatchEffect(piercing);
    }

    public override void Preview(NetIdeable target)
    {
        Debug.LogError("Preview not implemented");
    }
}
