﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEffect : EffectOnLiving {
    [SerializeField]
    private Vector3Int direction;
    [SerializeField]
    private bool useBezier=false;
    [SerializeField]
    private const float pushSpeed = 5f;
    public Vector3Int Direction
    {
        get
        {
            return direction;
        }

        set
        {
            direction = value;
        }
    }

    public bool UseBezier
    {
        get
        {
            return useBezier;
        }

        set
        {
            useBezier = value;
        }
    }

    public MoveEffect()
    {
        Direction=new Vector3Int();
    }
    public MoveEffect(Vector3Int direction,bool useBezier)
    {
        this.Direction = direction;
        this.UseBezier = useBezier;
    }
    public MoveEffect(Vector3Int direction, int numberOfturn, bool useBezier,  bool triggerAtEnd = false, bool hitOnDirectAttack = true) : base(numberOfturn, triggerAtEnd, hitOnDirectAttack)
    {
        this.Direction = direction;
        this.UseBezier = useBezier;
    }
    public MoveEffect(MoveEffect other) : base(other)
    {
        this.Direction = other.Direction;
        this.UseBezier = other.useBezier;
    }

    public override void Initialize(LivingPlaceable livingPlaceable)
    {
        base.Initialize(livingPlaceable);
        this.Target = livingPlaceable;
        this.Launcher = livingPlaceable ;
    }

    /// <summary>
    /// create a damage object
    /// </summary>
    /// <param name="target">target of the action</param>
    /// <param name="launcher">launcher of the action</param>
    /// <param name="damageValue">quanitity of damage given</param>
    public MoveEffect(LivingPlaceable target, Placeable launcher, Vector3Int direction,bool useBezier) : base(target, launcher)
    {
        this.Direction = direction;
        this.UseBezier = useBezier;
    }


    public override Effect Clone()
    {
        return new MoveEffect(this);
    }

    override
        public void Use()
    {
        if(Target.Movable && Grid.instance.CheckNull(Target.GetPosition() + direction))
        {
            Grid.instance.MoveBlock(Target, Target.GetPosition() + direction, !UseBezier);
            Debug.Log("Has been logically moved");
           if(UseBezier) //Coroutine conflicts, annoying, sorry
            {
               /* Animator animLauncher = GameManager.instance.playingPlaceable.gameObject.GetComponent<Animator>();

                Animator animTarget = Target.gameObject.GetComponent<Animator>();
                List<Vector3> path= new List<Vector3>() { Target.GetPosition() - new Vector3(0, 1, 0), Target.GetPosition() - new Vector3(0, 1, 0) 
                    + direction };
                AnimationHandler.Instance.StartCoroutine(AnimationHandler.Instance.WaitAndPushBlock(Target, path, pushSpeed, GetTimeOfLauncherAnimation()));
            */}
        }

       


    }
}
