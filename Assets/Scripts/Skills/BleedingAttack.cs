using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedingAttack : Skill
{
    [SerializeField]
    private int nbTurns;
    [SerializeField]
    private int damage;

    Damage DoTEffect { get { return (Damage)effects[0]; } }
    Damage DamageEffect { get { return (Damage)effects[1]; } }

    public BleedingAttack(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a bleeding skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["BleedingAttack"]);
        InitSpecific(deserializedSkill["BleedingAttack"]);
    }

    private void InitSpecific(JToken deserializedSkill)
    {
        squareShaped = true;
        damage = (int)deserializedSkill["damage"];
        nbTurns = (int)deserializedSkill["nbTurns"];
        effects = new List<Effect>();
        effects.Add(new Damage(damage, nbTurns, false, ActivationType.BEGINNING_OF_TURN));
        effects.Add(new Damage(damage, 1, false, ActivationType.INSTANT));
    }

    public override void Preview(NetIdeable target)
    {
        if (CheckConditions(GameManager.instance.PlayingPlaceable, target))
        {
            DamageEffect.Preview((Placeable)target);
        }
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
        vect = PatternSwordRange(position, vect);
        LivingPlaceable caster = (LivingPlaceable)Grid.instance.GetPlaceableFromVector(position);
        for (int i = vect.Count - 1; i >= 0; i--)
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
        target.DispatchEffect(DoTEffect.Clone());
        target.DispatchEffect(DamageEffect.Clone());
    }
}
