using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAnimation : Animation
{
    //MoveAlongBezier(List<Vector3> path, Placeable placeable, Animator animator = null, float speed = 4f, bool useCurve = false)
    List<Vector3> path;
    Placeable placeable;
    Animator animator;
    const float speed = 2f;
    bool useCurve;

    public WalkAnimation (Placeable placeable, List<Vector3> path)
    {
        this.placeable = placeable;
        this.animator = placeable.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("WalkAnimation: animator not found");
        }
        this.path = path;
    }
    /*
    public void Launch()
    {
        StartCoroutine(Animate());
    }*/

    public override IEnumerator Animate()
    {
        Debug.LogError("Start coroutine");

        if (path.Count < 2)
        {
            yield break;
        }

        placeable.isMoving = true;

        float timeBezier = 0f;
        Vector3 previousCheckpoint = path[0];
        Vector3 controlPoint = new Vector3();

        bool isJumping = false;

        //For visual rotation
        Vector3 targetDir = path[1] - placeable.transform.position;
        targetDir.y = 0;

        bool useBezier;

        int stepInPath = 1;
        int stepRef = 1;
        float distance = CalculateDistance(previousCheckpoint, path[stepInPath], useCurve, out useBezier, ref controlPoint);
        float distanceParcourue = 0;
        SoundHandler.Instance.StartWalkSound();
        while (timeBezier < 1)
        {
            Debug.LogError(Time.renderedFrameCount + "- TimeBezier: " + timeBezier);
            distanceParcourue += (speed * Time.deltaTime);
            timeBezier = distanceParcourue / distance;

            while (timeBezier > 1 && stepInPath < path.Count - 1) // If we overstep the next checkpoint, compute to the following which is not overstep
            {
                Debug.LogError(this.GetHashCode() + "- Reset TimeBezier: " + timeBezier);

                distanceParcourue -= distance;
                previousCheckpoint = path[stepInPath];
                stepInPath++;

                distance = CalculateDistance(previousCheckpoint, path[stepInPath], useCurve, out useBezier, ref controlPoint);//On calcule la distance au noeud suivant
                timeBezier = distanceParcourue / distance; //on recalcule

            }

            if (stepInPath == path.Count - 1 && timeBezier > 1)
            {
                // arrived to the last node of path, in precedent loop
                placeable.transform.position = path[stepInPath];
                // exit yield loop
                break;
            }

            if (placeable.IsLiving())
            {
                targetDir = path[stepInPath] - placeable.transform.position;//next one

                targetDir.y = 0;// don't move up 
                Vector3 vectorRotation = Vector3.RotateTowards(placeable.transform.forward, targetDir, 0.2f, 0);
                placeable.transform.rotation = Quaternion.LookRotation(vectorRotation);
            }

            //change what we look at

            if (useBezier && useCurve)
            {
                if (animator != null)
                {
                    if (!isJumping)
                    {
                        isJumping = true;
                        stepRef = stepInPath;
                        animator.Play("jump");
                        SoundHandler.Instance.PauseWalkSound();

                    }
                    else
                    {
                        if (stepInPath != stepRef)
                        {
                            stepRef = stepInPath;
                            animator.Play("land 0");
                            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

                        }
                    }
                }

                placeable.transform.position = Mathf.Pow(1 - timeBezier, 2) * (previousCheckpoint) + 2 * (1 - timeBezier) * timeBezier * (controlPoint) + Mathf.Pow(timeBezier, 2) * (path[stepInPath]);

            }
            else
            {
                if (animator != null)
                {
                    if (isJumping)
                    {
                        isJumping = false;
                        animator.Play("land 1");
                        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
                        SoundHandler.Instance.StartWalkSound();
                    }
                    else
                    {
                        animator.Play("walking");

                    }
                }

                Vector3 next = Vector3.Lerp(previousCheckpoint, path[stepInPath], timeBezier);


                placeable.transform.position = next;

            }




            yield return null;

        }
        if (animator != null)
        {
            if (isJumping)
            {
                animator.Play("land 2");
            }
            else
            {
                animator.SetTrigger("idle");
            }

        }
        SoundHandler.Instance.StopWalkSound();
        placeable.isMoving = false;

        Debug.Log("End" + placeable.GetPosition());
    }

    /// <summary>
    /// See https://www.gamedev.net/forums/topic/551455-length-of-a-generalized-quadratic-bezier-curve-in-3d/
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    public static float LengthQuadraticBezier(Vector3 start, Vector3 end, Vector3 control)
    {
        Vector3[] C = { start, control, end };
        // ASSERT:  C[0], C[1], and C[2] are distinct points.

        // The position is the following vector-valued function for 0 <= t <= 1.
        //   P(t) = (1-t)^2*C[0] + 2*(1-t)*t*C[1] + t^2*C[2].
        // The derivative is
        //   P'(t) = -2*(1-t)*C[0] + 2*(1-2*t)*C[1] + 2*t*C[2]
        //         = 2*(C[0] - 2*C[1] + C[2])*t + 2*(C[1] - C[0])
        //         = 2*A[1]*t + 2*A[0]
        // The squared length of the derivative is
        //   f(t) = 4*Dot(A[1],A[1])*t^2 + 8*Dot(A[0],A[1])*t + 4*Dot(A[0],A[0])
        // The length of the curve is
        //   integral[0,1] sqrt(f(t)) dt

        Vector3[] A ={ C[1] - C[0],  // A[0] not zero by assumption
            C[0] - 2.0f*C[1] + C[2]
            };

        float length;

        if (A[1] != Vector3.zero)
        {
            // Coefficients of f(t) = c*t^2 + b*t + a.
            float c = 4.0f * Vector3.Dot(A[1], A[1]);  // c > 0 to be in this block of code
            float b = 8.0f * Vector3.Dot(A[0], A[1]);
            float a = 4.0f * Vector3.Dot(A[0], A[0]);  // a > 0 by assumption
            float q = 4.0f * a * c - b * b;  // = 16*|Cross(A0,A1)| >= 0

            // Antiderivative of sqrt(c*t^2 + b*t + a) is
            // F(t) = (2*c*t + b)*sqrt(c*t^2 + b*t + a)/(4*c)
            //   + (q/(8*c^{3/2}))*log(2*sqrt(c*(c*t^2 + b*t + a)) + 2*c*t + b)
            // Integral is F(1) - F(0).

            float twoCpB = 2.0f * c + b;
            float sumCBA = c + b + a;
            float mult0 = 0.25f / c;
            float mult1 = q / (8.0f * Mathf.Pow(c, 1.5f));
            length =
                mult0 * (twoCpB * Mathf.Sqrt(sumCBA) - b * Mathf.Sqrt(a)) +
                mult1 * (Mathf.Log(2.0f * Mathf.Sqrt(c * sumCBA) + twoCpB) - Mathf.Log(2.0f * Mathf.Sqrt(c * a) + b));
        }
        else
        {
            length = 2.0f * A[0].magnitude;
        }

        return length;
    }
    /// <summary>
    /// Unused Hacky way to approximate bezier length
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    public float LengthBezierApprox(Vector3 start, Vector3 end, Vector3 control)
    {
        float chord = Vector3.Distance(end, start);
        float cont = Vector3.Distance(start, control) + Vector3.Distance(control, end);
        return (cont + chord) / 2;
    }
    /// <summary>
    /// Calculate distance of bezier, update isBezier and the needed controlPoint
    /// </summary>
    /// <param name="start"></param>
    /// <param name="nextNode"></param>
    /// <param name="isBezier"></param>
    /// <returns></returns>
    public static float CalculateDistance(Vector3 start, Vector3 nextNode, bool useCurve, out bool isBezier, ref Vector3 controlPoint)
    {
        isBezier = start.y != nextNode.y;

        if (isBezier && useCurve)// if difference of height
        {
            controlPoint = (nextNode + start + 2 * new Vector3(0, Mathf.Abs(nextNode.y - start.y), 0)) / 2;

            return LengthQuadraticBezier(start, nextNode, controlPoint);
        }
        else
        {
            return Vector3.Distance(start, nextNode);
        }

    }
}
