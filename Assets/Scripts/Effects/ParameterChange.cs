using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Effect changing value of an attribute. Need a specialized getter/setter (javatyped)
/// </summary>
/// <typeparam name="T"></typeparam>
public class ParameterChange<T> : Effect
{
    public delegate void DelegateSetter(T value);
    private DelegateSetter delegateSetter;
    private T paramNewValue;


    public ParameterChange(Placeable target, Placeable launcher, DelegateSetter delegateSetter, T paramNewValue) : base(target, launcher)
    {
        this.delegateSetter = delegateSetter;
        this.paramNewValue = paramNewValue;
    }

    override
        public void Use()
    {

        this.delegateSetter(paramNewValue);
    }


}
