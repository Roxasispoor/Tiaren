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
    private List<Placeable> affectedPlaceable = new List<Placeable>();

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
        affectedPlaceable = Skill.PatternUseSphere((Placeable)target, sizezone);
        
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
        }
    }

    public override void UnPreview(NetIdeable target)
    {
        foreach (Placeable placeable in affectedPlaceable)
        {
            if (placeable.IsLiving())
            {
                DamageEffect.ResetPreview(placeable);
            }
            else
            {
                DestroyEffect.ResetPreview(placeable);
            }
        }
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        return true;
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        vect =  PatternDestroy(position, vect);
        LivingPlaceable caster =(LivingPlaceable)Grid.instance.GetPlaceableFromVector(position);
        for(int i = 0; i < vect.Count; i++)
        {
            if (caster!=null && !CheckSpecificConditions(caster, vect[i]))
            {
                vect.Remove(vect[i]);
            }
        }
        return vect;
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        DamageEffect.Launcher = caster;

        affectedPlaceable = Skill.PatternUseSphere((Placeable)target, sizezone);
        
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
