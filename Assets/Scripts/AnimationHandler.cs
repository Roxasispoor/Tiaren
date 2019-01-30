﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hellmade.Sound;

public class AnimationHandler : MonoBehaviour
{
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

    // for sound 
    
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

    public IEnumerator WaitAndCreateBlock(GameObject go, Vector3Int position, float time)
    {
        int soundID = EazySoundManager.PlaySound((AudioClip)Resources.Load("Sounds/Blocksummon"));
        //EazySoundManager.GetAudio(SoundHandler.Instance.cubeCreateCharacID).Play();
        LivingPlaceable tmpPlaceable = GameManager.instance.PlayingPlaceable;
        Grid.instance.InstantiateCube(go, position);
        Placeable cubeConcerned = Grid.instance.GetPlaceableFromVector(position);
        //cubeConcerned.gameObject.SetActive(false);
        yield return StartCoroutine(CheckInterruptionsWithRef(time, tmpPlaceable));
        //cubeConcerned.gameObject.SetActive(true);

        // TODO : fix this to sync with anim
    }

    public IEnumerator WaitAndDestroyBlock(Placeable go, float time)
    {
        int soundID = EazySoundManager.PlaySound((AudioClip)Resources.Load("Sounds/Block destruction"));
        Vector3 pos = go.transform.position;
        yield return StartCoroutine(CheckInterruptions(time));
        
        if (GameManager.instance.isClient)
        {
            GameManager.instance.RemoveBlockFromBatch(go);
        }
        
        go.Destroy();
        
        Grid.instance.ConnexeFall((int)pos.x, (int)pos.y, (int)pos.z);
    }

    public IEnumerator WaitAndPushBlock(Placeable Target, List <Vector3>  path, float speed, float time)
    {
        GameManager.instance.PlayingPlaceable.gameObject.transform.LookAt(Target.transform);
        int soundID = EazySoundManager.PlaySound((AudioClip)Resources.Load("Sounds/Block move"));
        yield return StartCoroutine(CheckInterruptions(time/2));
        GameManager.instance.playingPlaceable.Player.StartMoveAlongBezier(path, Target, speed);
        // TODO : check if startmovealongbezier cannot cause bug (rebatch)
        // TODO : give Damage to living on the way

    }


    public IEnumerator WaitAndGetHurt(LivingPlaceable target, Animator animator, float time)
    {
        bool interrupted = false;
        bool finishHim = false;
        if (target.CurrentHP <= 0)
        {
            target.Destroy();
            finishHim = true;
        }

        LivingPlaceable tmpPlaceable = GameManager.instance.PlayingPlaceable;
        // le joueur qui joue
        yield return StartCoroutine(CheckContinuousInterruptions(time*0.75f, tmpPlaceable, interrupted));
        if (!interrupted)
        {
            animator.Play("hurt");
        }

        if (finishHim) {
            
            yield return StartCoroutine(CheckContinuousInterruptions(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length, tmpPlaceable, interrupted));
            if (!interrupted)
            {
                animator.Play("die");
                yield return StartCoroutine(CheckContinuousInterruptions(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length, tmpPlaceable, interrupted));
            }
            if (!(GameManager.instance.PlayingPlaceable == target && tmpPlaceable != target))
            {
                target.gameObject.SetActive(false);
            }
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "idle")
            {
                animator.Play("idle");
            }

        }
    }

   

}
