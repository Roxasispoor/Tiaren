using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    class JumpCurve : displacementElement
    {
        Vector3 controlPoint;

        public JumpCurve(Vector3 start, Vector3 end) : base(start, end)
        {
            controlPoint = (end + start + 2 * new Vector3(0, Mathf.Abs(end.y - start.y), 0)) / 2;
        }

        public override Vector3 computePosition(float index)
        {
            return Mathf.Pow(1 - index, 2) * (start) + 2 * (1 - index) * index * (controlPoint) + Mathf.Pow(index, 2) * (end); ;
        }
    }
}

