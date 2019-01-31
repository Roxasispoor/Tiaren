﻿using UnityEngine;

/// <summary>
/// Class representing damage
/// </summary>
public class Damage : EffectOnLiving
{
    [SerializeField]
    private float damageValue;

    public Damage()
    {
        damageValue = 0;
    }
    public Damage(float value)
    {
        damageValue = value;
    }
    public Damage(float value, int numberOfturn, bool triggerAtEnd = false, bool hitOnDirectAttack = true) : base(numberOfturn, triggerAtEnd, hitOnDirectAttack)
    {
        damageValue = value;
    }
    public Damage(Damage other) : base(other)
    {
        this.damageValue = other.damageValue;
    }


    /// <summary>
    /// create a damage object
    /// </summary>
    /// <param name="target">target of the action</param>
    /// <param name="launcher">launcher of the action</param>
    /// <param name="damageValue">quanitity of damage given</param>
    public Damage(LivingPlaceable target, Placeable launcher, float damageValue) : base(target, launcher)
    {
        this.DamageValue = damageValue;
    }
    public Damage(LivingPlaceable target, Placeable launcher, float damageValue, int numberOfTurns) : base(target, launcher, numberOfTurns)
    {
        this.DamageValue = damageValue;
    }

    public float DamageValue
    {
        get
        {
            return damageValue;
        }

        set
        {
            damageValue = value;
        }
    }

    public override Effect Clone()
    {
        return new Damage(this);
    }

    override
        public void Use()
    {
        Animator animLauncher = GameManager.instance.playingPlaceable.gameObject.GetComponent<Animator>();
        Animator animTarget = Target.gameObject.GetComponent<Animator>();
        Target.CurrentHP -= DamageValue;
        FloatingTextController.CreateFloatingText(DamageValue.ToString(), Target.transform);

        if (GameManager.instance.playingPlaceable == Target)
            // suicide
        {
            AnimationHandler.Instance.StartCoroutine(AnimationHandler.Instance.WaitAndGetHurt(Target, animTarget,0f));
        }

        else
        {
            animLauncher.Play("attack");
            SoundHandler.Instance.PlayAttackSound();
            AnimationHandler.Instance.StartCoroutine(AnimationHandler.Instance.WaitAndGetHurt(Target, animTarget, GetTimeOfLauncherAnimation()));

        }

    }
}
