using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PiercingShot : Skill
{

    public float power;

    public PiercingShot(string JSON) : base(JSON)
    {
        dynamic skills = JsonConvert.DeserializeObject(JSON);
        SkillDataFromJSON info = (SkillDataFromJSON)skills.PushSkill;
        base.Init(info);
        InitSpecific(skills);
    }

    private void InitSpecific(dynamic skills)
    {
        power = skills.PiercingShot.power;
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
        DamageCalculated damageCalculated = new PiercingDamageEffect(power, DamageCalculated.DamageScale.DEXT);
        damageCalculated.Launcher = caster;
        target.DispatchEffect(damageCalculated);
    }
}
