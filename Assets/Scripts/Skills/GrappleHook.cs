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
        if (CheckAccessible(GameManager.instance.PlayingPlaceable, (StandardCube)target, true))
        {
            if (offset.y == 1 || offset.y == -1)
            {
                Debug.Log("offset = " + offset);
                Vector3 offsetPreview = GameManager.instance.PlayingPlaceable.GetPosition() - (Vector3)(target.GetPosition() + offset);
                Debug.Log("offsetPreview = " + offsetPreview);
                float max = Mathf.Max(Mathf.Abs(offsetPreview.x), Mathf.Abs(offsetPreview.z));
                offsetPreview.y = 0;
                offsetPreview.x /= max;
                offsetPreview.z /= max;

                FXManager.instance.Grapplepreview((StandardCube)target, offset + offsetPreview);
            }
            else
            {
                FXManager.instance.Grapplepreview((StandardCube)target, offset);
            }
        }
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
        return PatternCreateTop(position, vect);
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        if (CheckAccessible(caster, (StandardCube)target, false))
        {
            MoveEffect.Direction = (target.GetPosition() + offset) - caster.GetPosition();
            caster.DispatchEffect(MoveEffect);
        }
    }

    //Checks accessibility of the side pointed and sets the offset used for preview and use
    public bool CheckAccessible(LivingPlaceable caster, StandardCube target, bool isPreview)
    {
        SelectionInfo curentinfo = CollectSelectionInfo(isPreview);
        Vector3 startingPoint = caster.GetPosition();

        if(null != Grid.instance.GetPlaceableFromVector(target.GetPosition() + curentinfo.face))
        {
            Debug.Log("The desired position is occupied");
            return false;
        }
        Vector3 direction = (target.GetPosition() + curentinfo.face * 0.55f) - startingPoint;
        if (target.GetPosition().y >= startingPoint.y)
        {
            if (Physics.Raycast(startingPoint, direction.normalized, direction.magnitude, LayerMask.GetMask("Cube") | LayerMask.GetMask("Totems")) && curentinfo.face != new Vector3(0, 1, 0))
            {
                Debug.Log("There is something in the way");
                return false;
            }
        }
        else
        {
            if (Physics.Raycast(startingPoint, direction.normalized, direction.magnitude, LayerMask.GetMask("Cube") | LayerMask.GetMask("Totems")))
            {
                Debug.Log("There is something in the way");
                return false;
            }
        }
        offset = Vector3Int.FloorToInt(curentinfo.face);
        return true;
    }
}
