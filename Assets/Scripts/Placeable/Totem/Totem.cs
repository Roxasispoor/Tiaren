using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Totem : StandardCube, IEffectOnTurnStart, IHurtable
{
    protected int hp;
    [SerializeField]
    protected int MaxHp;
    protected enum State { PURE, CORRUPTED}
    protected State state = State.PURE;
    [SerializeField]
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
        else if (hp < MaxHp)
        {
            hp++;
            SwitchState();
        }
        if (hp <= 0)
        {
            Destroy();
        }
    }

    private void OnDestroy()
    {
        EffectManager.instance.Totems.Remove(this);
    }

    protected void SwitchState()
    {
        state = state == State.CORRUPTED ? State.PURE : State.CORRUPTED;
    }

    public override void HighlightForAttacks()
    {
        Renderer rend = gameObject.GetComponentsInChildren<Renderer>()[0];
        rend.material.shader = GameManager.instance.PlayingPlaceable.outlineShader;
        rend.material.SetColor("_OutlineColor", Color.red);
    }

    public override void UnHighlight()
    {
        base.UnHighlight();
        gameObject.GetComponentsInChildren<Renderer>()[0].material.shader = GameManager.instance.PlayingPlaceable.originalShader;
    }

    /// <summary>
    /// Used to check if the target should be affected
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public abstract bool CheckInRange(LivingPlaceable target);

    public virtual void ApplyEffect(Placeable target)
    {
    }
}
