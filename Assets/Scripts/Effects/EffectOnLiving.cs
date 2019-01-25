using System;

[Serializable]
public abstract class EffectOnLiving : Effect
{
    private LivingPlaceable target;
    public int netIdTarget = -1;


    public EffectOnLiving(EffectOnLiving other) : base(other)
    {
        Target = other.target;

    }

    protected EffectOnLiving()
    {
    }
    protected EffectOnLiving(int numberOfTurn, bool triggerAtEnd = false, bool hitOnDirectAttack = true) : base(numberOfTurn, triggerAtEnd, hitOnDirectAttack)
    {
    }
    protected EffectOnLiving(LivingPlaceable target, Placeable launcher)
    {
        Target = target;
        Launcher = launcher;
    }
    protected EffectOnLiving(LivingPlaceable target, Placeable launcher, int numberOfTurns) : base(numberOfTurns)
    {
        Target = target;
        Launcher = launcher;
    }
    /// <summary>
    /// Also sets netId of launcher
    /// </summary>
    public virtual LivingPlaceable Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
            if (target != null)
            {
                netIdTarget = target.netId;
            }
        }
    }


    public override NetIdeable GetTarget()
    {
        return Target;
    }

    public override void TargetAndInvokeEffectManager(LivingPlaceable placeable)
    {
        Target = placeable;
        EffectManager.instance.DirectAttack(this);

    }

    public override void TargetAndInvokeEffectManager(Placeable placeable)
    {

    }
    public override void TargetAndInvokeEffectManager(ObjectOnBloc placeable)
    {

    }
}


