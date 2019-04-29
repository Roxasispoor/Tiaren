using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    abstract class displacementElement
    {
        protected Vector3 start;
        protected Vector3 end;
        protected float distance;

        public displacementElement (Vector3 start, Vector3 end)
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
    }
}

