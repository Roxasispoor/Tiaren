using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

/// <summary>
/// Represents something able to fill a bloc of the grid
/// </summary>

public abstract class Placeable:MonoBehaviour
{
    public int netId;
    public Batch batch;
    public static int currentMaxId=0;
    public int serializeNumber;
    private bool walkable;
    private List<Effect> onWalkEffects;
    private bool movable;
    private bool destroyable;
    private TraversableType tangible;
    private TraversableType traversableBullet;
    public Color colorOfObject;
    private float animationSpeed=1.0f;
   
    private GravityType gravityType;
    private CrushType crushable;
    public bool explored;
    private List<Effect> onDestroyEffects;
    private List<HitablePoint> hitablePoints;
    private List<Effect> onStartTurn;
    private List<Effect> onEndTurn;
    private List<Effect> attachedEffects;
    /// <summary>
    /// player who owns the placeable. players, neutral monsters, and null (independant blocs)
    /// </summary>
    public Player player;

    public Vector3Int GetPosition()
    {
        return new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);

    }
   
    public virtual bool IsLiving()
    {
        return false;
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
    /// <summary>
    /// On dispatch selon Living et placeable
    /// </summary>
    /// <param name="effect"></param>
    public virtual void DispatchEffect(Effect effect)
    {
        effect.TargetAndInvokeEffectManager(this);
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

    public List<Effect> AttachedEffects
    {
        get
        {
            return attachedEffects;
        }

        set
        {
            attachedEffects = value;
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
    public virtual void Destroy()
    {
        if (this.Destroyable)
        {
            foreach (var effect in this.OnDestroyEffects)
            {
                EffectManager.instance.UseEffect(effect);
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
        if (GameManager.instance.state == GameManager.States.Spawn)
        {
            if (Input.GetMouseButtonUp(0) && this.walkable)
            {
                
            }
        }

        if (Input.GetMouseButtonUp(0) && this.walkable)
        {
            GameManager.instance.CheckIfAccessible(this);
        }
        else if (Input.GetMouseButtonUp(2))
        {
            GameManager.instance.ShotPlaceable = this;

        }

    }

    //TODO
    public void onWalk(LivingPlaceable placeable)
    {

    }

    private void Awake()
    {

        this.OnWalkEffects = new List<Effect>();
        this.OnDestroyEffects = new List<Effect>();
        this.HitablePoints = new List<HitablePoint>();
        this.OnStartTurn = new List<Effect>();
        this.OnEndTurn = new List<Effect>();
        this.AttachedEffects = new List<Effect>();
    }
    /*public void Start()
    {
        Comma
        this.netId=this.
    }*/

}
