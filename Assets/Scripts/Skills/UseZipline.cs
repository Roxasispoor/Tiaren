using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

class UseZipline : Skill
{

    public static UseZipline reference;
    
    private ZipLineTeleport ZipLineTeleportEffect
    {
        get
        {
            return (ZipLineTeleport)effects[0];
        }
        set
        {
            effects[0] = value;
        }
    }

    public UseZipline(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a UseZipline skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        Init(deserializedSkill["UseZipline"]);
        oneClickUse = true;
    }
    
    protected UseZipline(): base()
    {
        oneClickUse = true;
    }

    public static UseZipline CreateNewInstanceFromReferenceAndSetTarget(ZipLine zipLine)
    {
        UseZipline newUseZipline = new UseZipline();
        newUseZipline.CopyMainVariables(UseZipline.reference);
        newUseZipline.effects.Add(new ZipLineTeleport(zipLine));
        //newUseZipline.ZipLineTeleportEffect.Target = zipLine;
        return newUseZipline;
    }

    protected override void Init(JToken deserializedSkill)
    {
        base.Init(deserializedSkill);

        //effects.Add(new MoveEffect());
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        StandardCube blocUnder = Grid.instance.GetPlaceableFromVector(caster.GetPosition() + Vector3.down) as StandardCube;
        Placeable placeableAbove = Grid.instance.GetPlaceableFromVector(caster.GetPosition() + Vector3.up);

        if (blocUnder == null || !blocUnder.isConstructableOn)
        {
            Debug.LogError("Trying to " + skillName + " but the bloc under does not exist or is not constructable on");
            return false;
        }
        if (placeableAbove != null)
        {
            return false;
        }
        return true;
    }


    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        ZipLineTeleportEffect.Launcher = caster;
        ZipLineTeleportEffect.Target.DispatchEffect(ZipLineTeleportEffect);

    }

    public override void Preview(NetIdeable target)
    {
        Debug.LogError(SkillName + ": no preview");
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        return new List<Placeable>() { Grid.instance.GetPlaceableFromVector(position) };
    }

    public void SetStart(ZipLine start)
    {
        ZipLineTeleportEffect.target = start;
    }
}

