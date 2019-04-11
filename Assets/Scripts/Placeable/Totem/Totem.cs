using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Totem : StandardCube, IHurtable
{
    private int hp;
    [SerializeField]
    private int MaxHp;
    protected enum State { PURE, CORRUPTED}

    
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        hp = MaxHp;
    }

    public void ReceiveDamage(float damage)
    {
        if (damage > 0)
            hp--;
        else if (hp < 3)
            hp++;
        if(hp <= 0)
        {
            Destroy();
        }
    }

    /// <summary>
    /// Used to find the objects to affect
    /// </summary>
    protected abstract void FindTargets();
}
