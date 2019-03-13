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
        scaleOn = scaleOn;
        SinFactor = sinFactor;
    }

    public DamageCalculated(DamageCalculated other) : base(other)
    {
        this.Power = other.Power;
        scaleOn = other.scaleOn;
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


    public override void preview()
    {
        throw new System.NotImplementedException();
    }

    public float CalculateDamage()
    {
        float totalDmg = 0;
        if (scaleOn == DamageScale.STR)
        {
            totalDmg = power / Target.Def;
        }
        else if (scaleOn == DamageScale.DEXT)
        {
            totalDmg = power / Target.Def;
        }
        else if (scaleOn == DamageScale.MAG)
        {
            totalDmg = power / Target.Mdef;
        }
        if (Launcher.GetPosition().y > Target.GetPosition().y)
        {
            totalDmg *= ( 1 + SinFactor * (Launcher.GetPosition().y - Target.GetPosition().y) /
                  (Launcher.GetPosition() - Target.GetPosition()).magnitude);
        }
        Debug.Log(totalDmg);

        return (int) totalDmg;
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
