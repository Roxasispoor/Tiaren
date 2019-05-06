    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

namespace Animation
{
    public class WalkAnimation : AnimationBlock
    {
        //MoveAlongBezier(List<Vector3> path, Placeable placeable, Animator animator = null, float speed = 4f, bool useCurve = false)
        List<Vector3> path;
        Placeable placeable;
        Animator animator;
        const float speedBlocPerSecond = 2.5f;
        bool useCurve = true;

        bool isJumping = false;

        public WalkAnimation(Placeable placeable, List<Vector3> path)
        {
            this.placeable = placeable;
            this.animator = placeable.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("WalkAnimation: animator not found");
            }
            this.path = path;
        }
        public override IEnumerator Animate()
        {
            int stepInPath = 0;

            //float overTime = 0;

            DisplacementElement currentElement;

            animator.SetBool("walking", true);

            while (stepInPath < path.Count - 1  )
            {
                Vector3 lookAtPosition = new Vector3(path[stepInPath + 1].x, placeable.transform.position.y, path[stepInPath + 1].z);
                placeable.transform.LookAt(lookAtPosition);

                if (path[stepInPath].y == path[stepInPath + 1].y)
                {
                    currentElement = new Segment(path[stepInPath], path[stepInPath + 1]);
                } else
                {
                    currentElement = new JumpCurve(path[stepInPath], path[stepInPath + 1]);
                    animator.SetTrigger("Jump");
                }


                float currentPercentage = 0;

                while (currentPercentage < 1)
                {

                    currentPercentage += Mathf.Min(speedBlocPerSecond * Time.deltaTime, 1);

                    if (currentPercentage > 0.9)
                    {
                        animator.SetTrigger("Land");
                    }

                    placeable.transform.position = currentElement.computePosition(currentPercentage);
                    yield return null;

                }

                stepInPath++;
            }

            placeable.transform.position = path[path.Count - 1];

            animator.SetBool("walking", false);
        }
    }
}