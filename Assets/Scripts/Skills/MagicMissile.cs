using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class MagicMissile : Skill
{
    [SerializeField]
    private float power;

    DamageCalculated DamageEffect { get { return (DamageCalculated)effects[0]; } }

    public MagicMissile(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a magicmissile skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["MagicMissile"]);
        InitSpecific(deserializedSkill["MagicMissile"]);
    }

    private void InitSpecific(JToken deserializedSkill)
    {
        power = (float)deserializedSkill["power"];
        effects = new List<Effect>();
        effects.Add(new DamageCalculated(power, DamageCalculated.DamageScale.MAG));
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

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        DamageEffect.Launcher = caster;
        target.DispatchEffect(DamageEffect);
    }

    public override void Preview(NetIdeable target)
    {
        if (CheckConditions(GameManager.instance.PlayingPlaceable, target))
        {
            DamageEffect.Preview((Placeable)target);
        }
    }
}
