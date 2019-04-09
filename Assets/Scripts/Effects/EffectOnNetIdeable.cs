
public abstract class EffectOnNetIdeable : Effect
{
    private NetIdeable target;
    public int netIdTarget= -1;

    /// <summary>
    /// Also sets netId of launcher
    /// </summary>
    public virtual NetIdeable Target
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
        return target;
    }

    protected EffectOnNetIdeable()
    {
    }
    protected EffectOnNetIdeable(int numberOfTurn, bool triggerAtEnd = false, ActivationType activationType = ActivationType.INSTANT) : base(numberOfTurn, triggerAtEnd, activationType)
    {
    }
    protected EffectOnNetIdeable(LivingPlaceable target, Placeable launcher) : base()
    {
        Target = target;
        Launcher = launcher;
    }
    protected EffectOnNetIdeable(LivingPlaceable target, Placeable launcher, int numberOfTurns) : base(numberOfTurns)
    {
        Target = target;
        Launcher = launcher;
    }

    public override void TargetAndInvokeEffectManager(LivingPlaceable placeable)
    {
        target = placeable;
        EffectManager.instance.DirectAttack(this);
    }

    public override void TargetAndInvokeEffectManager(Placeable placeable)
    {
        target = placeable;
        EffectManager.instance.DirectAttack(this);
    }

    public override void TargetAndInvokeEffectManager(ObjectOnBloc placeable)//TODO, doesn't work
    {
        target = placeable;
        EffectManager.instance.DirectAttack(this);
    }
}
