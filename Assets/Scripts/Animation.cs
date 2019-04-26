using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Animation
{
    public virtual IEnumerator Animate()
    {
        return null;
    }

    public IEnumerator Launch()
    {
        Debug.LogError("Launch animation!  + this.GetHashCode()");
        yield return Animate();
        Debug.LogError("End of animation.");
        AnimationHandler.Instance.NotifyAnimationEnded(this);
    }
}
