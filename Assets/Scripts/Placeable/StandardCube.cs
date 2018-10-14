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

    void Awake()
    {
        this.serializeNumber = 1;
       

        this.Walkable = true;
        this.OnWalkEffects = new List<Effect>();
        this.Movable = true;
        this.Destroyable = true;

        this.TraversableChar = TraversableType.NOTHROUGH;
        this.TraversableBullet = TraversableType.NOTHROUGH;

        this.GravityType = GravityType.GRAVITE_SIMPLE;
       
        this.Ecrasable = EcraseType.ECRASESTAY;
        this.Explored = false;
        this.OnDestroyEffects = new List<Effect>();
        this.HitablePoints = new List<HitablePoint>();
        this.OnDebutTour = new List<Effect>();
        this.OnFinTour = new List<Effect>();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
