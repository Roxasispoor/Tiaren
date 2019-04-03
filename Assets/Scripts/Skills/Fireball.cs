using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Skill
{
    [SerializeField]
    private float power;
    [SerializeField]
    private int sizezone;

    DamageCalculated DamageEffect { get { return (DamageCalculated)effects[0]; } }
    DestroyBloc DestroyEffect { get { return (DestroyBloc)effects[1]; } }

    public Fireball(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a fireball skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["Fireball"]);
        InitSpecific(deserializedSkill["Fireball"]);
    }

    private void InitSpecific(JToken deserializedSkill)
    {
        power = (float)deserializedSkill["power"];
        sizezone = (int)deserializedSkill["sizeZone"];
        effects = new List<Effect>();
        effects.Add(new DamageCalculated(power, DamageCalculated.DamageScale.MAG));
        effects.Add(new DestroyBloc());
    }

    public override void Preview(NetIdeable target)
    {
        List<Placeable> affectedPlaceable = Skill.PatternUseSphere((Placeable)target, sizezone);

        /*
        foreach (Placeable placeable in affectedPlaceable)
        {
            if (placeable.IsLiving())
            {
                DamageEffect.Preview(placeable);
            }
            else
            {
                DestroyEffect.Preview(placeable);
            }
        }*/
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        return true;
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        return PatternDestroy(position, vect);
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        DamageEffect.Launcher = caster;

        List<Placeable> affectedPlaceable = Skill.PatternUseSphere((Placeable)target, sizezone);
        
        foreach (Placeable placeable in affectedPlaceable)
        {
            if (placeable.IsLiving())
            {
                ((LivingPlaceable)placeable).DispatchEffect(DamageEffect);
            }
            else
            {
                ((StandardCube)placeable).DispatchEffect(DestroyEffect);
            }
        }
    }
}
