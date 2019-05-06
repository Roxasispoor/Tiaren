using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hellmade.Sound;
namespace Animation
{
    public class AnimationHandler : MonoBehaviour
    {

        // ##################### NEW VARIABLE ##########################

        private Queue<AnimationBlock> animationSequence;

        private AnimationBlock animationCurrentlyBuild;

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

        public int Couroutine_count { get { return animationSequence.Count; } }

        // ##################### OLD VARIABLE ##########################
        // dictionary of enums
        Dictionary<string, string> AnimDictionary;
        public string SkillAnimationToPlay;
        public Animator animLauncher;
        public List<Animator> animTargets;
        public List<Placeable> placeableTargets;
        public List<Vector3> positionTargets;


        // ####################################### NEW STUFF ####################################################

        private void Awake()
        {
            /*
            AnimDictionary = new Dictionary<string, string>() {

                    {"Sword attack", "WaitAndBasicAttack" },
                    {"Destruction", "WaitAndDestroyBlock" },
                    {"Push", "WaitAndPushBlock" },
                    {"Create", "WaitAndSummonBlock" },
                    {"Fissure","WaitAndDestroyBlock"},
                    {"Wall","WaitAndSummonBlock" },
                    {"Bleeding","WaitAndBleed"},
                    {"Break a leg","WaitAndLowKick" },
                    {"Higher Ground","WaitAndSummonBlock"},
                    {"Piercing shot","WaitAndArrowAttack" },
                    {"Zipline","WaitAndBuff" },
                    {"Spinning attack","WaitAndSpin" },
                    {"Explosive fireball","WaitAndLaunchFireball" },
                    {"Staff Shot","WaitAndLaunchFireball" },
                };*/

            animationSequence = new Queue<AnimationBlock>();
        }

        public void QueueAnimation(AnimationBlock animation)
        {
            if (animationSequence.Count == 0)
            {
                Debug.Log("Start animation");
                StartCoroutine(animation.Launch());
            }

            animationSequence.Enqueue(animation);
        }

        public void NotifyAnimationEnded(AnimationBlock animation)
        {
            animationSequence.Dequeue();
            if (animationSequence.Count > 0)
            {
                Debug.Log("Start animation");
                StartCoroutine(animationSequence.Peek().Launch());
            }
            Debug.LogError("Queue in animation: " + animationSequence.Count);
        }

        //Amélioration possible, traiter de manières spécial certain component (par exemple regrouper la gravité)
        public void AddComponentToCurrentAnimationBlock(AnimationComponent component)
        {
            animationCurrentlyBuild.AddComponent(component);
        }

        public void SetCurrentBuildingAnimation(AnimationBlock animationBlock)
        {
            if (animationCurrentlyBuild != null)
            {
                Debug.LogError("Warning, trying to change the animation block currently build but the old one is still in the variable");
            }
            animationCurrentlyBuild = animationBlock;
        }

        public void FinishCurrentAnimationCreation()
        {
            QueueAnimation(animationCurrentlyBuild);
            animationCurrentlyBuild = null;
        }

        public Coroutine InstantLaunch(IEnumerator enumerator)
        {
            return StartCoroutine(enumerator);
        }

        // ####################################### OLD STUFF ####################################################

        public void PlayAnimation()
        {
            if (AnimDictionary.ContainsKey(SkillAnimationToPlay))
            {
                StartCoroutine(AnimDictionary[SkillAnimationToPlay]);
            }
        }

        public IEnumerator PlayAnimationCoroutine()
        {
            yield return StartCoroutine(AnimDictionary[SkillAnimationToPlay]);
        }


        // CHECKING INTERRUPTIONS

        // this coroutine allows to check if turn passes and thus finishes what has to be finished at this time
        public IEnumerator CheckInterruptions(float time)
        {
            Placeable tmpPlaceable = GameManager.instance.PlayingPlaceable;
            for (float i = 0; i < time; i = i + 0.1f)
            {
                if (tmpPlaceable == GameManager.instance.PlayingPlaceable) // TODO case if pa > 1, check selection of skill and break wait
                    yield return new WaitForSeconds(0.1f);
                else
                {
                    break;
                }
            }
        }

