using UnityEngine;

public abstract class EffectOnObjectBloc : Effect
{
    public ObjectOnBloc target;
    public int netIdTarget = -1;

    public override NetIdeable GetTarget()
    {
        return Target;
    }
    public virtual ObjectOnBloc Target
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
    public override void TargetAndInvokeEffectManager(LivingPlaceable placeable)
    {
    }

    public override void TargetAndInvokeEffectManager(Placeable placeable)
    {
    }

    public override void TargetAndInvokeEffectManager(ObjectOnBloc placeable)//TODO, doesn't work
    {
        Debug.LogError("TargetAndInvokeEffectManager for EffectOnObjectBloc");
        Target = placeable;
        EffectManager.instance.DirectAttack(this);
    }

    public EffectOnObjectBloc(EffectOnObjectBloc other) : base(other)
    {
        Target = other.target;

    }
    public EffectOnObjectBloc(ObjectOnBloc target, int numberOfTurns=1) : base(numberOfTurns)
    {
        Target = target;
    }
    public EffectOnObjectBloc() : base()
    {
    }
}
