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
public abstract class Effect
{
    public int netIdLauncher=-1;
    [SerializeField]
    private Placeable launcher;
    private int turnActiveEffect; //-1 = unactive 0=stop. we use int.MaxValue/2 when it's independent
    public virtual Placeable Launcher
    {
        get
        {
            return launcher;
        }

        set
        {
            launcher = value;
            if(launcher!=null)
            { 
            netIdLauncher = launcher.netId;
            }
        }
    }
    protected Effect()
    {

    }
   
    public abstract Effect Clone();
    public abstract void TargetAndInvokeEffectManager(LivingPlaceable placeable);
    public abstract void TargetAndInvokeEffectManager(Placeable placeable);

    public abstract Placeable GetTarget();
    // Use this for initialization
    public abstract void Use();

    public string Save()
    {
        string text = JsonUtility.ToJson(this);
        text = GetType() + text ;
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

}
