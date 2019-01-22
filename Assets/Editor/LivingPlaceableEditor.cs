using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LivingPlaceable))]
public class LivingPlaceableEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LivingPlaceable living = (LivingPlaceable)base.target;

        if (GUILayout.Button("Convert"))
        {
            living.Save(living.Classname + ".json");
        }
    }
}
