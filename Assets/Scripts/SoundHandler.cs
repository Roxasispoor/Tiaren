using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hellmade.Sound;

public class SoundHandler : MonoBehaviour {

    private static SoundHandler m_Instance = null;
    public static SoundHandler Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = (new GameObject("SoundHandler")).AddComponent<SoundHandler>();
                DontDestroyOnLoad(m_Instance.gameObject);
            }
            return m_Instance;
        }
    }


    
    public int fightSceneMusicID;
    public int cubeCreateCharacID;

    public void PrepareAllSounds()
    {
        int fightSceneMusicID = EazySoundManager.PrepareMusic((AudioClip)Resources.Load("Sounds/backgroundmusic"), 1f, true, false, 0f, 0f);
        //int cubeCreateCharacID = EazySoundManager.PrepareSound((AudioClip)Resources.Load("Sounds/Blocksummon"), false);
    }
    public void StartFightMusic()
    {
        EazySoundManager.GetAudio(fightSceneMusicID).Play();
    }
    
    public void StartWalkSound()
    {
        EazySoundManager.PlaySound((AudioClip)Resources.Load("Sounds/Footsteps"), true);
    }

    public void PauseWalkSound()
    {
        EazySoundManager.PauseAllSounds();
    }

    public void StopWalkSound()
    {
        EazySoundManager.StopAllSounds();
    }

    public void PlayUISound()
    {
        EazySoundManager.PlaySound((AudioClip)Resources.Load("Sounds/Click"));
    }

    public void PlayCreateBlockSound()
    {
        EazySoundManager.PlaySound((AudioClip)Resources.Load("Sounds/Blocksummon"));
    }

    public void PlayDestroyBlockSound()
    {
        EazySoundManager.PlaySound((AudioClip)Resources.Load("Sounds/Block destruction"));
    }

    public void PlayAttackSound()
    {
        EazySoundManager.PlaySound((AudioClip)Resources.Load("Sounds/Sword"));
    }

    public void PlayPushBlockSound()
    {
        EazySoundManager.PlaySound((AudioClip)Resources.Load("Sounds/Block move"));
    }

    public void PlaySpinSound()
    {
        EazySoundManager.PlaySound((AudioClip)Resources.Load("Sounds/Spin attack"));
    }

    public void PlayBowSound()
    {
        EazySoundManager.PlaySound((AudioClip)Resources.Load("Sounds/Bow shot"));
    }

    public void PlayHurtSound()
    {
        EazySoundManager.PlaySound((AudioClip)Resources.Load("Sounds/Damage sound"),0.4f);
    }

    public void PlayFireballSound()
    {
        EazySoundManager.PlaySound((AudioClip)Resources.Load("Sounds/Fireball"));
    }

    public void PlayPunchSound()
    {
        EazySoundManager.PlaySound((AudioClip)Resources.Load("Sounds/punch"));
    }

    public void PlayHealingSound()
    {
        EazySoundManager.PlaySound((AudioClip)Resources.Load("Sounds/Healing sound"),0.5f);
    }

    public void PlaySwordSound()
    {
        EazySoundManager.PlaySound((AudioClip)Resources.Load("Sounds/OneSword"));
    }
}
