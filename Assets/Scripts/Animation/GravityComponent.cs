    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

namespace Animation
{
    public class GravityComponent : AnimationComponent
    {
        Vector3 landingPosition;
        Placeable placeable;
        const float speedBlocPerSecond = 2.5f;
        bool useCurve = true;

        bool isJumping = false;

        public GravityComponent(Placeable placeable, Vector3 landingPosition)
        {
            this.placeable = placeable;
            this.landingPosition = landingPosition;
        }

        public override IEnumerator Launch()
        {
            Transform animatedTransform = placeable.VisualTransform;
            Animator animator = placeable.GetComponent<Animator>();

            if (animator != null)
            {
                animator.SetBool("Falling_gravity", true);
            }

            Segment laneToFollow = new Segment(animatedTransform.position, landingPosition);
            yield return laneToFollow.makeMoveAlong(animatedTransform, speedBlocPerSecond);

            if (animator != null)
            {
                animator.SetBool("Falling_gravity", false);
            }
        }
    }
}