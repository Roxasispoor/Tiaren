using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

public class ParameterChangeV2<T, TProperty> : EffectOnLiving {
    private static List<Expression<Func<T, TProperty>>> methodsForEffects;
    [SerializeField]
    private TProperty value; //the value you want to change ex: Tproperty = float T=Placeable
    private Expression<Func<T, TProperty>> expression;
    public int numberMethod;

    public static List <Expression<Func<T, TProperty>>> MethodsForEffects
    {
        get
        {
            if (methodsForEffects == null)
            {
                methodsForEffects = new List<Expression<Func<T, TProperty>>> ();
            }
            return methodsForEffects;
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        expression = MethodsForEffects[numberMethod];
    }

    public override Effect Clone()
    {
        return new ParameterChangeV2<T, TProperty>(this);
    }

    //Used like this: SetPropertyFromValue(value, o => o.Property1);
    public ParameterChangeV2(TProperty value, int numberMethod, int numberOfturns=1, bool triggerAtEnd = false,
        bool hitOnDirectAttack = true):base(numberOfturns,triggerAtEnd,hitOnDirectAttack)
    {
        this.value = value;
        this.numberMethod = numberMethod;
        Initialize();
    }
    public ParameterChangeV2(ParameterChangeV2<T,TProperty> other):base(other)
    {
        this.value = other.value;
        this.expression = other.expression;
    }

    public override void Use()
    {
        if(Target==null)
        {
            throw new Exception("target is null in parameterchange, impossible!");
        }
        MemberExpression member = (MemberExpression)expression.Body;
        MemberInfo property = member.Member;
        switch (property.MemberType)
        {
            case MemberTypes.Field:
                ((FieldInfo)property).SetValue(Target, value);
                break;
            case MemberTypes.Property:
                ((PropertyInfo)property).SetValue(Target, value, null);
                break;
            default:
                throw new ArgumentException("MemberInfo must be of type FieldInfo or PropertyInfo", "member");
        }
        
    }
}
