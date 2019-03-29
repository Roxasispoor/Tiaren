using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
/// <summary>
/// Utilitary class used to copy living to json and load from too
/// </summary>
[Serializable]
public class Stats
{
    public string className;
    public Vector3 positionSave;
    public string playerPosesser;
    public Attribute maxHP;
    public Attribute currentHP;
    public Attribute pmMax;
    public Attribute currentPM;
    public Attribute currentPA;
    public Attribute paMax;
    public Attribute speed;
    public Attribute speedStack;
    public Attribute jump;
    public Attribute def;
    public Attribute mdef;
    public Attribute deathLength;
    public SkillTypes[] ChosenSkills;
    //public List<Skill> skills;
    public bool isDead;
    //initialize a mano
    public int serializeNumber;
    public int netId;

    /// <summary>
    /// Fill living from values of this. Equalize any fields that have the same type and the same name
    /// </summary>
    /// <param name="living"></param>
    public void FillLiving(LivingPlaceable living)
    {
         living.serializeNumber = this.serializeNumber ;
         living.netId = this.netId ;
        //Debug.LogError(living.PlayerPosesser);
        List<FieldInfo>livingFields= new List<FieldInfo>( );
        foreach (FieldInfo fieldInfo in this.GetType().GetFields())
        {
            foreach(FieldInfo fieldLiving in living.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if(fieldInfo.GetType()==fieldLiving.GetType() && fieldInfo.Name==fieldLiving.Name)
                {
                    fieldLiving.SetValue(living, fieldInfo.GetValue(this));
                    break;
                }
            }

        }
      //  Debug.LogError("at the end" +this.playerPosesser);
    }
    /// <summary>
    /// Sets the values of this object from living
    /// </summary>
    /// <param name="living"></param>
    public void FillThis(LivingPlaceable living)
    {
        this.serializeNumber = living.serializeNumber;
         this.netId= living.netId ;
        List<FieldInfo> livingFields = new List<FieldInfo>();
        foreach (FieldInfo fieldInfo in this.GetType().GetFields())
        {
            foreach (FieldInfo fieldLiving in living.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (fieldInfo.GetType() == fieldLiving.GetType() && fieldInfo.Name == fieldLiving.Name)
                {
                    fieldInfo.SetValue(this, fieldLiving.GetValue(living));
                    break;
                }
            }

        }
    }
}
