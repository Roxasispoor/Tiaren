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

}
