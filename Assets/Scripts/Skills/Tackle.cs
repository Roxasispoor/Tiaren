using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tackle : Skill
{
    private float power;

    Push PushEffect { get { return (Push)effects[0]; } }

    MoveEffect MoveEffect { get { return (MoveEffect)effects[1]; } }

    DamageCalculated DamageEffect { get { return (DamageCalculated)effects[2]; } }

    DamageCalculated ReboundEffect { get { return (DamageCalculated)effects[3]; } }

    public Tackle(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a tackle skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["Tackle"]);
        InitSpecific(deserializedSkill["Tackle"]);
    }

    private void InitSpecific(JToken deserializedSkill)
    {
        power = (int)deserializedSkill["power"];
        effects = new List<Effect>();
        effects.Add(new Push(1, 10));
        effects.Add(new MoveEffect());
        effects.Add(new DamageCalculated(power, DamageCalculated.DamageScale.STR));
        effects.Add(new DamageCalculated(power/2, DamageCalculated.DamageScale.STR));
    }

    public override void Preview(NetIdeable target)
    {

    }

    public override void UnPreview(NetIdeable target)
    {

    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        if(target.GetPosition().x == caster.GetPosition().x || target.GetPosition().z == caster.GetPosition().z)
        {
            return true;
        }
        return false;
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        LivingPlaceable caster = (LivingPlaceable)Grid.instance.GetPlaceableFromVector(position);
        for (int i = vect.Count - 1; i >= 0; i--)
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
        int travelDistance = 0;
        int pushDistance = 0;

        Vector3Int direction = FindDirection(caster, target);
        List<Placeable> colisions = ColisionSearch(caster, target, direction);
        MoveEffect.UpdateTransform = true;
        MoveEffect.Target = caster;
        MoveEffect.Launcher = caster;
        Placeable outsiderangeColision = Grid.instance.GetPlaceableFromVector(caster.GetPosition() + direction * (MaxRange + 1));

        if (colisions.Count == 0)
        {
            MoveEffect.Direction =  direction * CorrectOutbound(caster, direction, MaxRange);
            caster.DispatchEffect(MoveEffect);
        }
        else if (!colisions[0].IsLiving())
        {
            travelDistance = GetTravelDistanceWithBlock(caster, (StandardCube)colisions[0], direction);

            MoveEffect.Direction = direction * travelDistance;

            PushEffect.Launcher = caster;
            colisions[0].DispatchEffect(PushEffect);

            caster.DispatchEffect(MoveEffect);
        }
        else if (colisions[0].IsLiving())
        {
            if(colisions.Count > 1)
            {
                pushDistance = GetTravelDistanceWithLiving((LivingPlaceable)colisions[0], colisions[1], direction);
                travelDistance = GetTravelDistanceWithLiving(caster, colisions[0], direction) + pushDistance;

                MoveEffect.Direction = direction * travelDistance;
                caster.DispatchEffect(MoveEffect);

                MoveEffect.Target = (LivingPlaceable)colisions[0];
                MoveEffect.Launcher = (LivingPlaceable)colisions[0];
                MoveEffect.Direction = direction * pushDistance;
                ((LivingPlaceable)colisions[0]).DispatchEffect(DamageEffect);
                ((LivingPlaceable)colisions[0]).DispatchEffect(MoveEffect);
                if (colisions[1].IsLiving())
                {
                    ((LivingPlaceable)colisions[1]).DispatchEffect(ReboundEffect);
                }
                else
                {
                    ((LivingPlaceable)colisions[0]).DispatchEffect(ReboundEffect);
                }
            }
            else
            {
                if(outsiderangeColision == null)
                {
                    pushDistance = MaxRange - GetTravelDistanceWithLiving(caster, colisions[0], direction);
                    pushDistance = CorrectOutbound(colisions[0], direction, pushDistance);
                    MoveEffect.Target = (LivingPlaceable)colisions[0];
                    MoveEffect.Launcher = (LivingPlaceable)colisions[0];
                    MoveEffect.Direction = direction * pushDistance;
                    ((LivingPlaceable)colisions[0]).DispatchEffect(DamageEffect);
                    ((LivingPlaceable)colisions[0]).DispatchEffect(MoveEffect);

                    MoveEffect.Target = caster;
                    MoveEffect.Launcher = caster;
                    travelDistance = GetTravelDistanceWithLiving(caster, colisions[0], direction);
                    MoveEffect.Direction = direction * travelDistance;
                    caster.DispatchEffect(MoveEffect);
                }
                else
                {
                    pushDistance = MaxRange - GetTravelDistanceWithLiving(caster, colisions[0], direction);
                    MoveEffect.Direction = direction * (MaxRange -1);
                    caster.DispatchEffect(MoveEffect);

                    MoveEffect.Target = (LivingPlaceable)colisions[0];
                    MoveEffect.Launcher = (LivingPlaceable)colisions[0];
                    MoveEffect.Direction = direction * pushDistance;
                    ((LivingPlaceable)colisions[0]).DispatchEffect(DamageEffect);
                    ((LivingPlaceable)colisions[0]).DispatchEffect(MoveEffect);
                    if (outsiderangeColision.IsLiving())
                    {
                        outsiderangeColision.DispatchEffect(ReboundEffect);
                    }
                    else
                    {
                        ((LivingPlaceable)colisions[0]).DispatchEffect(ReboundEffect);
                    }
                }
            }
        }
    }

    private int CorrectOutbound(Placeable placeable, Vector3Int direction, int distance)
    {
        if (Grid.instance.CheckRange(placeable.GetPosition() + direction * distance))
        {
            return distance;
        }
        if(placeable.GetPosition().x + direction.x * distance >= Grid.instance.sizeX)
        {
            return Grid.instance.sizeX - placeable.GetPosition().x - 1;
        }
        if (placeable.GetPosition().x + direction.x * distance < 0)
        {
            return placeable.GetPosition().x;
        }
        if (placeable.GetPosition().z + direction.z * distance >= Grid.instance.sizeZ)
        {
            return Grid.instance.sizeZ - placeable.GetPosition().z - 1;
        }
        if (placeable.GetPosition().z + direction.z * distance < 0)
        {
            return placeable.GetPosition().z;
        }
        Debug.LogError("not inbound but doesn't go out on either x or z");
        return 0;
    }

    private int GetTravelDistanceWithBlock(LivingPlaceable movingPiece, StandardCube obstacle, Vector3Int direction)
    {
        Vector3Int diff = movingPiece.GetPosition() - obstacle.GetPosition();
        if (Grid.instance.GetPlaceableFromVector(obstacle.GetPosition() + direction) == null 
            && Grid.instance.CheckRange(obstacle.GetPosition() + direction))
        {
            return (int)(Math.Abs(diff.x + diff.z));
        }
        else
        {
            return (int)(Math.Abs(diff.x + diff.z) - 1);
        }
    }

    private int GetTravelDistanceWithLiving(LivingPlaceable movingPiece, Placeable obstacle, Vector3Int direction)
    {
        Vector3Int diff = movingPiece.GetPosition() - obstacle.GetPosition();
        return (int)(Math.Abs(diff.x + diff.z) -1);
    }


    private Vector3Int FindDirection(LivingPlaceable caster, NetIdeable target)
    {
        Vector3Int direction = new Vector3Int();
        if (caster.GetPosition().x == target.GetPosition().x)
        {
            if (caster.GetPosition().z < target.GetPosition().z)
                direction = new Vector3Int(0, 0, 1);
            else
                direction = new Vector3Int(0, 0, -1);
        }
        else if (caster.GetPosition().z == target.GetPosition().z)
        {
            if (caster.GetPosition().x < target.GetPosition().x)
                direction = new Vector3Int(1, 0, 0);
            else
                direction = new Vector3Int(-1, 0, 0);
        }
        return direction;
    }

    /// <summary>.
    /// Searches for the objecs that might be affected
    /// If returned list is empty then no encounter
    /// otherwise list contains elemnts in order of discovery
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private List<Placeable> ColisionSearch(LivingPlaceable caster, NetIdeable target, Vector3Int direction)
    {
        int range = MaxRange;
        Placeable placeableToCheck;
        bool livingSeen = false;
        List<Placeable> colisions = new List<Placeable>();

        for(int i = 1; i <= MaxRange; i++)
        {
            placeableToCheck = Grid.instance.GetPlaceableFromVector((caster.GetPosition() + direction * i));
            if (placeableToCheck != null)
            {
                if (placeableToCheck.IsLiving() && !livingSeen)
                {
                    colisions.Add(placeableToCheck);
                    livingSeen = true;
                }
                else if (!placeableToCheck.IsLiving() || livingSeen)
                {
                    colisions.Add(placeableToCheck);
                    break;
                }
            }
        }
        return colisions;
    }
}
