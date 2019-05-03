    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

namespace Animation
{
    public class PushAnimation : AnimationBlock
    {

        //MoveAlongBezier(List<Vector3> path, Placeable placeable, Animator animator = null, float speed = 4f, bool useCurve = false)
        List<Vector3> path;
        Placeable launcher;
        Animator animatorLauncher;
        Placeable target;
        Animator animatorTarget;
        const float speedBlocPerSecond = 2.5f;

        public PushAnimation(LivingPlaceable launcher, List<Vector3> path)
        {
            this.launcher = launcher;
            this.animatorLauncher = this.launcher.GetComponent<Animator>();
            
            this.path = path;
        }
        public override IEnumerator Animate()
        {
            int stepInPath = 0;

            //float overTime = 0;

            displacementElement currentElement;

            animatorLauncher.SetBool("walking", true);

            while (stepInPath < path.Count - 1  )
            {
                Vector3 lookAtPosition = new Vector3(path[stepInPath + 1].x, launcher.transform.position.y, path[stepInPath + 1].z);
                launcher.transform.LookAt(lookAtPosition);

                if (path[stepInPath].y == path[stepInPath + 1].y)
                {
                    currentElement = new Segment(path[stepInPath], path[stepInPath + 1]);
                } else
                {
                    currentElement = new JumpCurve(path[stepInPath], path[stepInPath + 1]);
                    animatorLauncher.SetTrigger("Jump");
                }


                float currentPercentage = 0;

                while (currentPercentage < 1)
                {

                    currentPercentage += Mathf.Min(speedBlocPerSecond * Time.deltaTime, 1);

                    if (currentPercentage > 0.9)
                    {
                        animatorLauncher.SetTrigger("Land");
                    }

                    launcher.transform.position = currentElement.computePosition(currentPercentage);
                    yield return null;

                }

                stepInPath++;
            }

            animatorLauncher.SetBool("walking", false);
        }
    }
}