using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingDamageEffect : DamageCalculated
{
    public float diminutionRate = 0.34f;
    private List<LivingPlaceable> targets = new List<LivingPlaceable>();
    private List<int> damages = new List<int>();

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

    public void PiercingDamage()
    {
        targets.Clear();
        damages.Clear();
        int effectivedmg;
        float totalDmg;
        RaycastHit[] hits = Physics.RaycastAll(Launcher.GetPosition(), Target.GetPosition() - Launcher.GetPosition(),
            (Target.GetPosition() - Launcher.GetPosition()).magnitude);
        int totalThrough = 0;
        foreach (RaycastHit hit in hits)
        {
            LivingPlaceable placeable = hit.collider.gameObject.GetComponent<LivingPlaceable>();
            if (placeable == null )
            {
                totalThrough++;
            }
            else if (placeable != Target)
            {
                totalDmg = CalculateDamage(placeable);
                effectivedmg = (int)(totalThrough * diminutionRate < 1 ? totalDmg * (1 - totalThrough * diminutionRate) : 0);
                targets.Add(hit.collider.gameObject.GetComponent<LivingPlaceable>());
                damages.Add(effectivedmg);
                totalThrough++;
            }
            else
            {
                totalDmg = CalculateDamage(placeable);
                effectivedmg = (int)(totalThrough * diminutionRate < 1 ? totalDmg * (1 - totalThrough * diminutionRate) : 0);
                targets.Add(hit.collider.gameObject.GetComponent<LivingPlaceable>());
                damages.Add(effectivedmg);
                break;
            }
            
        }
    }

    public override void Preview(Placeable target)
    {
        Target = (LivingPlaceable) target;
        Launcher = GameManager.instance.PlayingPlaceable;
        PiercingDamage();
        for (int i = 0; i < targets.Count; i++)
        {
            GameManager.instance.PlayingPlaceable.Player.gameObject.GetComponent<UIManager>().Preview(damages[i], targets[i]);
        }
    }

    public override void ResetPreview(Placeable target)
    {
        foreach(LivingPlaceable placeable in targets)
        {
            GameManager.instance.PlayingPlaceable.Player.gameObject.GetComponent<UIManager>().ResetPreview(placeable);
        }
    }

    override
        public void Use()
    {

        if (Launcher.IsLiving())
        {
            PiercingDamage();
            for (int i = 0; i < targets.Count; i++)
            {
                EffectManager.instance.DirectAttack(new Damage(targets[i], GameManager.instance.PlayingPlaceable, damages[i]));
            }
        }
        else
        {
            Debug.LogError("Damage calculated from a not living placeable");
        }


    }
}