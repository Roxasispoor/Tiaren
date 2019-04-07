﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Effect to push a bloc
/// </summary>
/// 
[Serializable]
public class Push : EffectOnPlaceable
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private float nbCases;
    [SerializeField]
    protected Vector3 direction;
    [SerializeField]
    private bool isDirectionFromPosition;
    [SerializeField]
    private bool doesHeightCount;
    [SerializeField]
    public float pushSpeed= 1f;
    [SerializeField]
    private bool shouldApplyGravity = true;
   
    
    public Push(Push other) : base(other)
    {
        this.damage = other.damage;
        this.nbCases = other.nbCases;
        this.direction = other.direction;
        this.isDirectionFromPosition = other.isDirectionFromPosition;
        this.doesHeightCount = other.doesHeightCount;
    }
    public Push( int nbCases, int damage,bool doesHeightCount) : this(nbCases, damage)
    {
        this.doesHeightCount =doesHeightCount;
        isDirectionFromPosition = true;
    }

    /// <summary>
    /// create a Push
    /// </summary>
    /// <param name="target">target of blow</param>
    /// <param name="launcher">launcher of blow</param>
    public Push(Placeable target, Placeable launcher, int nbCases,int damage) : base(target, launcher)
    {
        this.nbCases = nbCases;
        this.damage = damage;
        isDirectionFromPosition = true;
      }

    public Push(int nbCases, int damage,Vector3 direction, bool applyGravity = true) :this(nbCases,damage)
    {
        this.direction = direction;
        isDirectionFromPosition = false;
        doesHeightCount = true;
        shouldApplyGravity = applyGravity;
    }
    public Push( int nbCases, int damage)
    {
        this.nbCases = nbCases;
        this.damage = damage;
        isDirectionFromPosition = true;
    }

    public override void Preview(Placeable target)
    {
    }
    
    public override void ResetPreview(Placeable target)
    {
    }

    public override Effect Clone()
    {
        return new Push(this);
    }

    public virtual void GenerateDirectionFromLaucherAndTarget()
    {
        // Hypotesis: the Target and the Launcher are aligned
        Vector3 diff = Target.GetPosition() - Launcher.GetPosition();
        direction = diff.normalized;
    }
    /// <summary>
    /// rounds up the angle to 0,45 or 90, does not use bresenham due to complexity and non deterministic
    /// </summary>
    override
    public void Use()
    {
        if (isDirectionFromPosition)
        {
            if (Launcher==Target)
            {
                Debug.Log("The hell, push caster is push target");
            }
            GenerateDirectionFromLaucherAndTarget();
        }

        direction.Normalize();

        //if we point more than one direction
        if (!(Mathf.Abs(direction.x) == 1 ^ Mathf.Abs(direction.y) == 1 ^ Mathf.Abs(direction.z) == 1))
        {
            Debug.LogError(direction);  
            Debug.LogError("Push: ERROR try to push in a non-straight line");
        }

        int distance = (int)nbCases;

        Placeable placeableHitted;

        List<Vector3> path= GeneratePath(direction, distance, out placeableHitted);
        
        if (placeableHitted != null && placeableHitted.IsLiving())
        {
            EffectManager.instance.DirectAttack(new Damage((LivingPlaceable)placeableHitted, Launcher,damage));
        }

        if(path.Count > 1)
        {
            Vector3 targetPositionStart = Target.GetPosition();
            Grid.instance.MovePlaceable(Target, new Vector3Int((int)path[path.Count - 1].x, (int)path[path.Count - 1].y, (int)path[path.Count - 1].z), GameManager.instance.isServer);
            GameManager.instance.RemoveBlockFromBatch((StandardCube)Target);
            if (GameManager.instance.isClient)
            {
                Vector3 launcherPosition = Launcher.transform.position;
                Vector3 positionToLook = new Vector3(targetPositionStart.x, launcherPosition.y, targetPositionStart.z);
                GameManager.instance.PlayingPlaceable.gameObject.transform.LookAt(positionToLook, Vector3.up);
                GameManager.instance.PlayingPlaceable.Player.StartMoveAlongBezier(path, Target, pushSpeed, false);
            }

            if (shouldApplyGravity)
            {
                Vector3 targetPositionEnd = Target.GetPosition();

                Grid.instance.Gravity((int)targetPositionStart.x, (int)targetPositionStart.y, (int)targetPositionStart.z);
                Grid.instance.Gravity((int)targetPositionEnd.x, (int)targetPositionEnd.y, (int)targetPositionEnd.z);
            }
        }
    }
    
    public List<Vector3> GeneratePath(Vector3 direction, int distance, out Placeable collision)
    {

        List<Vector3> positions = new List<Vector3>();

        collision = null;

        positions.Add(Target.GetPosition());

        for (int i = 1; i < distance + 1; i++)
        {
            Vector3 nextPosition = Target.GetPosition() + direction * i;

            if (Grid.instance.CheckRange(nextPosition) == false)
            {
                break;
            }

            if (Grid.instance.CheckNull(nextPosition) == false) 
            {
                collision  = Grid.instance.GetPlaceableFromVector(nextPosition);
                break;
            }

            positions.Add(nextPosition);

        }


        return positions;
    }
}
