using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

using Animation;

public class PushSkill : Skill
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private int nbCases;

    Push PushEffect { get { return (Push)effects[0]; } }


    public PushSkill(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a pushing skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["PushSkill"]);
        InitSpecific(deserializedSkill["PushSkill"]);
    }

    protected void InitSpecific(JToken deserializedSkill)
    {
        damage = (int)deserializedSkill["damage"];
        nbCases = (int)deserializedSkill["nbCases"];
        effects = new List<Effect>();
        effects.Add(new Push(nbCases, damage));
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        PushEffect.Launcher = caster;
        target.DispatchEffect(PushEffect);
    }

    public override void Preview(NetIdeable target)
    {
        Debug.LogError("Preview not implemented");
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        vect = PatternPush(position, vect);
        LivingPlaceable caster = (LivingPlaceable)Grid.instance.GetPlaceableFromVector(position);
        for (int i = 0; i < vect.Count; i++)
        {
            if (caster != null && !CheckSpecificConditions(caster, vect[i]))
            {
                vect.Remove(vect[i]);
            }
        }
        return vect;
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        if (target as Placeable == null)
        {
            Debug.LogError("target is not a placeable! Not good in push");
            return false;
        }
        return true;
    }

    protected override void InitializeAnimation(LivingPlaceable caster, NetIdeable target)
    {
        base.InitializeAnimation(caster, target);
        animation = new PushAnimation(caster, target);
    }

    public class PushAnimation : AnimationBlock
    {

        //MoveAlongBezier(List<Vector3> path, Placeable placeable, Animator animator = null, float speed = 4f, bool useCurve = false)
        Placeable launcher;
        Animator animatorLauncher;
        NetIdeable target;

        public PushAnimation(LivingPlaceable launcher, NetIdeable target)
        {
            this.launcher = launcher;
            this.animatorLauncher = this.launcher.GetComponent<Animator>();
            this.target = target;
            
        }
        public override IEnumerator Animate()
        {
            Vector3 targetPosition = target.VisualTransform.position;
            Vector3 launcherPosition = target.VisualTransform.position;
            Vector3 positionToLookAt = new Vector3(targetPosition.x, launcherPosition.y, targetPosition.z);

            launcher.VisualTransform.LookAt(positionToLookAt);
            animatorLauncher.SetTrigger("Push");

            yield return null;
        }
    }

}
