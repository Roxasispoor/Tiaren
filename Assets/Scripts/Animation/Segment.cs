using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    class Segment : displacementElement
    {
        public Segment(Vector3 start, Vector3 end) : base(start, end)
        {
        }

        public override Vector3 computePosition(float index)
        {
            return (1 - index) * start + index * end;
        }
    }
}

