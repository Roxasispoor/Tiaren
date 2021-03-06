﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pull : Push
{
    public Pull(Push other) : base(other)
    {
    }

    public Pull(int nbCases, float damage) : base(nbCases, damage)
    {
    }

    public Pull(int nbCases, float damage, bool doesHeightCount) : base(nbCases, damage, doesHeightCount)
    {
    }

    public Pull(int nbCases, float damage, Vector3 direction) : base(nbCases, damage, direction)
    {
    }

    public Pull(Placeable target, Placeable launcher, int nbCases, float damage) : base(target, launcher, nbCases, damage)
    {
    }
    public override void GetDirection()
    {
        direction = Launcher.GetPosition() - Target.GetPosition();
    }
    public override void Use()
    {
        base.Use();
    }
}
