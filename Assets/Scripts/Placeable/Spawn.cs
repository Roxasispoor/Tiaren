﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : StandardCube {
    new void Awake()
    {
        base.Awake();
        this.Walkable = true;

        this.Movable = false;
        this.Destroyable = false;

        this.TraversableChar = TraversableType.NOTHROUGH;
        this.TraversableBullet = TraversableType.NOTHROUGH;

        this.GravityType = GravityType.NULL_GRAVITY;

        this.crushable = CrushType.CRUSHDESTROYBLOC;
        this.explored = false;
        this.grounded = false;
        this.OnWalkEffects = new List<Effect>();
        this.AttachedEffects = new List<Effect>();
    }
 
}
