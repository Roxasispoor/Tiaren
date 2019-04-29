using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    public abstract class Animation
    {
        public virtual IEnumerator Animate()
        {
            return null;
        }

        public IEnumerator Launch()
        {
            yield return Animate();
            AnimationHandler.Instance.NotifyAnimationEnded(this);
        }
    }
}
