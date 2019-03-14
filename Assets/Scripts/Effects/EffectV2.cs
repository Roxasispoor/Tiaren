using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
/// <summary>
/// effects must be used by gameEffectManager which will resolve them
/// </summary>
/// 
[Serializable]
public abstract class EffectV2<L,T> where T : NetIdeable where L : NetIdeable
{
    public int netIdLauncher = -1;
    [SerializeField]

    // for animation
    protected Animator animLauncher;

    private L launcher;
    private T target;
    private int turnActiveEffect = 1; //-1 = unactive 0=stop. we use int.MaxValue/2 when it's independent
    private bool triggerAtOneOnly = false;
    public L Launcher
    {
        get
        {
            return launcher;
        }

        set
        {
            launcher = value;
            if (launcher != null)
            {
                netIdLauncher = launcher.netId;
            }
        }
    }
    public virtual void AttachToTarget()
    {
        
    }

    public int TurnActiveEffect
    {
        get
        {
            return turnActiveEffect;
        }

        set
        {
            turnActiveEffect = value;
        }
    }

    public T Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
        }
    }

    public bool TriggerAtOneOnly
    {
        get
        {
            return triggerAtOneOnly;
        }

        set
        {
            triggerAtOneOnly = value;
        }
    }

    protected EffectV2()
    {
       
    }
    protected EffectV2(EffectV2<L,T> other)
    {
        TurnActiveEffect = other.TurnActiveEffect;
        Launcher = other.Launcher;
        turnActiveEffect = other.turnActiveEffect;
        triggerAtOneOnly = other.triggerAtOneOnly;

    }
    protected EffectV2(int numberOfTurns)
    {
        TurnActiveEffect = numberOfTurns;
    }
    public abstract Effect Clone();
    public virtual void TargetAndInvokeEffectManager(T target)
    {
        Target = target;
    }
    public void TargetAndInvokeEffectManager(NetIdeable placeable)
    {

    }
    // Use this for initialization
    public abstract void Use();

    public virtual string Save()
    {
        string text = JsonUtility.ToJson(this);
        text = GetType() + text;
        return text;
        // string path = "Skill1.json";
        //File.WriteAllText(path, text);
    }

    public static JaggedGrid FillGridFromJSON()
    {
        return JsonUtility.FromJson<JaggedGrid>(ReadString());
    }


    static string ReadString()
    {
        string path = "Skill1.json";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        string toReturn = reader.ReadToEnd();
        reader.Close();
        return toReturn;
    }
    public virtual void Initialize()
    {

    }

    public void GetLauncherAnimation()
    {
        animLauncher = GameManager.instance.PlayingPlaceable.gameObject.GetComponent<Animator>();
    }

    public float GetTimeOfLauncherAnimation()
    {
        return animLauncher.GetCurrentAnimatorClipInfo(0)[0].clip.length;
    }

}
