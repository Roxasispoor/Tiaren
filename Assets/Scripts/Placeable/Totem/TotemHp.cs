using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemHp : Totem, IEffectOnTurnStart
{
    HealingEffect HealingEffect { get { return (HealingEffect)effects[0]; } }
    DamageCalculated DamageEffect { get { return (DamageCalculated)effects[1]; } }

    protected override void Awake()
    {
        base.Awake();
        range = 4;
        effects.Add(new HealingEffect(20));
        effects.Add(new DamageCalculated(20, DamageCalculated.DamageScale.BRUT));
    }

    public void ApplyEffect(Placeable target)
    {
        if (target as LivingPlaceable)
        {
            if (state == State.PURE)
                target.DispatchEffect(HealingEffect);
            else
                target.DispatchEffect(DamageEffect);
        }
    }

    protected override void CheckInRange(LivingPlaceable target)
    {
        Vector3 direction = target.GetPosition() - GetPosition();
        if (Physics.Raycast(GetPosition(), direction, range, LayerMask.GetMask("LivingPlaceable")))
        {
            ApplyEffect(target);
        }
    }
}
