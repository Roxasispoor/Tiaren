using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    abstract class DisplacementElement
    {
        protected Vector3 start;
        protected Vector3 end;
        protected float distance;

        public DisplacementElement (Vector3 start, Vector3 end)
        {
            this.start = start;
            this.end = end;
            this.distance = computeDistance();
        }

        protected abstract float computeDistance();

        /// <summary>
        /// Compute the position relative to this element
        /// </summary>
        /// <param name="index">the percentage of the element, in [0,1]</param>
        /// <returns>The position relative to the index</returns>
        public abstract Vector3 computePosition(float index);

        public IEnumerator makeMoveAlong(Transform transform, float speedBlocPerSec)
        {
            float retlativeSpeed = speedBlocPerSec / distance;

            float currentPercentage = 0;

            while (currentPercentage < 1)
            {

                currentPercentage += Mathf.Min(speedBlocPerSec * Time.deltaTime, 1);

                transform.position = computePosition(currentPercentage);
                yield return null;

            }

            transform.position = end;
        }
    }
}

