using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSkill : Skill
{
    [SerializeField]
    private float power;
    [SerializeField]
    private int sizezone;

    DamageCalculated DamageEffect { get { return (DamageCalculated)effects[0]; } }
    DestroyBloc DestroyEffect { get { return (DestroyBloc)effects[1]; } }

    public FireballSkill(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a fireball skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["FireballSkill"]);
        InitSpecific(deserializedSkill["FireballSkill"]);
    }

    private void InitSpecific(JToken deserializedSkill)
    {
        power = (float)deserializedSkill["power"];
        effects = new List<Effect>();
        effects.Add(new DamageCalculated(power, DamageCalculated.DamageScale.MAG));
        effects.Add(new DestroyBloc());
    }

    public override void Preview(NetIdeable target)
    {
        List<Placeable> affectedPlaceable = Skill.((Placeable)target);

        foreach (Placeable placeable in affectedPlaceable)
        {
            LivingPlaceable fleshyTarget = placeable as LivingPlaceable;
            if (fleshyTarget)
            {
                DamageEffect.ResetPreview(fleshyTarget);
            }
        }
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        throw new System.NotImplementedException();
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        throw new System.NotImplementedException();
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        throw new System.NotImplementedException();
    }
}
