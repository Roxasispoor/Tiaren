using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class BowAttack : Skill
{
    public float power;

    public BowAttack(string JSON) : base(JSON)
    {
        dynamic skills = JsonConvert.DeserializeObject(JSON);
        SkillDataFromJSON info = (SkillDataFromJSON)skills.PushSkill;
        base.Init(info);
        InitSpecific(skills);
    }

    private void InitSpecific(dynamic skills)
    {
        power = skills.BowAttack.power;
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
}
