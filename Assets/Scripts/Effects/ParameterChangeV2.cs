using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

public class ParameterChangeV2<T, TProperty> : EffectOnPlaceable {
    TProperty value;
    Expression<Func<T, TProperty>> expression;
    public override Effect Clone()
    {
        throw new System.NotImplementedException();
    }

    //Used like this: SetPropertyFromValue(value, o => o.Property1);
   public ParameterChangeV2(TProperty value, Expression<Func<T, TProperty>> expr)
        {
        this.value = value;
        this.expression = expr;
        }
    public ParameterChangeV2(ParameterChangeV2<T,TProperty> other)
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
        PropertyInfo property = (PropertyInfo)member.Member;
        property.SetValue(Target, value, null);
    }
}
