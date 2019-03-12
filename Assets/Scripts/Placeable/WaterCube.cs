using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCube : StandardCube {
    protected override void Awake()
    {
        base.Awake();
        this.movable = false;
        this.walkable = false;

    }
    private void Start()
    {
    }
}
