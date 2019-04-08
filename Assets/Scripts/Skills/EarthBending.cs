using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class EarthBending : Skill
{
    [SerializeField]
    private int nbCasePush;
    [SerializeField]
    private int damage;

    Push PushUpEffect { get { return (Push)effects[0]; } }
    Push PushInLineEffect { get { return (Push)effects[1]; } }


    public EarthBending(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a EarthBending skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["EarthBending"]);
        InitSpecific(deserializedSkill["EarthBending"]);
    }

    protected void InitSpecific(JToken deserializedSkill)
    {
        damage = (int)deserializedSkill["damage"];
        nbCasePush = (int)deserializedSkill["nbCasePush"];
        effects = new List<Effect>();
        effects.Add(new Push(1, 0, Vector3.up, false));
        effects.Add(new Push(nbCasePush, damage, false));
        PushInLineEffect.pushSpeed = 5f;
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        PushUpEffect.Launcher = caster;
        PushInLineEffect.Launcher = caster;
        target.DispatchEffect(PushUpEffect);
        target.DispatchEffect(PushInLineEffect);
    }

    public override void Preview(NetIdeable target)
    {
        Debug.LogError("Preview not implemented");
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        vect = PatternCreate(position, vect);
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

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        StandardCube cube = target as StandardCube;
        if (cube == null)
        {
            Debug.LogError("target is not a StandardCube! Not good in push");
            return false;
        }
        if (cube.movable == false)
        {
            return false;
        }
        return true;
    }
}
