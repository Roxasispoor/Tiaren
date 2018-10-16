using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// abstract class for weapons
/// </summary>
public abstract class Weapon : MonoBehaviour
{

    private float range;
    private float baseDamage;
    private float statMultiplier;
    private bool scalesOnForce;
    private List<Effect> passiveEffects;
    private List<Skill> skills;
   

    public float Range
    {
        get
        {
            return range;
        }

        set
        {
            range = value;
        }
    }

    public float BaseDamage
    {
        get
        {
            return baseDamage;
        }

        set
        {
            baseDamage = value;
        }
    }

    public float StatMultiplier
    {
        get
        {
            return statMultiplier;
        }

        set
        {
            statMultiplier = value;
        }
    }

    public List<Effect> PassiveEffects
    {
        get
        {
            return passiveEffects;
        }

        set
        {
            passiveEffects = value;
        }
    }

 

    public bool ScalesOnForce
    {
        get
        {
            return scalesOnForce;
        }

        set
        {
            scalesOnForce = value;
        }
    }

    public List<Skill> Skills
    {
        get
        {
            return skills;
        }

        set
        {
            skills = value;
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
