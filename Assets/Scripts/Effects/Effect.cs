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
    /// Should the effect trigger only once when its countdown is finished.
    /// </summary>
    [SerializeField]
    protected bool triggerOnce = false;

    /// <summary>
    /// When the effect should trigger.
    /// </summary>
    [SerializeField]
    public ActivationType activationType;

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

    /// <summary>
    /// Should the effect trigger only once when its countdown is finished.
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

    protected Effect()
    {
        
    }
    protected Effect(Effect other)
    {
        turnActiveEffect = other.turnActiveEffect;
        Launcher = other.Launcher;
        turnActiveEffect = other.turnActiveEffect;
        TriggerOnce = other.TriggerOnce;
        activationType = other.activationType;
    }
    protected Effect(int numberOfTurns, bool triggerOnce = false, ActivationType activationType = ActivationType.INSTANT)
    {
        this.turnActiveEffect = numberOfTurns;
        this.TriggerOnce = triggerOnce;
        this.activationType = activationType;
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
