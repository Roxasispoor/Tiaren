using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemHp : Totem, IEffectOnTurnStart
{
    [SerializeField]
    private int power;

    HealingEffect HealingEffect { get { return (HealingEffect)effects[0]; } }
    DamageCalculated DamageEffect { get { return (DamageCalculated)effects[1]; } }

    protected override void Awake()
    {
        base.Awake();
        EffectManager.instance.Totems.Add(this);
        effects.Add(new HealingEffect(power));
        effects.Add(new DamageCalculated(power, DamageCalculated.DamageScale.BRUT));
    }

    public override void ApplyEffect(Placeable target)
    {
        if (target as LivingPlaceable)
        {
            if (state == State.PURE)
                target.DispatchEffect(HealingEffect);
            else
                target.DispatchEffect(DamageEffect);
        }
    }

    public  override bool CheckInRange(LivingPlaceable target)
    {
        Vector3 direction = target.GetPosition() - GetPosition();
        if (Math.Abs(direction.x) + Math.Abs(direction.y) + Math.Abs(direction.z) < range)
        {
            return true;
        }
        return false;
    }
}
