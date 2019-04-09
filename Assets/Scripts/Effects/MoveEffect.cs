using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEffect : EffectOnLiving {
    [SerializeField]
    private Vector3Int direction;
    [SerializeField]
    private bool useBezier=false;
    [SerializeField]
    private bool useGravity = true;
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

    public bool UseGravity { get => useGravity; set => useGravity = value; }

    public MoveEffect()
    {
        Direction=new Vector3Int();
    }
    public MoveEffect(Vector3Int direction, bool useBezier, bool useGravity)
    {
        this.Direction = direction;
        this.UseBezier = useBezier;
        this.UseGravity = useGravity;
    }
    public MoveEffect(Vector3Int direction, int numberOfturn, bool useBezier, bool useGravity, bool triggerAtEnd = false, ActivationType activationType = ActivationType.INSTANT) : base(numberOfturn, triggerAtEnd, activationType)
    {
        this.Direction = direction;
        this.UseBezier = useBezier;
    }
    public MoveEffect(MoveEffect other) : base(other)
    {
        this.Direction = other.Direction;
        this.UseBezier = other.useBezier;
        this.UseGravity = other.UseGravity;
    }

    public override void Initialize(LivingPlaceable livingPlaceable)
    {
        base.Initialize(livingPlaceable);
        this.Target = livingPlaceable;
        this.Launcher = livingPlaceable ;
    }

    public override void Preview(NetIdeable target)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Move the object
    /// </summary>
    /// <param name="target"></param>
    /// <param name="launcher"></param>
    /// <param name="direction"></param>
    /// <param name="useBezier"></param>
    public MoveEffect(LivingPlaceable target, Placeable launcher, Vector3Int direction,bool useBezier, bool useGravity) : base(target, launcher)
    {
        this.Direction = direction;
        this.UseBezier = useBezier;
        this.UseGravity = useGravity;
    }


    public override Effect Clone()
    {
        return new MoveEffect(this);
    }

    override
        public void Use()
    {
        if(Target.movable && Grid.instance.CheckNull(Target.GetPosition() + direction))
        {
            Grid.instance.MovePlaceable(Target, Target.GetPosition() + direction, UseBezier);
            Debug.Log("Has been logically moved");

            //TODO mettre une animation
           if(UseBezier) //Coroutine conflicts, annoying, sorry
            {
                /* Animator animLauncher = GameManager.instance.playingPlaceable.gameObject.GetComponent<Animator>();

                 Animator animTarget = Target.gameObject.GetComponent<Animator>();
                 List<Vector3> path= new List<Vector3>() { Target.GetPosition() - new Vector3(0, 1, 0), Target.GetPosition() - new Vector3(0, 1, 0) 
                     + direction };
                 AnimationHandler.Instance.StartCoroutine(AnimationHandler.Instance.WaitAndPushBlock(Target, path, pushSpeed, GetTimeOfLauncherAnimation()));
                */
            }
            if (UseGravity)
            {
                Grid.instance.Gravity(Target.GetPosition().x, Target.GetPosition().y, Target.GetPosition().z);
            }
        }
    }

    public override void ResetPreview(NetIdeable target)
    {
        throw new System.NotImplementedException();
    }
}
