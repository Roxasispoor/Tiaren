using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcrobaticSkill : Skill
{
    int debufValue;

    ParameterChangeV2<LivingPlaceable, float> buff { get { return (ParameterChangeV2<LivingPlaceable, float>)effects[0]; } }
    ParameterChangeV2<LivingPlaceable, float> unbuff { get { return (ParameterChangeV2<LivingPlaceable, float>)effects[1]; } }

    public AcrobaticSkill(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a JumpBonus skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["AcrobaticSkill"]);
        InitSpecific(deserializedSkill["AcrobaticSkill"]);

        oneClickUse = true;
    }

    private void InitSpecific(JToken deserializedSkill)
    {
        debufValue = (int)deserializedSkill["buffValue"];
        effects = new List<Effect>();
        effects.Add(new ParameterChangeV2<LivingPlaceable, float>(debufValue, 1, 0, false, ActivationType.INSTANT));
        effects.Add(new ParameterChangeV2<LivingPlaceable, float>(-debufValue, 1, 3, true, ActivationType.BEGINNING_OF_TURN));
    }

    public override void Preview(NetIdeable target)
    {
        //no preview
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
        //caster and target should be equal
        buff.Launcher = caster;
        unbuff.Launcher = caster;
        buff.Launcher = caster;
        unbuff.Launcher = caster;
        target.DispatchEffect(buff.Clone());
        target.DispatchEffect(unbuff.Clone());
    }
}
