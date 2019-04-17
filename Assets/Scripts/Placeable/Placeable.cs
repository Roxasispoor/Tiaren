using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents something able to fill a bloc of the grid
/// </summary>
[Serializable]
public abstract class Placeable : NetIdeable
{

    /// <summary>
    /// Can we walk on it.
    /// </summary>
    public bool walkable;

    /// <summary>
    /// Is the placeable movable (for push, etc.).
    /// </summary>
    public bool movable;

    /// <summary>
    /// Is the placeable traversable by another Placeable.
    /// </summary>
    public TraversableType traversableType;

    /// <summary>
    /// Is the placeable traversable by bullets.
    /// </summary>
    public TraversableType traversableBullet;

    /// <summary>
    /// The speed used for the animation.
    /// </summary>
    private float animationSpeed = 2.5f;

    /// <summary>
    /// Previous material (used for highlighting)
    /// </summary>
    // AMELIORATION: move to cube ?
    [NonSerialized]
    public Material oldMaterial;

    /// <summary>
    /// How it react to gravity.
    /// </summary>
    // TODO: Changed it to protected -> Need to create a Spawn prefab
    public GravityType gravityType;

    /// <summary>
    /// How it react when crushed.
    /// </summary>
    protected CrushType crushable;

    /// <summary>
    /// Used for gravity - Indicates if gravity tests have been already done on placeable.
    /// </summary>
    public bool explored;

    /// <summary>
    /// Used for gravity - Indicates if it is connected to the ground.
    /// </summary>
    public bool grounded;

    /// <summary>
    /// Current coroutine used for the movement.
    /// </summary>
    public Coroutine moveCoroutine;

    /// <summary>
    /// player who owns the placeable. players, neutral monsters, and null (independant blocs)
    /// </summary>
    public Player player;

    /// <summary>
    /// How it react to gravity.
    /// </summary>
    public GravityType GravityType
    {
        get
        {
            return gravityType;
        }
    }

    /// <summary>
    /// How it react when crushed.
    /// </summary>
    public CrushType Crushable
    {
        get
        {
            return crushable;
        }
    }

    /// <summary>
    /// player who owns the placeable. players, neutral monsters, and null (independant blocs)
    /// </summary>
    public virtual Player Player
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

    /// <summary>
    /// The speed used for the animation.
    /// </summary>
    public float AnimationSpeed
    {
        get
        {
            return animationSpeed;
        }
    }

    /// <summary>
    /// Return true if this Placeable is Living.
    /// </summary>
    /// <returns></returns>
    public override bool IsLiving()
    {
        return false;
    }

    /// <summary>
    /// method to call for destroying object
    /// </summary>
    public abstract void Destroy();

    public abstract void Highlight();

    public virtual void HighlightForAttacks()
    {

    }

    public abstract void UnHighlight();

    protected virtual void Awake()
    {
    }

    /// <summary>
    /// To call when creating a Placeable to initialize needed values
    /// </summary>
    public virtual void Init()
    {

    }

    /// <summary>
    /// On dispatch selon Living et placeable
    /// </summary>
    /// <param name="effect"></param>
    public override void DispatchEffect(Effect effect)
    {
        effect.TargetAndInvokeEffectManager(this);
    }
}
