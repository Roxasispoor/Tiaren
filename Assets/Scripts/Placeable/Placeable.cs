using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;
using UnityEngine.EventSystems;

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
    public Material oldMaterial;
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
        if (GameManager.instance.state == States.Move)
        {
            // Debug.Log(EventSystem.current.IsPointerOverGameObject());
            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonUp(0) && this.walkable)
            {
                if (GameManager.instance.playingPlaceable.Player.isLocalPlayer)
                {
                    Debug.Log("You have authority to ask for a move");
                    //Vector3 destination = this.GetPosition();
                    Vector3[] path = GameManager.instance.GetPathFromClicked(this);//Check and move on server
                    GameManager.instance.playingPlaceable.Player.CmdMoveTo(path);
                    // GameManager.instance.CheckIfAccessible(this);
                }
            }
        }
        else if (GameManager.instance.state == States.UseSkill)
        {
            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonUp(0))
            {
                if (GameManager.instance.playingPlaceable.Player.isLocalPlayer)
                {
                    Debug.Log("You have authority to ask to act");
                    GameManager.instance.playingPlaceable.player.CmdUseSkill(GameManager.instance.playingPlaceable.Skills.FindIndex(GameManager.instance.activeSkill.Equals), netId);
                    //GameManager.instance.activeSkill.Use(GameManager.instance.playingPlaceable, new List<Placeable>(){this});
                }
            }
        }
    }

    private void Awake()
    {
        //WARNING: NEVER CALLED BY CHILDREN (BECAUSE ABSTRACT?)

     
    }
    /*public void Start()
    {
        Comma
        this.netId=this.
    }*/
    /// <summary>
    /// On dispatch selon Living et placeable
    /// </summary>
    /// <param name="effect"></param>
    public virtual void DispatchEffect(Effect effect)
    {
        effect.TargetAndInvokeEffectManager(this);
    }
}
