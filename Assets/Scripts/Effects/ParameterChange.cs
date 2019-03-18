using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Effect changing value of an attribute. Need a specialized getter/setter (javatyped)
/// </summary>
/// <typeparam name="T"></typeparam>
public class ParameterChange<T> : EffectOnPlaceable
{
    public delegate void DelegateSetter(T value);
    private DelegateSetter delegateSetter;
    private T paramNewValue;


    

    public ParameterChange(Placeable target, Placeable launcher, DelegateSetter delegateSetter, T paramNewValue) : base(target, launcher)
    {
        this.delegateSetter = delegateSetter;
        this.paramNewValue = paramNewValue;
    }

    public ParameterChange(ParameterChange<T> other) : base(other)
    {
        this.delegateSetter = other.delegateSetter;
        this.paramNewValue = other.paramNewValue;
    }

    public override void Preview(Placeable target)
    {
        throw new System.NotImplementedException();
    }

    override
        public void Use()
    {

        this.delegateSetter(paramNewValue);
    }

    public override Effect Clone()
    {
        return new ParameterChange<T>(this);
    }

    public override void ResetPreview(Placeable target)
    {
        throw new NotImplementedException();
    }
}
