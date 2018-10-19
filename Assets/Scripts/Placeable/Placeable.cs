using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

/// <summary>
/// Represents something able to fill a bloc of the grid
/// </summary>

public abstract class Placeable : NetworkBehaviour
{
    public int serializeNumber;
    private bool walkable;
    private List<Effect> onWalkEffects;
    private bool movable;
    private bool destroyable;
    private TraversableType tangible;
    private TraversableType traversableBullet;
    public Color colorOfObject;
    private float animationSpeed;
   
    private GravityType gravityType;
    private CrushType crushable;
    public bool explored;
    private List<Effect> onDestroyEffects;
    private List<HitablePoint> hitablePoints;
    private List<Effect> onStartTurn;
    private List<Effect> onEndTurn;
    /// <summary>
    /// player who owns the placeable. players, neutral monsters, and null (independant blocs)
    /// </summary>
    public Player player;

    public Vector3Int GetPosition()
    {
        return new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);

    }

    public bool Walkable
    {
        get
        {
            return walkable;
        }

        set
        {
            walkable = value;
        }
    }

    public bool Movable
    {
        get
        {
            return movable;
        }

        set
        {
            movable = value;
        }
    }

   

    /// <summary>
    /// indicates if gravity tests have been already done on placeable 
    /// </summary>
    public bool Explored
    {
        get
        {
            return explored;
        }

        set
        {
            explored = value;
        }
    }




    public List<Effect> OnStartTurn
    {
        get
        {
            return onStartTurn;
        }

        set
        {
            onStartTurn = value;
        }
    }

    public List<Effect> OnEndTurn
    {
        get
        {
            return onEndTurn;
        }

        set
        {
            onEndTurn = value;
        }
    }

    public List<Effect> OnDestroyEffects
    {
        get
        {
            return onDestroyEffects;
        }

        set
        {
            onDestroyEffects = value;
        }
    }

    public List<Effect> OnWalkEffects
    {
        get
        {
            return onWalkEffects;
        }

        set
        {
            onWalkEffects = value;
        }
    }

    public List<HitablePoint> HitablePoints
    {
        get
        {
            return hitablePoints;
        }

        set
        {
            hitablePoints = value;
        }
    }

    public TraversableType TraversableChar
    {
        get
        {
            return tangible;
        }

        set
        {
            tangible = value;
        }
    }

    public TraversableType TraversableBullet
    {
        get
        {
            return traversableBullet;
        }

        set
        {
            traversableBullet = value;
        }
    }

    public GravityType GravityType
    {
        get
        {
            return gravityType;
        }

        set
        {
            gravityType = value;
        }
    }

    public CrushType Crushable
    {
        get
        {
            return crushable;
        }

        set
        {
            crushable = value;
        }
    }

    public Player Player
    {
        get
        {
            return player;
        }

        set
        {
            player = value;
        }
    }

   

    public bool Destroyable
    {
        get
        {
            return destroyable;
        }

        set
        {
            destroyable = value;
        }
    }

    public float AnimationSpeed
    {
        get
        {
            return animationSpeed;
        }

        set
        {
            animationSpeed = value;
        }
    }




    /// <summary>
    /// Copy object
    /// </summary>
    /// <returns>Return a copy of the object</returns>
    public virtual Placeable Cloner()
    {
        var copy = (Placeable)this.MemberwiseClone();
        return copy;
    }

    /// <summary>
    /// method to call for destroying object
    /// </summary>
    public virtual void DestroyLivingPlaceable()
    {
        if (this.Destroyable)
        {
            foreach (var effet in this.OnDestroyEffects)
            {
                effet.Use();
            }
        }
        Destroy(this);
        Destroy(this.gameObject);
    }
    /// <summary>
    /// allows shoot and shifting
    /// </summary>
    void OnMouseOver()
    {


        if (Input.GetMouseButtonUp(0) && this.walkable)
        {
            GameManager.instance.CheckIfAccessible(this);
        }
        else if (Input.GetMouseButtonUp(2))
        {
            GameManager.instance.ShotPlaceable = this;

        }

    }

    [ClientRpc]
    public void RpcMoveOnClient(Vector3 position)
    {
        this.transform.position = position;
    }
}
