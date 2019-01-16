using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class Stats
{
   
    public float maxHP;
    public float currentHP;
    public int pmMax;
    public int currentPM;
    public float currentPA;
    public float paMax;
    public int force;
    public float speed;
    public int dexterity;
    public float speedStack;
    public int jump;
    /// <summary>
    /// Equalize any fields that have the same type and the same name
    /// </summary>
    /// <param name="living"></param>
    public void FillLiving(LivingPlaceable living)
    {
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
    }
    public void FillThis(LivingPlaceable living)
    {
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
