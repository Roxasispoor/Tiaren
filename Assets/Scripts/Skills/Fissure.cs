using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

class Fissure : Skill
{

    [SerializeField]
    private int sizeZone;

    DestroyBloc DestroyBlockEffect { get { return (DestroyBloc)effects[0]; } }

    public Fissure(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a fissure skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        Init(deserializedSkill["Fissure"]);
    }

    protected override void Init(JToken deserializedSkill)
    {
        base.Init(deserializedSkill);

        sizeZone = (int)deserializedSkill["sizeZone"];


        effects = new List<Effect>();

        effects.Add(new DestroyBloc(2));
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        return true;
    }


    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        DestroyBlockEffect.Launcher = caster;

        List<Placeable> affectedPlaceable = Skill.PatternUseLine((Placeable)target, false, sizeZone);


        foreach (Placeable placeable in affectedPlaceable)
        {
            StandardCube cube = placeable as StandardCube;
            if (cube)
            {
                cube.DispatchEffect(DestroyBlockEffect);
            }
        }

        target.DispatchEffect(DestroyBlockEffect);
    }

    public override void Preview(NetIdeable target)
    {
        List<Placeable> affectedPlaceable = Skill.PatternUseLine((Placeable)target, true, sizeZone);

        

        foreach (Placeable placeable in affectedPlaceable)
        {
            StandardCube cube = placeable as StandardCube;
            if (cube)
            {
                DestroyBlockEffect.Preview(cube);
            }
        }
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        vect = PatternCreateTop(position, vect);
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

