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
        Init(deserializedSkill["SwordAttack"]);
    }

    protected override void Init(JToken deserializedSkill)
    {
        squareShaped = true;

        base.Init(deserializedSkill);

        power = (float)deserializedSkill["power"];

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
        vect = PatternSwordRange(position, vect);
        LivingPlaceable caster = (LivingPlaceable)Grid.instance.GetPlaceableFromVector(position);
        for (int i = 0; i < vect.Count; i++)
        {
            if (caster != null && !CheckSpecificConditions(caster, vect[i]))
            {
                vect.Remove(vect[i]);
            }
        }
        return vect;
    }
}

