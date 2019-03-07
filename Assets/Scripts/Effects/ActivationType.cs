using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used by effect to know when they should be trigger
/// </summary>
public enum ActivationType
{
    /// <summary>
    /// Trigger when applied.
    /// </summary>
    INSTANT,
    /// <summary>
    /// Trigger at the beginning of the turn.
    /// </summary>
    BEGINNING_OF_TURN,
    /// <summary>
    /// Trigger at the end of the turn.
    /// </summary>
    END_OF_TURN
}
