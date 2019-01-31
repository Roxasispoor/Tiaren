using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCube : StandardCube {
    private void Awake()
    {
        base.Awake();
        this.movable = false;
        this.Walkable = false;

        OnWalkEffects.Add(new Damage(9999));

    }
    private void Start()
    {
    }
}
