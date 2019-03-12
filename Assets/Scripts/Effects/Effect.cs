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

    /// <summary>
    ///  The creator of the effect.
    /// </summary>
    private Placeable launcher;

    /// <summary>
    /// The number of turn left for the effect.
    /// </summary>
    [SerializeField]
    public int turnActiveEffect = 1; //-1 = unactive 0=stop. we use int.MaxValue/2 when it's independent

    /// <summary>
    /// Should the effect trigger only once at the when its countdown is finished.
    /// </summary>
    [SerializeField]
    protected bool triggerOnce = false;

    /// <summary>
    /// Should the effect trigger when attached to the target.
    /// </summary>
    [SerializeField]
    protected bool triggerOnApply = true;

    /// <summary>
    ///  The creator of the effect.
    /// </summary>
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
    /*
    public virtual void AttachToTarget()
    {

    }*/

    /// <summary>
    /// Should the effect trigger only once at the when its countdown is finished.
    /// </summary>
    public bool TriggerOnce
    {
        get
        {
            return triggerOnce;
        }

        set
        {
            triggerOnce = value;
        }
    }

    /// <summary>
    /// Should the effect trigger when attached to the target.
    /// </summary>
    public bool TriggerOnApply
    {
        get
        {
            return triggerOnApply;
        }

        set
        {
            triggerOnApply = value;
        }
    }

    protected Effect()
    {
        
    }
    protected Effect(Effect other)
    {
        turnActiveEffect = other.turnActiveEffect;
        Launcher = other.Launcher;
        turnActiveEffect = other.turnActiveEffect;
        TriggerOnce = other.TriggerOnce;
        TriggerOnApply = other.TriggerOnApply;
    }
    protected Effect(int numberOfTurns, bool triggerAtEnd = false, bool triggerOnApply = true)
    {
        turnActiveEffect = numberOfTurns;
        TriggerOnce = triggerAtEnd;
        TriggerOnApply = triggerOnApply;
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
    public virtual void Initialize(LivingPlaceable livingPlaceable)
    {

    }

}
