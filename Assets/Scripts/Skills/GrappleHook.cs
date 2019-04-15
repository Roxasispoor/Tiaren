using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : Skill
{
    MoveEffect MoveEffect { get { return (MoveEffect)effects[0]; } }

    private Vector3Int offset;

    public GrappleHook(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a grapple skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["GrappleHook"]);
        Init(deserializedSkill["GrappleHook"]);
    }

    protected override void Init(JToken deserializedSkill)
    {
        base.Init(deserializedSkill);

        effects = new List<Effect>();
        effects.Add(new MoveEffect());
    }

    public override void Preview(NetIdeable target)
    {
        offset = FindDestinationOffset(GameManager.instance.PlayingPlaceable, (StandardCube)target);
        FXManager.instance.Grapplepreview((StandardCube)target, offset);
    }

    public override void UnPreview(NetIdeable target)
    {
        FXManager.instance.GrappleUnpreview();
    }


    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        if(null == target as StandardCube)
        {
            return false;
        }
        if(caster.GetPosition().x == target.GetPosition().x && caster.GetPosition().z == target.GetPosition().z)
        {
            return false;
        }
        return true;
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        return PatternCreate(position, vect);
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        Debug.Log(offset.ToString());
        MoveEffect.Direction = (target.GetPosition() + offset) - caster.GetPosition();
        caster.DispatchEffect(MoveEffect);
    }

    private Vector3Int FindDestinationOffset(LivingPlaceable caster, StandardCube target)
    {
        if(null == Grid.instance.GetPlaceableFromVector(target.GetPosition() + new Vector3Int(0,1,0)))
        {
            return new Vector3Int(0, 1, 0);
        }
        return new Vector3Int();
    }
}
