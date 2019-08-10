    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

namespace Animation
{
    public class EmptyAnimation : AnimationBlock
    {
        public EmptyAnimation()
        {
        }
        public override IEnumerator Animate()
        {
            return null;
        }
    }
}