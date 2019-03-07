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
            if (grid.GridMatrix[position.x, position.y, position.z] != null)
            {
                Debug.Log("At" + position.x + ", " + position.y + ", " + position.z + " is " + grid.GridMatrix[position.x, position.y, position.z]);
            }
            else
            {
                Debug.Log("This Cube is empty, sorry bro");
            }
        }
    }
}
