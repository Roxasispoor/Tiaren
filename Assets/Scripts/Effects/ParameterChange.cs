using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterChange<T> : Effect {

    
    public delegate void DelegateSetter(T value);
    private DelegateSetter delegateSetter;
    private T paramNewValue; 

    
    public ParameterChange(Placeable cible,Placeable lanceur,DelegateSetter delegateSetter, T paramNewValue):base(cible,lanceur)
    {
        this.delegateSetter = delegateSetter;
        this.paramNewValue=paramNewValue;
    }

    override
        public void Use()
    {
      
        this.delegateSetter(paramNewValue);
    }
    

}
