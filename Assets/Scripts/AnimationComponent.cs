using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    public abstract class AnimationComponent
    {
        public abstract IEnumerator Launch();
    }
}
