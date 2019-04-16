using UnityEngine;

/// <summary>
/// Struct containing the info of the placeable hitted by the RecastSelector
/// </summary>
public struct SelectionInfo
{
    public SelectionInfo (Placeable placeable, Vector3 face, int orientationState)
    {
        this.placeable = placeable;
        this.face = face;
        this.orientationState = orientationState;
    }
    
    /// <summary>
    /// Create an array communicable through network
    /// </summary>
    /// <returns></returns>
    public int[] ConvertForNetwork()
    {
        int[] array = new int[5];
        array[0] = placeable.netId;
        array[1] = (int) face.x;
        array[2] = (int) face.y;
        array[3] = (int) face.z;
        array[4] = orientationState;
        return array;
    }

    /// <summary>
    /// Read the array to fill the object.
    /// </summary>
    /// <param name="buffer">The array received through the network</param>
    public void FillFromNetwork(int[] buffer)
    {
        placeable = GameManager.instance.FindLocalObject(buffer[0]);
        face = new Vector3(buffer[1], buffer[2], buffer[3]);
        orientationState = buffer[4];
    }

    /// <summary>
    /// The placeable hitted.
    /// </summary>
    public Placeable placeable;
    /// <summary>
    /// The face hitted.
    /// </summary>
    public Vector3 face;
    /// <summary>
    /// The orientation of the selected area (used for line for example).
    /// </summary>
    public int orientationState;
}
