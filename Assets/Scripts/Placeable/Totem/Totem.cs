using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Totem : StandardCube, IHurtable
{
    protected int hp;
    [SerializeField]
    protected int MaxHp;
    protected enum State { PURE, CORRUPTED}
    protected State state = State.PURE;
    protected float range;

    protected List<Effect> effects = new List<Effect>();


    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        hp = MaxHp;
    }

    public void ReceiveDamage(float damage)
    {
        if (damage > 0)
        {
            hp--;
            SwitchState();
        }
        else if (hp < 3)
        {
            hp++;
            SwitchState();
        }
        if (hp <= 0)
        {
            Destroy();
        }
    }

    protected void SwitchState()
    {
        state = state == State.CORRUPTED ? State.PURE : State.CORRUPTED;
    }

    /// <summary>
    /// Used to find the objects to affect
    /// </summary>
    protected abstract void CheckInRange(LivingPlaceable target);
}
