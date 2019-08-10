    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

namespace Animation
{
    public class DamageComponent : AnimationComponent
    {
        Placeable target;
        int damage;

        public DamageComponent(Placeable target, int damage)
        {
            this.target = target;
            this.damage = damage;
        }

        public override IEnumerator Launch()
        {
            Animator animator = target.GetComponent<Animator>();

            if (animator != null)
            {
                animator.SetTrigger("Hurt");
            }

            Transform visualTranform = target.VisualTransform;
            FloatingTextController.CreateFloatingText(damage.ToString(), visualTranform, Color.red);
            return null;
        }
    }
}