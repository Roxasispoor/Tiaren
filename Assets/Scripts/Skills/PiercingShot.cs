using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;


public class PiercingShot : Skill
{
    [SerializeField]
    private float power;

    PiercingDamageEffect DamageEffect { get { return (PiercingDamageEffect)effects[0]; } }

    public PiercingShot(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a piercing skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["PiercingShot"]);
        InitSpecific(deserializedSkill["PiercingShot"]);
    }

    private void InitSpecific(JToken deserializedSkill)
    {
        throughblocks = true;
        power = (float)deserializedSkill["power"];
        effects = new List<Effect>();
        effects.Add(new PiercingDamageEffect(power, DamageCalculated.DamageScale.DEXT));
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        if (target as IHurtable == null)
        {
            Debug.LogError("Trying to launch an attack on a non hurtable");
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
