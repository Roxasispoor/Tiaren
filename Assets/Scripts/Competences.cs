using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Competence {
    private int cost;
    private int cooldown;
    private List<Effect> effects;
    public delegate bool DelegateCondition();
    public DelegateCondition condition;

    private DelegateCondition Condition
    {
        get
        {
            return condition;
        }

        set
        {
            condition = value;
        }
    }
}
