using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : Placeable {
    new void Awake()
    {
        base.Awake();

        this.serializeNumber = 1;


        this.Walkable = true;

        this.Movable = false;
        this.Destroyable = false;

        this.TraversableChar = TraversableType.NOTHROUGH;
        this.TraversableBullet = TraversableType.NOTHROUGH;

        this.GravityType = GravityType.NULL_GRAVITY;

        this.Crushable = CrushType.CRUSHDESTROYBLOC;
        this.Explored = false;
        this.Grounded = false;
        this.OnWalkEffects = new List<Effect>();
        this.OnDestroyEffects = new List<Effect>();
        this.HitablePoints = new List<HitablePoint>();
        this.OnStartTurn = new List<Effect>();
        this.OnEndTurn = new List<Effect>();
        this.AttachedEffects = new List<Effect>();
    }
}
