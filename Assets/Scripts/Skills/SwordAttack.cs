using System;
using System.Collections.Generic;
using UnityEngine;

class SwordAttack: Skill
{
    [SerializeField]
    private int power;

    public SwordAttack(string JSON): base(JSON)
    {
        dynamic deserializedSkill = Newtonsoft.Json.JsonConvert.DeserializeObject(JSON);
        base.Init((SkillInfo)deserializedSkill.SwordAttack);
        InitSpecific(deserializedSkill.SwordAttack);
    }

    protected void InitSpecific(dynamic deserializedSkill)
    {
        power = (int)deserializedSkill.power;
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
        List<Placeable> targetableunits = new List<Placeable>(vect);
        foreach (LivingPlaceable Character in vect)
        {
            Vector3 Pos = Character.transform.position;
            if (Math.Abs(Pos.y - position.y) > 1 || Math.Abs(Pos.x - position.x) > 1 || Math.Abs(Pos.z - position.z) > 1)
                targetableunits.Remove(Character);
        }
        return targetableunits;
    }
}

