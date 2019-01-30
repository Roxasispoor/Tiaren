using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculated : EffectOnLiving {
  
    public enum DamageScale {STR,DEXT,MAG}
    [SerializeField]
    private float power;
    [SerializeField]
    private DamageScale scaleOn;
    [SerializeField]
    private float sinFactor = 0.3f;

    public DamageCalculated()
    {
        Power = 0;
    }
    public DamageCalculated(float value,DamageScale scaleOn,float sinFactor=0.3f)
    {
        Power = value;
        ScaleOn = scaleOn;
        SinFactor = sinFactor;
    }

    public DamageCalculated(DamageCalculated other) : base(other)
    {
        this.Power = other.Power;
        ScaleOn = other.ScaleOn;
        SinFactor = other.sinFactor;
    }


    public float Power
    {
        get
        {
            return power;
        }

        set
        {
            power = value;
        }
    }

    private DamageScale ScaleOn
    {
        get
        {
            return scaleOn;
        }

        set
        {
            scaleOn = value;
        }
    }

    public float SinFactor
    {
        get
        {
            return sinFactor;
        }

        set
        {
            sinFactor = value;
        }
    }

    public override Effect Clone()
    {
        return new DamageCalculated(this);
    }

    public float CalculateDamage()
    {
        float totalDmg = 0;
        if (ScaleOn == DamageScale.STR)
        {
            totalDmg = power * ((LivingPlaceable)Launcher).Force / Target.Def;
        }
        else if (ScaleOn == DamageScale.DEXT)
        {
            totalDmg = power * ((LivingPlaceable)Launcher).Dexterity / Target.Def;
        }
        else if (ScaleOn == DamageScale.MAG)
        {
            totalDmg = power * ((LivingPlaceable)Launcher).Mstr / Target.Mdef;
        }
        if (Launcher.GetPosition().y > Target.GetPosition().y)
        {
            totalDmg *= SinFactor * (Launcher.GetPosition().y - Target.GetPosition().y) /
                  (Launcher.GetPosition() - Target.GetPosition()).magnitude;
        }
        Debug.Log(totalDmg);

        return totalDmg;
    }

    override
        public void Use()
    {
        
        if(Launcher.IsLiving())
        {
            float totalDmg = CalculateDamage();
            EffectManager.instance.DirectAttack(new Damage(Target, Launcher, totalDmg));
        }
        else
        {
            Debug.LogError("Damage calculated from a not living placeable");
        }


    }

}
