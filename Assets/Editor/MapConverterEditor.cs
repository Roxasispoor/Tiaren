using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapConverter))]
public class MapConverterEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //DrawDefaultInspector();

        //GUILayout.TextField("txt file", inPath);
        

        MapConverter mapConverter = (MapConverter) base.target;

        if (GUILayout.Button("Convert"))
        {
            //mapConverter.;
            mapConverter.ConvertFromGUI();
        }
    }

}
