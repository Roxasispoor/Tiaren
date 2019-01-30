using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackAndPlaceable {

    private LivingPlaceable character;
    private float speedStack;
    private bool seenBefore;



    public StackAndPlaceable(LivingPlaceable character, float speedStack, bool seenBefore)
    {
        this.Character = character;
        this.SpeedStack = speedStack;
        this.SeenBefore = seenBefore;
    }

    public float SpeedStack
    {
        get
        {
            return speedStack;
        }

        set
        {
            speedStack = value;
        }
    }

    public LivingPlaceable Character
    {
        get
        {
            return character;
        }

        set
        {
            character = value;
        }
    }

    public bool SeenBefore
    {
        get
        {
            return seenBefore;
        }

        set
        {
            seenBefore = value;
        }
    }
}
