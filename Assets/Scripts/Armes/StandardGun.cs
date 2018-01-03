using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Classe test pour gun classique
/// </summary>
public class StandardGun : Arme {

	// Use this for initialization
	void Start ()
    {
     this.Range=10;
    this.BaseDamage=35;
    this.StatMultiplier=0.01f;
   this.ScalesOnForce=false;
    this.PassiveEffects=new List<Effect>();
    this.OnShootEffects=new List<Effect>();

}
	
	// Update is called once per frame
	void Update () {
		
	}
}
