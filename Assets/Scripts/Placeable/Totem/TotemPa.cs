using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemPa : Totem, IEffectOnTurnStart
{
    [SerializeField]
    private int power;
    [SerializeField]
    private int nbTurns;

    protected override void Awake()
    {
        base.Awake();
        EffectManager.instance.Totems.Add(this);
    }

    public override void ApplyEffect(Placeable target)
    {
        if(target as LivingPlaceable)
        {
            if (state == State.PURE)
                ParameterChangeV2<LivingPlaceable, float>.CreateChangeAndReset((LivingPlaceable)target, power, 2, nbTurns);
            else
                ParameterChangeV2<LivingPlaceable, float>.CreateChangeAndReset((LivingPlaceable)target, -power, 2, nbTurns);
        }
    }

    public override bool CheckInRange(LivingPlaceable target)
    {
        Vector3 direction = target.GetPosition() - GetPosition();
        if (Math.Abs(direction.x) + Math.Abs(direction.y) + Math.Abs(direction.z) <= range)
        {
            return true;
        }
        return false;
    }
}
