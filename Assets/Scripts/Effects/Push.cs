using System;
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
    private float damage;
    [SerializeField]
    private float nbCases;
    [SerializeField]
    protected Vector3 direction;
    [SerializeField]
    private bool isDirectionFromPosition;
    [SerializeField]
    private bool doesHeightCount;
    [SerializeField]
    private const float pushSpeed= 1f;
   
    
    public Push(Push other) : base(other)
    {
        this.damage = other.damage;
        this.nbCases = other.nbCases;
        this.direction = other.direction;
        this.isDirectionFromPosition = other.isDirectionFromPosition;
        this.doesHeightCount = other.doesHeightCount;
    }
    public Push( int nbCases, float damage,bool doesHeightCount) : this(nbCases, damage)
    {
        this.doesHeightCount =doesHeightCount;
        isDirectionFromPosition = true;
    }

    /// <summary>
    /// create a Push
    /// </summary>
    /// <param name="target">target of blow</param>
    /// <param name="launcher">launcher of blow</param>
    public Push(Placeable target, Placeable launcher, int nbCases,float damage) : base(target, launcher)
    {
        this.nbCases = nbCases;
        this.damage = damage;
        isDirectionFromPosition = true;
      }

    public Push(int nbCases, float damage,Vector3 direction) :this(nbCases,damage)
    {

        this.direction = direction;
        isDirectionFromPosition = false;
        doesHeightCount = true;
    }
    public Push( int nbCases, float damage)
    {
        this.nbCases = nbCases;
        this.damage = damage;
    }
    public override Effect Clone()
    {
        return new Push(this);
    }
    private Vector2Int CalculateDelta()
    {
        int dx = 0, dz = 0;
        if (direction.x > 0.38268343236) //Trace an hexakaidecan, to chose from, 360/16=22.5; sin(22.5)=0.38268343236
        {
            dx = 1;
        }
        else if (direction.x < -0.38268343236)
        {
            dx = -1;
        }
        if (direction.z > 0.38268343236)
        {
            dz = 1;
        }
        else if (direction.z < -0.38268343236)
        {
            dz = -1;
        }
        return new Vector2Int(dx, dz);
    }
    public virtual void GetDirection()
    {
        direction=Target.GetPosition() - Launcher.GetPosition();
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

                GetDirection();
              
                direction.Normalize();
            if(doesHeightCount)
            {
                direction.z = 0;
            }
        }
        //float angle = Mathf.Atan2(direction.y, direction.x); // we didive by the length of interval and round up
        //‎int hexakaidecan = (int) (angle / (22.5 * Mathf.Deg2Rad));
        Vector2Int delta = CalculateDelta();
        int distance = (int)nbCases;
        if (delta.x*delta.y!=0)// si les deux sont à 1
        {
            distance = (int)(nbCases / 1.4142 + 0.5); //Round up the euclidian distance
        }
        Placeable directCollision=null ;
        List<Placeable> diagonalCollisions = new List<Placeable>();
        List<Vector3> path= CheckPath(distance, delta,out directCollision, out diagonalCollisions);
        
        //Make damage and chek dodge conditions, destructions.... to modify according gameplay decided
        if (directCollision != null && directCollision.IsLiving())
        {
            EffectManager.instance.DirectAttack(new Damage((LivingPlaceable)directCollision,Launcher,damage));
        }
        foreach(Placeable diagcoll in diagonalCollisions)
        {
            if (diagcoll != null && diagcoll.IsLiving())
            {
                EffectManager.instance.DirectAttack(new Damage((LivingPlaceable)diagcoll, Launcher, damage));
            }
        }
        if(path.Count>0)
        {
            Grid.instance.MoveBlock(Target, new Vector3Int((int)path[path.Count - 1].x, (int)path[path.Count - 1].y, (int)path[path.Count - 1].z),GameManager.instance.isServer);
            if (GameManager.instance.isClient)
            { 
                GameManager.instance.RemoveBlockFromBatch(Target);
                //Could be either player, really...
                path.Insert(0, Target.GetPosition());
                // trigger visual effect and physics consequences
                Vector3 pos = Target.transform.position;
                GameManager.instance.PlayingPlaceable.gameObject.transform.LookAt(Target.transform);
                GameManager.instance.playingPlaceable.Player.StartMoveAlongBezier(path, Target, pushSpeed, false);
                Grid.instance.ConnexeFall((int)pos.x, (int)pos.y, (int)pos.z);
            }
        }

        
        //cmdMoveBlock(Target
    }
    //Todo check que ça sort pas du terrain...
    public List<Vector3> CheckPath(int distance, Vector2Int delta,out Placeable directCollision, out List<Placeable> diagonalCollisions)
    {
        directCollision = null;
        List<Vector3> positions = new List<Vector3>();
        diagonalCollisions = new List<Placeable>();
        bool isDiagonal = delta.x * delta.y != 0;
        for (int i = 1; i < distance; i++)
        {
            Vector3Int positionCurrent = new Vector3Int(Target.GetPosition().x + delta.x * i, Target.GetPosition().y,
                Target.GetPosition().z + delta.y * i);
            if (!Grid.instance.CheckNull(positionCurrent)) 
            {
                directCollision=Grid.instance.GridMatrix[Target.GetPosition().x + delta.x * i, Target.GetPosition().y, Target.GetPosition().z + delta.y * i];
            }
            else
            {
                //check diagonal
                if (isDiagonal && !Grid.instance.CheckNull(
                    new Vector3Int(Target.GetPosition().x + delta.x * (i-1), Target.GetPosition().y, Target.GetPosition().z + delta.y * i)))
                {
                    diagonalCollisions.Add(Grid.instance.GridMatrix[Target.GetPosition().x + delta.x * (i - 1),
                Target.GetPosition().y, Target.GetPosition().z + delta.y * i]);

                }
                if (isDiagonal && !Grid.instance.CheckNull(
                    new Vector3Int(Target.GetPosition().x + delta.x * i, Target.GetPosition().y, Target.GetPosition().z + delta.y * (i-1))))
                {
                    diagonalCollisions.Add(Grid.instance.GridMatrix[Target.GetPosition().x + delta.x * i,
                Target.GetPosition().y, Target.GetPosition().z + delta.y * (i - 1)]);

                }


            }
            if (directCollision || diagonalCollisions.Count > 0)
            {
                break;
            }
            else
            {
                if(Grid.instance.CheckRange(
                    new Vector3Int(Target.GetPosition().x + delta.x * i, Target.GetPosition().y, Target.GetPosition().z + delta.y * i)))
                    { 
                positions.Add(new Vector3(Target.GetPosition().x + delta.x * i, Target.GetPosition().y, Target.GetPosition().z + delta.y * i));
                }
            }
        }
        return positions;
    }

}
