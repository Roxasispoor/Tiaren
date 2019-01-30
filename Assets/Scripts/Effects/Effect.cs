using System;
using System.IO;
using UnityEngine;
/// <summary>
/// effects must be used by gameEffectManager which will resolve them
/// </summary>
/// 
[Serializable]
public abstract class Effect
{
    public int netIdLauncher = -1;
    [SerializeField]

    // for animation
    protected Animator animLauncher;

    private Placeable launcher;
    [SerializeField]
    private int turnActiveEffect = 1; //-1 = unactive 0=stop. we use int.MaxValue/2 when it's independent
    [SerializeField]
    private bool triggerAtOneOnly = false;
    [SerializeField]
    private bool hitOnDirectAttack = true;
    public virtual Placeable Launcher
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

    public bool HitOnDirectAttack
    {
        get
        {
            return hitOnDirectAttack;
        }

        set
        {
            hitOnDirectAttack = value;
        }
    }

    protected Effect()
    {
        
    }
    protected Effect(Effect other)
    {
        TurnActiveEffect = other.TurnActiveEffect;
        Launcher = other.Launcher;
        turnActiveEffect = other.TurnActiveEffect;
        TriggerAtOneOnly = other.TriggerAtOneOnly;
        HitOnDirectAttack = other.HitOnDirectAttack;
    }
    protected Effect(int numberOfTurns, bool triggerAtEnd = false, bool hitOnDirectAttack = true)
    {
        TurnActiveEffect = numberOfTurns;
        TriggerAtOneOnly = triggerAtEnd;
        HitOnDirectAttack = hitOnDirectAttack;
    }
    public abstract Effect Clone();
    public abstract void TargetAndInvokeEffectManager(LivingPlaceable placeable);
    public abstract void TargetAndInvokeEffectManager(Placeable placeable);
    public abstract void TargetAndInvokeEffectManager(ObjectOnBloc placeable);

    public abstract NetIdeable GetTarget();
    // Use this for initialization
    public abstract void Use();

    public string Save()
    {
        string text = JsonUtility.ToJson(this);
        text = GetType() + text + "\n";
        return text;
        // string path = "Skill1.json";
        //File.WriteAllText(path, text);
    }

    public static JaggedGrid FillGridFromJSON()
    {
        return JsonUtility.FromJson<JaggedGrid>(ReadString());
    }

    private static string ReadString()
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
        animLauncher = GameManager.instance.playingPlaceable.gameObject.GetComponent<Animator>();
    }

    public float GetTimeOfLauncherAnimation()
    {
        return animLauncher.GetCurrentAnimatorClipInfo(0)[0].clip.length;
    }

}
