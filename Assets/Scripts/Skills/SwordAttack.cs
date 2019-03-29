using System;
using System.Collections.Generic;
using UnityEngine;

class SwordAttack: Skill
{
    [SerializeField]
    private int power;

    DamageCalculated damageEffect { get { return (DamageCalculated)effects[0]; } }

    public SwordAttack(string JSON): base(JSON)
    {
        Newtonsoft.Json.Linq.JObject deserializedSkill = Newtonsoft.Json.Linq.JObject.Parse(JSON);
        base.Init(deserializedSkill["SwordAttack"]);
        InitSpecific(deserializedSkill["SwordAttack"]);
    }

    protected void InitSpecific(Newtonsoft.Json.Linq.JToken deserializedSkill)
    {
        power = (int)deserializedSkill["power"];

        effects = new List<Effect>();
        effects.Add(new DamageCalculated(power, DamageCalculated.DamageScale.STR));
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
        damageEffect.Launcher = caster;
        target.DispatchEffect(damageEffect);
    }

    public override void Preview(NetIdeable target)
    {
        if (CheckConditions(GameManager.instance.PlayingPlaceable, target))
        {
            damageEffect.Preview((Placeable) target);
        }
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