        public IEnumerator CheckInterruptionsWithRef(float time, LivingPlaceable refPlaceable)
        {
            for (float i = 0; i < time; i = i + 0.1f)
            {
                if (refPlaceable == GameManager.instance.PlayingPlaceable) // TODO case if pa > 1, check selection of skill and break wait
                    yield return new WaitForSeconds(0.1f);
                else
                {
                    break;
                }
            }
        }

        // checking interruptions if several actions in a row - give playing placeable as ref at the start
        // use if several coroutine in one action (ex : damage -> hurt -> die)

        public IEnumerator CheckContinuousInterruptions(float time, LivingPlaceable refPlaceable, bool interrupted)
        {
            if (!interrupted)
            {
                for (float i = 0; i < time; i = i + 0.1f)
                {
                    if (refPlaceable == GameManager.instance.PlayingPlaceable) // TODO case if pa > 1, check selection of skill and break wait
                        yield return new WaitForSeconds(0.1f);
                    else
                    {
                        interrupted = true;
                        break;
                    }
                }
            }
        }

        // ROUTINES FOR ANIMATION 

        public IEnumerator WaitAndDestroyBlock()
        {
            yield return null;
            SoundHandler.Instance.PlayDestroyBlockSound();
            animLauncher.Play("destroyBlock");
        }

        public IEnumerator WaitAndPushBlock()
        {
            yield return null;
            SoundHandler.Instance.PlayPushBlockSound();
            animLauncher.Play("pushBlock");
        }

        public IEnumerator WaitAndSummonBlock()
        {
            yield return null;
            animLauncher.Play("createBlock");
            SoundHandler.Instance.PlayCreateBlockSound();
        }

        // ATTACKS
        public IEnumerator WaitAndBasicAttack()
        {
            yield return null;
            SoundHandler.Instance.PlayAttackSound();
            animLauncher.Play("attack");
            yield return new WaitForSeconds(animLauncher.GetCurrentAnimatorStateInfo(0).length / 2);
            StartCoroutine("WaitAndGetHurt");
        }

        public IEnumerator WaitAndSpin()
        {
            yield return null;
            SoundHandler.Instance.PlaySpinSound();
            animLauncher.Play("tourbilol");
            yield return new WaitForSeconds(animLauncher.GetCurrentAnimatorStateInfo(0).length / 2);
            StartCoroutine("WaitAndGetHurt");
        }

        public IEnumerator WaitAndBleed()
        {
            yield return null;
            SoundHandler.Instance.PlaySwordSound();
            animLauncher.Play("makebleed");
            yield return new WaitForSeconds(animLauncher.GetCurrentAnimatorStateInfo(0).length / 2);
            StartCoroutine("WaitAndGetHurt");
        }

        public IEnumerator WaitAndLaunchFireball()
        {
            yield return null;
            SoundHandler.Instance.PlayFireballSound();
            GameObject tmp = Instantiate((GameObject)Resources.Load("FX/Fireball"), GameManager.instance.PlayingPlaceable.GetPosition(), Quaternion.identity);
            tmp.GetComponent<FireballFX>().Init(positionTargets[0] + new Vector3(0, 1, 0));
            //tmp.Init(placeableTargets[0].GetPosition());
        }

        public IEnumerator WaitAndBuff()
        {
            yield return null;
            animLauncher.Play("buff");
            SoundHandler.Instance.PlayHealingSound();
        }

        public IEnumerator WaitAndArrowAttack()
        {
            yield return null;
            SoundHandler.Instance.PlayBowSound();
            animLauncher.Play("shootArrow");
            yield return new WaitForSeconds(animLauncher.GetCurrentAnimatorStateInfo(0).length / 2);
            StartCoroutine("WaitAndGetHurt");
        }

        public IEnumerator WaitAndGetHurt()
        {
            yield return null;
            foreach (Animator animTarget in animTargets)
            {
                animTarget.Play("hurt");
                SoundHandler.Instance.PlayHurtSound();
            }
        }

        public IEnumerator WaitAndLowKick()
        {
            yield return null;
            animLauncher.Play("lowkick");
            yield return new WaitForSeconds(animLauncher.GetCurrentAnimatorStateInfo(0).length / 2);
            SoundHandler.Instance.PlayPunchSound();
        }
    }
}
