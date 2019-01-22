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

    new void Awake()
    {
        base.Awake();
        
        this.serializeNumber = 1;
       

        this.Walkable = true;
        
        this.Movable = true;
        this.Destroyable = true;

        this.TraversableChar = TraversableType.NOTHROUGH;
        this.TraversableBullet = TraversableType.NOTHROUGH;

        this.GravityType = GravityType.RELATED_GRAVITY;
       
        this.Crushable = CrushType.CRUSHSTAY;
        this.Explored = false;
        this.Grounded = false;
        this.OnWalkEffects = new List<Effect>();
        this.OnDestroyEffects = new List<Effect>();
        this.HitablePoints = new List<HitablePoint>();
        this.OnStartTurn = new List<Effect>();
        this.OnEndTurn = new List<Effect>();
        this.AttachedEffects = new List<Effect>();
    }

    // Update is called once per frame
 
}
