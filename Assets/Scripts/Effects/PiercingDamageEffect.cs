using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingDamageEffect : DamageCalculated
{
    public float diminutionRate = 0.34f;

    public PiercingDamageEffect() : base()
    {
    }
    public PiercingDamageEffect(float value, DamageScale scaleOn, float diminutionRate=0.34f, float sinFactor = 0.3f) : base(value, scaleOn, sinFactor)
    {
        this.diminutionRate = diminutionRate;
    }

    public PiercingDamageEffect(PiercingDamageEffect other) : base(other)
    {

        this.diminutionRate = other.diminutionRate;
    }


    public override Effect Clone()
    {
        return new PiercingDamageEffect(this);
    }
    override
        public void Use()
    {

        if (Launcher.IsLiving())
        {
            float totalDmg = CalculateDamage();
            RaycastHit[] hits=Physics.RaycastAll(Launcher.GetPosition(), Target.GetPosition() - Launcher.GetPosition(), 
                (Target.GetPosition() - Launcher.GetPosition()).magnitude);
            int totalThrough=0;
            foreach (RaycastHit hit in hits)
            {
               if(hit.collider.gameObject.GetComponent<LivingPlaceable>()!=null)
                {
                    float effectivedmg;
                    if(hit.collider.gameObject.GetComponent<LivingPlaceable>()==Target)
                    {
                        effectivedmg = totalThrough * diminutionRate < 1 ? totalDmg * (1 - totalThrough * diminutionRate) : 0;
                        EffectManager.instance.DirectAttack(new Damage(Target, Launcher, effectivedmg));
                        break;
                    }
                    else
                    {
                        effectivedmg = totalThrough * diminutionRate < 1 ? totalDmg * (1 - totalThrough * diminutionRate) : 0;
                        EffectManager.instance.DirectAttack(new Damage(hit.collider.gameObject.GetComponent<LivingPlaceable>(), Launcher, effectivedmg));
                        totalThrough++;

                    }
                }
               else if((hit.collider.gameObject.GetComponent<Placeable>() != null))
                    {
                    totalThrough++;


                }
            }
        }
        else
        {
            Debug.LogError("Damage calculated from a not living placeable");
        }


    }
}