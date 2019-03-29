using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegSwipe : Skill
{
    int debufValue;
    int damage;

    public LegSwipe(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a debufPM skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["MagicMissile"]);
        InitSpecific(deserializedSkill["MagicMissile"]);
    }

    private void InitSpecific(JToken deserializedSkill)
    {
        debufValue = (int)deserializedSkill["debufValue"];
        damage = (int)deserializedSkill["damage"];
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
        ParameterChangeV2<LivingPlaceable, float> debuf = new ParameterChangeV2<LivingPlaceable, float>(-1, 0);
        ParameterChangeV2<LivingPlaceable, float> rebuf = new ParameterChangeV2<LivingPlaceable, float>(0, 0, 2, true, ActivationType.BEGINNING_OF_TURN);
        debuf.Launcher = caster;
        target.DispatchEffect(debuf);
        target.DispatchEffect(rebuf);
    }

    public override void Preview(NetIdeable target)
    {
        Debug.LogError("Preview not implemented");
    }
}
