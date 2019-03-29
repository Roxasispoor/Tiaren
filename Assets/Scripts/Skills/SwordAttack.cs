using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

class SwordAttack: Skill
{
    [SerializeField]
    private float power;

    DamageCalculated DamageEffect { get { return (DamageCalculated)effects[0]; } }

    public SwordAttack(string JSON): base(JSON)
    {
        Debug.LogError("Creating a sword skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["SwordAttack"]);
        InitSpecific(deserializedSkill["SwordAttack"]);
    }

    protected void InitSpecific(JToken deserializedSkill)
    {

        effects = new List<Effect>();
        effects.Add(new DamageCalculated(power, DamageCalculated.DamageScale.STR));
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


    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        DamageEffect.Launcher = caster;
        target.DispatchEffect(DamageEffect);
    }

    public override void Preview(NetIdeable target)
    {
        if (CheckConditions(GameManager.instance.PlayingPlaceable, target))
        {
            DamageEffect.Preview((Placeable) target);
        }
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        return PatternSwordRange(position, vect);
    }
}

