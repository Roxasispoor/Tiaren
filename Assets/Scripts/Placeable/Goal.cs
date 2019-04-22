using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : StandardCube {
    new void Awake()
    {
        base.Awake();

        this.serializeNumber = 1;


        this.walkable = true;

        this.movable = false;
        this.Destroyable = false;

        this.traversableType = TraversableType.NOTHROUGH;
        this.traversableBullet = TraversableType.NOTHROUGH;

        this.gravityType = GravityType.NULL_GRAVITY;
        this.crushable = CrushType.CRUSHSTAY;
        this.explored = false;
        this.grounded = false;
        this.OnWalkEffects = new List<Effect>() { new FlagVictoryEffect(this)};
    }
}
