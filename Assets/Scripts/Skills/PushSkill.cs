﻿using System.Collections;
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

    public class PushAnimation : AnimationBlock
    {

        //MoveAlongBezier(List<Vector3> path, Placeable placeable, Animator animator = null, float speed = 4f, bool useCurve = false)
        List<Vector3> path;
        Placeable launcher;
        Animator animatorLauncher;
        Placeable target;
        Animator animatorTarget;
        const float speedBlocPerSecond = 2.5f;

        public PushAnimation(LivingPlaceable launcher, List<Vector3> path)
        {
            this.launcher = launcher;
            this.animatorLauncher = this.launcher.GetComponent<Animator>();

            this.path = path;
        }
        public override IEnumerator Animate()
        {
            int stepInPath = 0;

            //float overTime = 0;

            displacementElement currentElement;

            animatorLauncher.SetBool("walking", true);

            while (stepInPath < path.Count - 1)
            {
                Vector3 lookAtPosition = new Vector3(path[stepInPath + 1].x, launcher.transform.position.y, path[stepInPath + 1].z);
                launcher.transform.LookAt(lookAtPosition);

                if (path[stepInPath].y == path[stepInPath + 1].y)
                {
                    currentElement = new Segment(path[stepInPath], path[stepInPath + 1]);
                }
                else
                {
                    currentElement = new JumpCurve(path[stepInPath], path[stepInPath + 1]);
                    animatorLauncher.SetTrigger("Jump");
                }


                float currentPercentage = 0;

                while (currentPercentage < 1)
                {

                    currentPercentage += Mathf.Min(speedBlocPerSecond * Time.deltaTime, 1);

                    if (currentPercentage > 0.9)
                    {
                        animatorLauncher.SetTrigger("Land");
                    }

                    launcher.transform.position = currentElement.computePosition(currentPercentage);
                    yield return null;

                }

                stepInPath++;
            }

            animatorLauncher.SetBool("walking", false);
        }
    }

}
