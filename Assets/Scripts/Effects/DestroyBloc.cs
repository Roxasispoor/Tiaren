using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// effect to destroy a bloc
/// </summary>
public class DestroyBloc : EffectOnPlaceableOnly
{
    public DestroyBloc(DestroyBloc other) : base(other)
    {
    }
    public DestroyBloc() : base()
    {
    }

    public override Effect Clone()
    {
        return new DestroyBloc(this);
    }

    // Use this for initialization
    override
    public void Use()
    {
        if (GameManager.instance.isClient)
        {
            GameManager.instance.RemoveBlockFromBatch(Target);
        }

        Animator animLauncher = GameManager.instance.playingPlaceable.gameObject.GetComponent<Animator>();
        animLauncher.Play("destroyBlock");
        AnimatorClipInfo[] m_CurrentClipInfo = animLauncher.GetCurrentAnimatorClipInfo(0);
        float timeAnim = m_CurrentClipInfo[0].clip.length;

        AnimationHandler.Instance.StartCoroutine(AnimationHandler.Instance.WaitAndDestroyBlock(Target, timeAnim));
            
    }
}
