using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Grid))]
public class GridSearcher : Editor
{
    public Vector3Int position = Vector3Int.one;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Grid grid = (Grid)target;

        position = EditorGUILayout.Vector3IntField("Position", position);
        
        if (GUILayout.Button("Search"))
        {
            Placeable searchResult = grid.GetPlaceableFromVector(position);
            if (searchResult != null)
            {
                Debug.Log("At" + position.x + ", " + position.y + ", " + position.z + " is " + searchResult, searchResult);
            }
            else
            {
                Debug.Log("This Cube is empty, sorry bro");
            }
        }
    }
}
