using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBlock : EffectOnPlaceableOnly {

    public GameObject prefab;
    [SerializeField]
    public Vector3Int face;

    public CreateBlock()
    {
    }

    public CreateBlock(CreateBlock other) : base(other)
    {
        this.prefab = other.prefab;
        this.face = other.face;
    }

    public CreateBlock(GameObject prefab, Vector3Int face)
    {
        this.prefab = prefab;
        this.face = face;
    }

    public override Effect Clone()
    {
        return new CreateBlock(this);
    }

    public override void Use()
    {

        Animator animLauncher = GameManager.instance.playingPlaceable.gameObject.GetComponent<Animator>();
        animLauncher.Play("createBlock");
        AnimatorClipInfo[] m_CurrentClipInfo = animLauncher.GetCurrentAnimatorClipInfo(0);
        float timeAnim = m_CurrentClipInfo[0].clip.length;
        AnimationHandler.Instance.StartCoroutine(AnimationHandler.Instance.WaitAndCreateBlock(prefab, Target.GetPosition() + face,timeAnim));

    }
    
}
