using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public abstract class Totem : StandardCube, IEffectOnTurnStart, IHurtable
{
    [SerializeField]
    protected int hp;
    [SerializeField]
    protected int MaxHp;
    protected enum State { PURE, CORRUPTED}
    protected State state = State.PURE;
    [SerializeField]
    private int range;
    [SerializeField]
    protected GameObject rangeDisplay;

    protected List<Effect> effects = new List<Effect>();
    [SerializeField]
    protected ParticleSystem particulesForState;

    public int Range { get => range; set => range = value; }

    public List<Skill> linkSkill;


    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        hp = MaxHp;
        linkSkill = new List<Skill>(); 
        InitializeLinkSkils();
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
        /*
        var ps = particulesForState.main;
        if (state == State.CORRUPTED)
        {
            ps.startColor = Color.black;
        }
        else
        {
            ps.startColor = Color.white;
        }
        */
    }

    protected void InitializeLinkSkils()
    {
        linkSkill.Add(HealingTotemLinkSkill.CreateNewInstanceFromReferenceAndSetTarget(this));
        linkSkill.Add(ExplosionLinkSkill.CreateNewInstanceFromReferenceAndSetTarget(this));
    }

    public override void HighlightForAttacks()
    {
        isTarget = true;
        Renderer rend = gameObject.GetComponentsInChildren<Renderer>()[0];
        rend.material.shader = GameManager.instance.PlayingPlaceable.outlineShader;
        rend.material.SetColor("_OutlineColor", Color.red);
    }

    public override void UnhighlightHovered()
    {
        base.UnhighlightHovered();
        if (isTarget)
        {
            Renderer rend = gameObject.GetComponentsInChildren<Renderer>()[0];
            rend.material.SetColor("_OutlineColor", Color.red);
        }
        else
        {
            gameObject.GetComponentsInChildren<Renderer>()[0].material.shader = GameManager.instance.PlayingPlaceable.originalShader;
        }
        rangeDisplay.SetActive(false);
    }

    public void UnHighlightTarget()
    {
        isTarget = false;
        UnhighlightHovered();
    }

    public override void Highlight()
    {
        base.Highlight();
        if(GameManager.instance.State == States.UseSkill)
        {
            Renderer rend = gameObject.GetComponentsInChildren<Renderer>()[0];
            rend.material.shader = GameManager.instance.PlayingPlaceable.outlineShader;
            rend.material.SetColor("_OutlineColor", Color.white);
        }
        else
        {
            rangeDisplay.SetActive(true);
        }
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
