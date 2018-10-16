using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// effects must be used by gameEffectManager which will resolve them
/// </summary>
public abstract class Effect
{
    private Placeable target;
    private Placeable launcher;
    private int turnActiveEffect; //-1 = unactive 0=stop. we use int.MaxValue/2 when it's independent
    protected Effect()
    {

    }
    protected Effect(Placeable target, Placeable launcher)
    {
        this.target = target;
        this.launcher = launcher;
    }
    public virtual Placeable Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
        }
    }

    public virtual Placeable Lanceur
    {
        get
        {
            return launcher;
        }

        set
        {
            launcher = value;
        }
    }

    public int TurnActiveEffect
    {
        get
        {
            return turnActiveEffect;
        }

        set
        {
            turnActiveEffect = value;
        }
    }

    // Use this for initialization
    public abstract void Use();

}
