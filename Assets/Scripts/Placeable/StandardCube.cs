using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Cube placeable standard
/// </summary>
[Serializable]
public class StandardCube : Placeable
{

    [NonSerialized]
    public Batch batch;
    protected List<Effect> onWalkEffects;

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

    protected override void Awake()
    {
        base.Awake();
        this.Walkable = true;
        
        this.Movable = true;
        this.Destroyable = true;

        this.TraversableChar = TraversableType.NOTHROUGH;
        this.TraversableBullet = TraversableType.NOTHROUGH;

        this.GravityType = GravityType.RELATED_GRAVITY;
       
        this.Crushable = CrushType.CRUSHSTAY;
        this.Explored = false;
        this.Grounded = false;
        this.onWalkEffects = new List<Effect>();
        this.OnDestroyEffects = new List<Effect>();
        this.OnStartTurn = new List<Effect>();
        this.OnEndTurn = new List<Effect>();
        this.AttachedEffects = new List<Effect>();
    }

    // Update is called once per frame
 
}
