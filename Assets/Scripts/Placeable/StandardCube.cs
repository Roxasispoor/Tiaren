using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StandardCube : Placeable {

   public StandardCube() : base(new Vector3Int(), true, new List<Effect>(), true, true, TraversableType.NOTHROUGH, TraversableType.NOTHROUGH, GravityType.GRAVITE_SIMPLE, false, EcraseType.ECRASESTAY,
            new List<Effect>(), new List<HitablePoint>(), new List<Effect>(), new List<Effect>(), null)
    {
       
    }
    void Start()
    {

        this.Position = new Vector3Int();

        this.Walkable = true;
        this.OnWalkEffects = new List<Effect>();
        this.Movable = true;
        this.Destroyable = true;

        this.TraversableChar = TraversableType.NOTHROUGH;
        this.TraversableBullet = TraversableType.NOTHROUGH;

        this.GravityType = GravityType.GRAVITE_SIMPLE;
        this.Pickable = false;
        this.Ecrasable = EcraseType.ECRASESTAY;
        this.Explored = false;
        this.OnDestroyEffects = new List<Effect>();
        this.HitablePoints = new List<HitablePoint>();
        this.OnDebutTour = new List<Effect>();
        this.OnFinTour = new List<Effect>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
