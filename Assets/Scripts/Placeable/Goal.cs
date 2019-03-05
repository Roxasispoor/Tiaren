using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : StandardCube {
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
        this.crushable = CrushType.CRUSHDESTROYBLOC;
        this.Explored = false;
        this.Grounded = false;
        this.OnWalkEffects = new List<Effect>() { new FlagVictoryEffect()};
    }
}
