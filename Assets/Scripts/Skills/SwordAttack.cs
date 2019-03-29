using System;
using System.Collections.Generic;
using UnityEngine;

class SwordAttack: Skill
{
    [SerializeField]
    private float power;

    public SwordAttack(string JSON): base(JSON)
    {
        dynamic deserializedSkill = Newtonsoft.Json.JsonConvert.DeserializeObject(JSON);
        base.Init((SkillDataFromJSON)deserializedSkill.SwordAttack);
        InitSpecific(deserializedSkill.SwordAttack);
    }

    protected void InitSpecific(dynamic deserializedSkill)
    {
        power = deserializedSkill.power;
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


    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        
        //TODO: gérer les animations
        DamageCalculated damageCalculated = new DamageCalculated(power, DamageCalculated.DamageScale.STR);
        damageCalculated.Launcher = caster;
        target.DispatchEffect(damageCalculated);
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        return PatternSwordRange(position, vect);
    }
}

