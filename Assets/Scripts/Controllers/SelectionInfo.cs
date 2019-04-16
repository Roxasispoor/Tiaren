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
