using UnityEngine;

/// <summary>
/// Struct containing the info of the placeable hitted by the RecastSelector
/// </summary>
public struct HoveredInfo
{
    public HoveredInfo (Placeable placeable, Vector3 face)
    {
        this.placeable = placeable;
        this.face = face;
    }

    /// <summary>
    /// The placeable hitted.
    /// </summary>
    Placeable placeable;
    /// <summary>
    /// The face hitted.
    /// </summary>
    Vector3 face;
}
