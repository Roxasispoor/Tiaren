﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hellmade.Sound;

public class AnimationHandler : MonoBehaviour
{
    private static AnimationHandler m_Instance = null;
    // dictionary of enums
    Dictionary<string, string> AnimDictionary;
    public string SkillAnimationToPlay;
    public Animator animLauncher;
    public List<Animator> animTargets;
    public List<Placeable> placeableTargets;
    public List<Vector3> positionTargets;
    public static AnimationHandler Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = (new GameObject("AnimationHandler")).AddComponent<AnimationHandler>();
                DontDestroyOnLoad(m_Instance.gameObject);
                m_Instance.AnimDictionary = new Dictionary<string, string>() {

                    {"Basic_attack", "WaitAndBasicAttack" },
                    {"Basic_destruction", "WaitAndDestroyBlock" },
                    {"Basic_push", "WaitAndPushBlock" },
                    {"Basic_creation", "WaitAndSummonBlock" },
                    {"Fissure","WaitAndDestroyBlock"},
                    {"Wall","WaitAndSummonBlock" },
                    {"Bleeding","WaitAndBleed"},
                    {"debuffPm","WaitAndLowKick" },
                    {"HigherGround","WaitAndSummonBlock"},
                    {"Piercing_arrow","WaitAndArrowAttack" },
                    {"Zipline","WaitAndBuff" },
                    {"Spinning","WaitAndSpin" },
                    {"ExplosiveFireball","WaitAndLaunchFireball" },

                };
            }
            return m_Instance;
        }
    }

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
        yield return new WaitForSeconds(animLauncher.GetCurrentAnimatorStateInfo(0).length/2);
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
        GameObject tmp = Instantiate((GameObject)Resources.Load("FX/Fireball"),GameManager.instance.playingPlaceable.GetPosition(),Quaternion.identity);
        tmp.GetComponent<Fireball>().Init(positionTargets[0]+new Vector3(0,1,0));
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