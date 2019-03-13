using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : StandardCube {
    new void Awake()
    {
        base.Awake();
        this.walkable = true;

        this.movable = false;
        this.Destroyable = false;

        this.traversableType = TraversableType.NOTHROUGH;
        this.traversableBullet = TraversableType.NOTHROUGH;

        this.gravityType = GravityType.NULL_GRAVITY;

        this.crushable = CrushType.CRUSHDESTROYBLOC;
        this.explored = false;
        this.grounded = false;
        this.OnWalkEffects = new List<Effect>();
        this.AttachedEffects = new List<Effect>();
    }
 
}
