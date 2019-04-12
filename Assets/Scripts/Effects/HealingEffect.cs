using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingEffect : EffectOnNetIdeable
{
    private int healingValue;

    public HealingEffect(HealingEffect other)
    {
        this.healingValue = other.healingValue;
    }

    public HealingEffect(int healingValue)
    {
        this.healingValue = healingValue;
    }

    public override Effect Clone()
    {
        return new HealingEffect(this);
    }

    public override void Preview(NetIdeable target)
    {
        if (!target.IsLiving())
            return;
        Launcher = GameManager.instance.PlayingPlaceable;
        Target = (LivingPlaceable)target;
        GameManager.instance.PlayingPlaceable.Player.gameObject.GetComponent<UIManager>().Preview(-healingValue, 1, (LivingPlaceable)target); 
    }

    public override void ResetPreview(NetIdeable target)
    {
        if (!target.IsLiving())
            return;
        GameManager.instance.PlayingPlaceable.Player.gameObject.GetComponent<UIManager>().ResetPreview((LivingPlaceable)target);
    }

    public override void Use()
    {
        IHurtable hurtable = Target as IHurtable;
        if (hurtable == null)
        {
            Debug.LogError("Try to healing a non-Hurtable NetIdeable");
        }
        hurtable.ReceiveDamage(-healingValue);
    }
}
