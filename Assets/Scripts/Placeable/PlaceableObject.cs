using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject{

    private string name;
    private List<Effect> effects;

    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }

    public List<Effect> Effects
    {
        get
        {
            return effects;
        }

        set
        {
            effects = value;
        }
    }

    public void Activate()
    {
        foreach (Effect effect in Effects)
        {
            effect.Use();
        }
    }
}