using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemHp : Totem, IEffectOnTurnStart
{
    private List<Effect> effects = new List<Effect>();

    DamageCalculated DamageEffect { get { return (DamageCalculated)effects[2]; } }

    protected override void Awake()
    {
        base.Awake();
        effects.Add(new DamageCalculated(30, DamageCalculated.DamageScale.MAG));
    }

    public void ApplyEffect(Placeable target)
    {
        throw new System.NotImplementedException();
    }

    protected override void FindTargets()
    {
        throw new System.NotImplementedException();
    }
}
