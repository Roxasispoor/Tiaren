using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    public abstract class AnimationBlock
    {
        protected Queue<AnimationComponent> components = new Queue<AnimationComponent>();

        public virtual IEnumerator Animate()
        {
            return null;
        }

        public IEnumerator Launch()
        {
            OnStart();
            yield return Animate();
            while (components.Count > 0)
            {
                AnimationComponent current = components.Dequeue();
                yield return current.Launch();
            }
            OnEnd();
            AnimationHandler.Instance.NotifyAnimationEnded(this);
        }

        protected virtual void OnStart()
        {

        }

        protected virtual void OnEnd()
        {

        }

        public void AddComponent(AnimationComponent component)
        {
            components.Enqueue(component);
        }
    }
}
