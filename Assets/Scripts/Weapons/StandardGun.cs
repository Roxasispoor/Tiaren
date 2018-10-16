using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// test class for classic gun
/// </summary>
public class StandardGun : Weapon
{

    // Use this for initialization
    void Start()
    {
        this.Range = 10;
        this.BaseDamage = 35;
        this.StatMultiplier = 0.01f;
        this.ScalesOnForce = false;
        this.PassiveEffects = new List<Effect>();
        this.Skills = new List<Skill>();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
