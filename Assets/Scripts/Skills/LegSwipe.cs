using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegSwipe : Skill
{
    int debufValue;
    int damage;
    string field;

    ParameterChangeV2<LivingPlaceable, float>  buff { get { return (ParameterChangeV2<LivingPlaceable, float>)effects[0]; } }
    ParameterChangeV2<LivingPlaceable, float>  unbuff { get { return (ParameterChangeV2<LivingPlaceable, float>)effects[1]; } }

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
        
        debufValue = (int)deserializedSkill["debufValue"];
        damage = (int)deserializedSkill["damage"];
        field = (string)deserializedSkill["fieldName"];
        Debug.Log(field);
        effects = new List<Effect>();
        effects.Add(new ParameterChangeV2<LivingPlaceable, float>(-debufValue, 0, 0, false, ActivationType.INSTANT));
        effects.Add(new ParameterChangeV2<LivingPlaceable, float>(debufValue, 0, 3, true, ActivationType.BEGINNING_OF_TURN));
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
        buff.Launcher = caster;
        unbuff.Launcher = caster;
        target.DispatchEffect(buff.Clone());
        target.DispatchEffect(unbuff.Clone());
    }

    public override void Preview(NetIdeable target)
    {
        Debug.LogError("Preview not implemented");
    }
}
