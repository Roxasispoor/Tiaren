using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemPa : Totem, IEffectOnTurnStart
{

    protected override void Awake()
    {
        base.Awake();
        range = 4;
    }

    public void ApplyEffect(Placeable target)
    {
        if(target as LivingPlaceable)
        {
            if (state == State.PURE)
                ParameterChangeV2<LivingPlaceable, float>.CreateChangeAndReset((LivingPlaceable)target, 1, 3, 1);
            else
                ParameterChangeV2<LivingPlaceable, float>.CreateChangeAndReset((LivingPlaceable)target, -1, 3, 1);
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
