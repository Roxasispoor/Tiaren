using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hellmade.Sound;
namespace Animation
{
    public class AnimationHandler : MonoBehaviour
    {

        /// <summary>
        /// The main queue of animation, the first element is currently played and should be removed when it is finished.
        /// </summary>
        private Queue<AnimationBlock> animationSequence;

        /// <summary>
        /// Used when creating an animation via skills, when an effect want to add a component it should add to this animation.
        /// </summary>
        private AnimationBlock animationCurrentlyBuild;

        /// <summary>
        /// Singleton pattern instance.
        /// </summary>
        private static AnimationHandler m_Instance = null;

        public static AnimationHandler Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = (new GameObject("AnimationHandler")).AddComponent<AnimationHandler>();
                    DontDestroyOnLoad(m_Instance.gameObject);
                }
                return m_Instance;
            }
        }

        /// <summary>
        /// Used by the debbuger.
        /// </summary>
        public int AnimationblockInQueue { get { return animationSequence.Count; } }

        private void Awake()
        {
            animationSequence = new Queue<AnimationBlock>();
        }

        /// <summary>
        /// Call at the beginning of the use of a skill, to be completed by the component added by the effectss
        /// </summary>
        /// <param name="animationBlock"></param>
        public void SetCurrentBuildingAnimation(AnimationBlock animationBlock)
        {
            if (GameManager.instance.isServer)
                // TODO: Rather than just return, could use the termitateAnimation when they exist
                return;
            if (animationCurrentlyBuild != null)
            {
                Debug.LogError("Warning, trying to change the animation block currently build but the old one is still in the variable");
            }
            animationCurrentlyBuild = animationBlock;
        }

        //Amélioration possible, traiter de manières spécial certain component (par exemple regrouper la gravité)
        /// <summary>
        /// Used by effect to add a part of animation to an animationBlock.
        /// </summary>
        /// <param name="component"></param>
        public void AddComponentToCurrentAnimationBlock(AnimationComponent component)
        {
            if (GameManager.instance.isServer)
                // TODO: Rather than just return, could use the termitateAnimation when they exist
                return;
            animationCurrentlyBuild.AddComponent(component);
        }

        /// <summary>
        /// Call at the end of the creation of the animation (when it have passed through all the effect) and put it in the queue.
        /// </summary>
        public void FinishCurrentAnimationCreation()
        {
            if (GameManager.instance.isServer)
                // TODO: Rather than just return, could use the termitateAnimation when they exist
                return;
            QueueAnimation(animationCurrentlyBuild);
            animationCurrentlyBuild = null;
        }

        public Coroutine InstantLaunch(IEnumerator enumerator)
        {
            return StartCoroutine(enumerator);
        }
    
        /// <summary>
        /// Add a block to the sequence, launch it if there was any before.
        /// </summary>
        /// <param name="animation"></param>
        public void QueueAnimation(AnimationBlock animation)
        {
            if (GameManager.instance.isServer)
                // TODO: Rather than just return, could use the termitateAnimation when they exist
                return;
            if (animationSequence.Count == 0)
            {
                StartCoroutine(animation.Launch());
            }

            animationSequence.Enqueue(animation);
        }

        /// <summary>
        /// Called when an animation is finished.
        /// </summary>
        /// <param name="animation"></param>
        public void NotifyAnimationEnded(AnimationBlock animation)
        {
            //TODO: check if it is the first one (just to spot error, it should not exist)
            animationSequence.Dequeue();
            if (animationSequence.Count > 0)
            {
                StartCoroutine(animationSequence.Peek().Launch());
            }
        }
    }
        
}
