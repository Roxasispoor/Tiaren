using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculated : EffectOnNetIdeable {
  
    public enum DamageScale {STR,DEXT,MAG,BRUT}
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
        this.scaleOn = scaleOn;
        this.SinFactor = sinFactor;
    }

    public DamageCalculated(DamageCalculated other) : base()
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


    public override void Preview(NetIdeable target)
    {
        if (target as IHurtable == null)
            return;
        if (target.IsLiving())
        {
            Target = (LivingPlaceable)target;
            int damage = CalculateDamage();
            GameManager.instance.PlayingPlaceable.Player.gameObject.GetComponent<UIManager>().Preview(damage, 1, (LivingPlaceable)target);
        }
    }

    public override void ResetPreview(NetIdeable target)
    {
        if (!target.IsLiving())
            return;
        GameManager.instance.PlayingPlaceable.Player.gameObject.GetComponent<UIManager>().ResetPreview((LivingPlaceable)target);
    }

    /// <summary>
    /// Calculates the damage to deal before calling damage to deal it
    /// </summary>
    /// <returns></returns>
    public int CalculateDamage()
    {
        if (null == Launcher)
            Launcher = GameManager.instance.PlayingPlaceable;
        float totalDmg = 0;
        if (Target as Totem != null)
        {
            totalDmg = 1;
        }
        else
        {
            LivingPlaceable target = (LivingPlaceable)Target;
            if (scaleOn == DamageScale.STR || scaleOn == DamageScale.DEXT)
            {
                totalDmg = power * power / target.Def;
            }
            else if (scaleOn == DamageScale.MAG)
            {
                totalDmg = power * power / target.Mdef;
            }
            else if (scaleOn == DamageScale.BRUT)
            {
                totalDmg = power;
            }
            if (Launcher.GetPosition().y > Target.GetPosition().y)
            {
                totalDmg *= (1 + SinFactor * (Launcher.GetPosition().y - Target.GetPosition().y) /
                      (Launcher.GetPosition() - Target.GetPosition()).magnitude);
            }
        }

        return (int) totalDmg;
    }

    public int CalculateDamage(NetIdeable target)
    {
        NetIdeable save = Target;
        Target = target;
        int damage = CalculateDamage();
        Target = save;
        return damage;
    }

    override
        public void Use()
    {
        if(Target as IHurtable != null)
        {
            int totalDmg = CalculateDamage();
            EffectManager.instance.DirectAttack(new Damage(Target, Launcher, totalDmg));
        }
        else
        {
            Debug.LogError("Damage calculated on a non hurtable in forbidden");
        }
    }

}
