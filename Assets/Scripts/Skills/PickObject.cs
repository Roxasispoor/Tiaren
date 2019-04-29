using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

class PickObject : Skill
{

    public static PickObject reference;
    
    private PickObjectOnBlockEffect PickObjectEffect
    {
        get
        {
            return (PickObjectOnBlockEffect)effects[0];
        }
        set
        {
            effects[0] = value;
        }
    }

    public PickObject(string JSON) : base(JSON)
    {
        Debug.LogError("Creating Pick the object");
        JObject deserializedSkill = JObject.Parse(JSON);
        Init(deserializedSkill["PickObject"]);
        oneClickUse = true;
    }
    
    protected PickObject(): base()
    {
        oneClickUse = true;
    }

    public static PickObject CreateNewInstanceFromReferenceAndSetTarget(ObjectOnBloc objectToPick)
    {
        PickObject newPickObject = new PickObject();
        newPickObject.CopyMainVariables(PickObject.reference);
        newPickObject.effects.Add(new PickObjectOnBlockEffect(objectToPick));
        return newPickObject;
    }

    protected override void Init(JToken deserializedSkill)
    {
        base.Init(deserializedSkill);
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        return true;
    }


    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        PickObjectEffect.Launcher = caster;
        PickObjectEffect.Target.DispatchEffect(PickObjectEffect);

    }

    public override void Preview(NetIdeable target)
    {
        Debug.Log(SkillName + ": no preview");
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        return new List<Placeable>() { Grid.instance.GetPlaceableFromVector(position) };
    }

    public void SetStart(ZipLine start)
    {
        PickObjectEffect.target = start;
    }
}

