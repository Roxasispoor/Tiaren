using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemPa : Totem, IEffectOnTurnStart
{
    private int power;
    private int nbTurns;

    protected override void Awake()
    {
        base.Awake();
    }

    public void ApplyEffect(Placeable target)
    {
        if(target as LivingPlaceable)
        {
            if (state == State.PURE)
                ParameterChangeV2<LivingPlaceable, float>.CreateChangeAndReset((LivingPlaceable)target, power, 3, nbTurns);
            else
                ParameterChangeV2<LivingPlaceable, float>.CreateChangeAndReset((LivingPlaceable)target, -power, 3, nbTurns);
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
