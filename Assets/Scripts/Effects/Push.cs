using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Animation;

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
    public Vector3 direction = Vector3.zero;
    [SerializeField]
    public float pushSpeed= 1f;
    [SerializeField]
    private bool shouldApplyGravity = true;
   
    
    public Push(Push other) : base(other)
    {
        this.damage = other.damage;
        this.nbCases = other.nbCases;
        this.direction = other.direction;
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
      }

    public Push(int nbCases, int damage,Vector3 direction, bool applyGravity = true) :this(nbCases,damage)
    {
        this.direction = direction;
        shouldApplyGravity = applyGravity;
    }
    public Push( int nbCases, int damage)
    {
        this.nbCases = nbCases;
        this.damage = damage;
    }

    public override void Preview(NetIdeable target)
    {
    }
    
    public override void ResetPreview(NetIdeable target)
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
        if (direction == Vector3.zero)
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

        // TODO : no further need for List<Vector3>, maybe could only use a vector3 
        List<Vector3> path= GeneratePath(direction, distance, out placeableHitted);

        AnimationComponent animation = new pushAnimationComponent(Target, path[0], path[path.Count - 1]);

        AnimationHandler.Instance.AddComponentToCurrentAnimationBlock(animation);

        if (placeableHitted != null)
        {
            if (Target.IsLiving())
            {
                Target.DispatchEffect(new DamageCalculated(damage, DamageCalculated.DamageScale.BRUT, 0));
            }
            if (placeableHitted.IsLiving())
            {
                Target.DispatchEffect(new DamageCalculated(damage, DamageCalculated.DamageScale.BRUT, 0));
            }
        }

        if(path.Count > 1)
        {
            Vector3 targetPositionStart = Target.GetPosition();
            Grid.instance.MovePlaceable(Target, new Vector3Int((int)path[path.Count - 1].x, (int)path[path.Count - 1].y, (int)path[path.Count - 1].z), GameManager.instance.isServer);

            StandardCube targetAsCube = Target as StandardCube;
            if (targetAsCube)
            {
                GameManager.instance.RemoveBlockFromBatch(targetAsCube);
            }

            if (GameManager.instance.isClient)
            {
                Vector3 launcherPosition = Launcher.transform.position;
                Vector3 positionToLook = new Vector3(targetPositionStart.x, launcherPosition.y, targetPositionStart.z);
                //GameManager.instance.PlayingPlaceable.gameObject.transform.LookAt(positionToLook, Vector3.up);
                //GameManager.instance.PlayingPlaceable.Player.FollowPathAnimation(path, Target, null, pushSpeed, false);
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

    private class pushAnimationComponent : AnimationComponent
    {
        NetIdeable target;
        Vector3 start;
        Vector3 end;

        //TODO: get the animator in another way
        public pushAnimationComponent(NetIdeable target, Vector3 start, Vector3 end): base ()
        {
            this.target = target;
            this.start = start;
            this.end = end;
        }

        public override IEnumerator Launch()
        {
            StandardCube cube = target as StandardCube;
            if (cube != null)
            {
                GameManager.instance.RemoveBlockFromBatch(cube);
            }
            Segment lane = new Segment(start, end);
            yield return lane.makeMoveAlong(target.VisualTransform, 2f);
        }
    }
}
