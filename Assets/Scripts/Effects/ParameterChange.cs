using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Effet permettant de changer la valeur d'un attribut. Nécessite un getter/setter spécialisé, typé java
/// </summary>
/// <typeparam name="T"></typeparam>
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
